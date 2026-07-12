using System.Text;
using System.Text.RegularExpressions;

namespace DtoGenerator
{
    /// <summary>
    /// Emits Python 3.10 dataclasses and IntEnums into the dto package of the Python client.
    /// Fields are snake_case (PEP 8) with explicit to_dict/from_dict methods carrying the
    /// PascalCase wire names; enums are IntEnum so they serialize as integers like
    /// System.Text.Json does. One module per type, plus a generated __init__.py that re-exports
    /// every type and a _support.py with the shared enum mapping helper.
    /// </summary>
    internal sealed class PythonEmitter
    {
        private static readonly HashSet<string> ReservedWords = new(StringComparer.Ordinal)
        {
            "False", "None", "True", "and", "as", "assert", "async", "await", "break", "class",
            "continue", "def", "del", "elif", "else", "except", "finally", "for", "from", "global",
            "if", "import", "in", "is", "lambda", "nonlocal", "not", "or", "pass", "raise",
            "return", "try", "while", "with", "yield"
        };

        private readonly XmlDocs _docs;
        private readonly string _header;

        public PythonEmitter(XmlDocs docs, string header)
        {
            _docs = docs;
            _header = header;
        }

        public void Emit(IEnumerable<Type> types, string outDir)
        {
            var typeList = types.ToList();
            Directory.CreateDirectory(outDir);

            //Windows file systems are case-insensitive, so two type names must never snake-case
            //to the same module name or one file would silently overwrite the other.
            var modules = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            foreach (var type in typeList)
            {
                var module = ModuleName(type);
                if (modules.TryGetValue(module, out var clash))
                    throw new InvalidOperationException(
                        $"{type.Name} and {clash.Name} both map to module {module}.py; rename one or extend the emitter.");
                modules[module] = type;
            }

            foreach (var type in typeList)
            {
                var code = type.IsEnum ? EmitEnum(type) : EmitClass(type);
                File.WriteAllText(Path.Combine(outDir, ModuleName(type) + ".py"), code, new UTF8Encoding(false));
            }
            File.WriteAllText(Path.Combine(outDir, "_support.py"), EmitSupport(), new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(outDir, "__init__.py"), EmitInit(typeList), new UTF8Encoding(false));
        }

        private string EmitEnum(Type type)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# {_header}");
            sb.AppendLine("from enum import IntEnum");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"class {type.Name}(IntEnum):");
            AppendDocstring(sb, "    ", _docs.ForType(type));
            sb.AppendLine();
            foreach (var (name, value) in TypeGraph.EnumMembers(type))
            {
                AppendComment(sb, "    ", _docs.ForEnumMember(type, name));
                sb.AppendLine($"    {MemberName(name)} = {value}");
            }
            var zeroCase = TypeGraph.ZeroCase(type);
            if (zeroCase != null)
            {
                //_missing_ must return a member: returning None from it raises ValueError.
                sb.AppendLine();
                sb.AppendLine("    @classmethod");
                sb.AppendLine("    def _missing_(cls, value):");
                sb.AppendLine("        # Unknown wire values (from a newer Data Connector) map to the zero case");
                sb.AppendLine("        # instead of failing the whole response.");
                sb.AppendLine($"        return cls.{MemberName(zeroCase)}");
            }
            return sb.ToString();
        }

        private string EmitClass(Type type)
        {
            var body = new StringBuilder();
            var isAbstract = type.IsAbstract;
            var baseType = type.BaseType != null && type.BaseType != typeof(object) ? type.BaseType : null;

            body.AppendLine("@dataclass");
            body.Append("class ").Append(type.Name);
            body.Append(baseType != null ? $"({baseType.Name})" : "");
            body.AppendLine(":");
            var summary = _docs.ForType(type);
            AppendDocstring(body, "    ", summary);

            var first = summary == null;
            var emittedAnything = summary != null;
            foreach (var constant in TypeGraph.Constants(type))
            {
                if (!first)
                    body.AppendLine();
                first = false;
                emittedAnything = true;
                AppendComment(body, "    ", _docs.ForField(constant.Info));
                var value = constant.Value is string s ? PyString(s) : constant.Value?.ToString() ?? "None";
                //No annotation on purpose: an annotated class attribute would become a dataclass field.
                body.AppendLine($"    {constant.Name} = {value}");
            }

            foreach (var prop in TypeGraph.OwnPropModels(type))
            {
                if (!first)
                    body.AppendLine();
                first = false;
                emittedAnything = true;
                AppendComment(body, "    ", _docs.ForProperty(prop.Info));
                body.AppendLine($"    {Declaration(prop)}");
            }

            if (!isAbstract)
            {
                var all = TypeGraph.AllPropModels(type);
                emittedAnything = true;
                body.AppendLine();
                body.AppendLine("    def to_dict(self) -> dict:");
                body.AppendLine("        \"\"\"The JSON-ready dict with the PascalCase wire names, in wire order.\"\"\"");
                body.AppendLine("        return {");
                foreach (var prop in all)
                    body.AppendLine($"            \"{prop.Name}\": {SerializeExpression(prop)},");
                body.AppendLine("        }");

                body.AppendLine();
                body.AppendLine("    @classmethod");
                body.AppendLine($"    def from_dict(cls, data: dict) -> \"{type.Name}\":");
                body.AppendLine("        \"\"\"Builds the DTO from a parsed JSON dict; absent or null values keep the field defaults.\"\"\"");
                body.AppendLine("        dto = cls()");
                foreach (var prop in all)
                    body.AppendLine($"        {FromDictStatement(prop)}");
                body.AppendLine("        return dto");
            }

            if (!emittedAnything)
                body.AppendLine("    pass");

            var sb = new StringBuilder();
            sb.AppendLine($"# {_header}");
            sb.AppendLine("from dataclasses import dataclass");
            sb.AppendLine();
            foreach (var import in Imports(type, isAbstract, baseType))
                sb.AppendLine(import);
            sb.AppendLine();
            sb.AppendLine();
            sb.Append(body);
            return sb.ToString();
        }

        /// <summary>
        /// The sorted relative imports a class module needs: its base class, the element types of
        /// its properties and the enum helper. Serialization covers inherited properties too, so a
        /// concrete class imports over the whole property chain; an abstract one (declarations
        /// only) just over its own.
        /// </summary>
        private static IReadOnlyList<string> Imports(Type type, bool isAbstract, Type? baseType)
        {
            var referenced = new HashSet<Type>();
            var needsEnumHelper = false;
            var props = isAbstract ? TypeGraph.OwnPropModels(type) : TypeGraph.AllPropModels(type);
            foreach (var prop in props)
            {
                if (prop.Kind is PropKind.Enum or PropKind.Dto or PropKind.DtoArray && prop.ElementType != type)
                    referenced.Add(prop.ElementType!);
                if (!isAbstract && prop.Kind == PropKind.Enum)
                    needsEnumHelper = true;
            }
            if (baseType != null)
                referenced.Add(baseType);

            var imports = referenced
                .Select(t => $"from .{ModuleName(t)} import {t.Name}")
                .OrderBy(l => l, StringComparer.Ordinal)
                .ToList();
            if (needsEnumHelper)
                imports.Insert(0, "from ._support import enum_from_wire");
            return imports;
        }

        private static string Declaration(PropModel prop)
        {
            var name = FieldName(prop.Name);
            switch (prop.Kind)
            {
                case PropKind.String:
                    return $"{name}: str | None = None";
                case PropKind.Int:
                    return $"{name}: int = 0";
                case PropKind.Bool:
                    return $"{name}: bool = False";
                case PropKind.StringArray:
                    return $"{name}: list[str] | None = None";
                case PropKind.Enum:
                    var enumName = prop.ElementType!.Name;
                    var zeroCase = TypeGraph.ZeroCase(prop.ElementType!);
                    //Default to the wire default (0) where the enum names it, mirroring the C# default.
                    //An enum member is immutable, so it is safe as a dataclass default.
                    return zeroCase != null
                        ? $"{name}: {enumName} = {enumName}.{MemberName(zeroCase)}"
                        : $"{name}: {enumName} | None = None";
                case PropKind.Dto:
                    return $"{name}: {prop.ElementType!.Name} | None = None";
                case PropKind.DtoArray:
                    return $"{name}: list[{prop.ElementType!.Name}] | None = None";
                default:
                    throw new ArgumentOutOfRangeException(nameof(prop));
            }
        }

        private static string SerializeExpression(PropModel prop)
        {
            var name = FieldName(prop.Name);
            switch (prop.Kind)
            {
                case PropKind.String:
                case PropKind.Int:
                case PropKind.Bool:
                case PropKind.StringArray:
                    return $"self.{name}";
                case PropKind.Enum:
                    //int() also normalizes a raw int a caller may have assigned. A null enum must
                    //become 0 on the wire: that is what .NET serializes for an unset enum.
                    return TypeGraph.ZeroCase(prop.ElementType!) != null
                        ? $"int(self.{name})"
                        : $"int(self.{name}) if self.{name} is not None else 0";
                case PropKind.Dto:
                    return $"self.{name}.to_dict() if self.{name} is not None else None";
                case PropKind.DtoArray:
                    return $"[item.to_dict() for item in self.{name}] if self.{name} is not None else None";
                default:
                    throw new ArgumentOutOfRangeException(nameof(prop));
            }
        }

        private static string FromDictStatement(PropModel prop)
        {
            var name = FieldName(prop.Name);
            var wire = prop.Name;
            switch (prop.Kind)
            {
                case PropKind.String:
                case PropKind.StringArray:
                    return $"dto.{name} = data.get(\"{wire}\")";
                case PropKind.Int:
                    //A JSON null falls back to the default, like the PHP/Java ports (and unlike a plain
                    //dict lookup, which would surface None into an int field).
                    return $"dto.{name} = data[\"{wire}\"] if data.get(\"{wire}\") is not None else 0";
                case PropKind.Bool:
                    return $"dto.{name} = data[\"{wire}\"] if data.get(\"{wire}\") is not None else False";
                case PropKind.Enum:
                    var enumName = prop.ElementType!.Name;
                    var zeroCase = TypeGraph.ZeroCase(prop.ElementType!);
                    return zeroCase != null
                        ? $"dto.{name} = enum_from_wire({enumName}, data.get(\"{wire}\"), {enumName}.{MemberName(zeroCase)})"
                        : $"dto.{name} = enum_from_wire({enumName}, data.get(\"{wire}\"))";
                case PropKind.Dto:
                    return $"dto.{name} = {prop.ElementType!.Name}.from_dict(data[\"{wire}\"]) if data.get(\"{wire}\") is not None else None";
                case PropKind.DtoArray:
                    return $"dto.{name} = [{prop.ElementType!.Name}.from_dict(item) for item in data[\"{wire}\"]] if data.get(\"{wire}\") is not None else None";
                default:
                    throw new ArgumentOutOfRangeException(nameof(prop));
            }
        }

        private string EmitSupport()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# {_header}");
            sb.AppendLine("\"\"\"Internal helper shared by the generated DTO modules.\"\"\"");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("def enum_from_wire(enum_cls, value, default=None):");
            sb.AppendLine("    \"\"\"Maps a wire value to the enum.");
            sb.AppendLine();
            sb.AppendLine("    None (absent or JSON null) falls back to the default. Unknown values from a newer");
            sb.AppendLine("    Data Connector either map through the enum's _missing_ hook (enums with a zero");
            sb.AppendLine("    case) or fall back to the default here, instead of failing the whole response.");
            sb.AppendLine("    \"\"\"");
            sb.AppendLine("    if value is None:");
            sb.AppendLine("        return default");
            sb.AppendLine("    try:");
            sb.AppendLine("        return enum_cls(value)");
            sb.AppendLine("    except ValueError:");
            sb.AppendLine("        return default");
            return sb.ToString();
        }

        private string EmitInit(IReadOnlyList<Type> types)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# {_header}");
            sb.AppendLine("\"\"\"Generated DTO layer: the wire-format types shared with the .NET reference client.\"\"\"");
            sb.AppendLine();
            foreach (var type in types)
                sb.AppendLine($"from .{ModuleName(type)} import {type.Name}");
            sb.AppendLine();
            sb.AppendLine("__all__ = [");
            foreach (var type in types)
                sb.AppendLine($"    \"{type.Name}\",");
            sb.AppendLine("]");
            return sb.ToString();
        }

        internal static string ModuleName(Type type) => SnakeCase(type.Name);

        internal static string FieldName(string propertyName) => SnakeCase(propertyName);

        /// <summary>
        /// Enum members keep their C# PascalCase name (matching the developer's guide), except the
        /// capitalized keywords None/True/False, which get the PEP 8 trailing underscore.
        /// </summary>
        internal static string MemberName(string name) =>
            ReservedWords.Contains(name) ? name + "_" : name;

        internal static string SnakeCase(string name)
        {
            var s = Regex.Replace(name, "([A-Z]+)([A-Z][a-z])", "$1_$2");
            s = Regex.Replace(s, "([a-z0-9])([A-Z])", "$1_$2");
            s = s.ToLowerInvariant();
            return ReservedWords.Contains(s) ? s + "_" : s;
        }

        private static void AppendDocstring(StringBuilder sb, string indent, string? summary)
        {
            if (summary == null)
                return;
            var lines = PhpEmitter.Wrap(Escape(summary), 100 - indent.Length - 4).ToList();
            if (lines.Count == 1)
            {
                sb.AppendLine($"{indent}\"\"\"{lines[0]}\"\"\"");
                return;
            }
            sb.AppendLine($"{indent}\"\"\"{lines[0]}");
            foreach (var line in lines.Skip(1))
                sb.AppendLine($"{indent}{line}");
            sb.AppendLine($"{indent}\"\"\"");
        }

        private static void AppendComment(StringBuilder sb, string indent, string? summary)
        {
            if (summary == null)
                return;
            foreach (var line in PhpEmitter.Wrap(summary, 100 - indent.Length - 2))
                sb.AppendLine($"{indent}# {line}");
        }

        private static string PyString(string value) =>
            "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";

        private static string Escape(string value) =>
            value.Replace("\\", "\\\\").Replace("\"\"\"", "\\\"\\\"\\\"");
    }
}
