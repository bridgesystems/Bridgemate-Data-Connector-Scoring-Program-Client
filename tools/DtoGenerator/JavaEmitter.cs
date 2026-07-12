using System.Text;

namespace DtoGenerator
{
    /// <summary>
    /// Emits Java 11 POJOs and enums into the dto package of the Java client. Fields are public
    /// and camelCase with a @JsonProperty annotation carrying the PascalCase wire name; enums are
    /// int-backed with @JsonValue/@JsonCreator so they serialize as integers like System.Text.Json does.
    /// </summary>
    internal sealed class JavaEmitter
    {
        private const string Package = "nl.bridgemate.dataconnector.dto";

        private static readonly HashSet<string> ReservedWords = new(StringComparer.Ordinal)
        {
            "abstract", "assert", "boolean", "break", "byte", "case", "catch", "char", "class", "const",
            "continue", "default", "do", "double", "else", "enum", "extends", "final", "finally", "float",
            "for", "goto", "if", "implements", "import", "instanceof", "int", "interface", "long", "native",
            "new", "package", "private", "protected", "public", "return", "short", "static", "strictfp",
            "super", "switch", "synchronized", "this", "throw", "throws", "transient", "try", "void",
            "volatile", "while"
        };

        private readonly XmlDocs _docs;
        private readonly string _header;

        public JavaEmitter(XmlDocs docs, string header)
        {
            _docs = docs;
            _header = header;
        }

        public void Emit(IEnumerable<Type> types, string outDir)
        {
            Directory.CreateDirectory(outDir);
            foreach (var type in types)
            {
                var code = type.IsEnum ? EmitEnum(type) : EmitClass(type);
                File.WriteAllText(Path.Combine(outDir, type.Name + ".java"), code, new UTF8Encoding(false));
            }
        }

        private string EmitEnum(Type type)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"// {_header}");
            sb.AppendLine($"package {Package};");
            sb.AppendLine();
            sb.AppendLine("import com.fasterxml.jackson.annotation.JsonCreator;");
            sb.AppendLine("import com.fasterxml.jackson.annotation.JsonValue;");
            sb.AppendLine();
            AppendJavadoc(sb, "", _docs.ForType(type));
            sb.AppendLine($"public enum {type.Name} {{");
            var members = TypeGraph.EnumMembers(type);
            for (var i = 0; i < members.Count; i++)
            {
                var (name, value) = members[i];
                AppendJavadoc(sb, "    ", _docs.ForEnumMember(type, name));
                sb.AppendLine($"    {name}({value}){(i == members.Count - 1 ? ";" : ",")}");
            }
            sb.AppendLine();
            sb.AppendLine("    private final int value;");
            sb.AppendLine();
            sb.AppendLine($"    {type.Name}(int value) {{");
            sb.AppendLine("        this.value = value;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    @JsonValue");
            sb.AppendLine("    public int getValue() {");
            sb.AppendLine("        return value;");
            sb.AppendLine("    }");
            sb.AppendLine();
            var zeroCase = TypeGraph.ZeroCase(type);
            var unknownResult = zeroCase ?? "null";
            sb.AppendLine("    /**");
            sb.AppendLine("     * Maps a wire value back to the enum. Unknown values (from a newer Data Connector)");
            sb.AppendLine($"     * map to {unknownResult} instead of failing the whole response.");
            sb.AppendLine("     */");
            sb.AppendLine("    @JsonCreator");
            sb.AppendLine($"    public static {type.Name} fromValue(int value) {{");
            sb.AppendLine($"        for ({type.Name} member : values()) {{");
            sb.AppendLine("            if (member.value == value) {");
            sb.AppendLine("                return member;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine($"        return {unknownResult};");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string EmitClass(Type type)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"// {_header}");
            sb.AppendLine($"package {Package};");
            sb.AppendLine();
            sb.AppendLine("import com.fasterxml.jackson.annotation.JsonIgnoreProperties;");
            sb.AppendLine("import com.fasterxml.jackson.annotation.JsonProperty;");
            sb.AppendLine();
            AppendJavadoc(sb, "", _docs.ForType(type));
            sb.AppendLine("@JsonIgnoreProperties(ignoreUnknown = true)");
            var baseType = type.BaseType != null && type.BaseType != typeof(object) ? type.BaseType.Name : null;
            sb.Append("public ").Append(type.IsAbstract ? "abstract " : "").Append("class ").Append(type.Name);
            if (baseType != null)
                sb.Append(" extends ").Append(baseType);
            sb.AppendLine(" {");

            var first = true;
            foreach (var constant in TypeGraph.Constants(type))
            {
                if (!first)
                    sb.AppendLine();
                first = false;
                AppendJavadoc(sb, "    ", _docs.ForField(constant.Info));
                var (javaType, literal) = constant.Value is string s
                    ? ("String", JavaString(s))
                    : ("int", constant.Value?.ToString() ?? "0");
                sb.AppendLine($"    public static final {javaType} {constant.Name} = {literal};");
            }

            foreach (var prop in TypeGraph.OwnPropModels(type))
            {
                if (!first)
                    sb.AppendLine();
                first = false;
                AppendJavadoc(sb, "    ", _docs.ForProperty(prop.Info));
                sb.AppendLine($"    @JsonProperty(\"{prop.Name}\")");
                sb.AppendLine($"    {Declaration(prop)}");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string Declaration(PropModel prop)
        {
            var fieldName = FieldName(prop.Name);
            switch (prop.Kind)
            {
                case PropKind.String:
                    return $"public String {fieldName};";
                case PropKind.Int:
                    return $"public int {fieldName};";
                case PropKind.Bool:
                    return $"public boolean {fieldName};";
                case PropKind.StringArray:
                    return $"public String[] {fieldName};";
                case PropKind.Enum:
                    var zeroCase = TypeGraph.ZeroCase(prop.ElementType!);
                    //Initialize to the wire default (0) where the enum names it, mirroring the C# default.
                    return zeroCase != null
                        ? $"public {prop.ElementType!.Name} {fieldName} = {prop.ElementType.Name}.{zeroCase};"
                        : $"public {prop.ElementType!.Name} {fieldName};";
                case PropKind.Dto:
                    return $"public {prop.ElementType!.Name} {fieldName};";
                case PropKind.DtoArray:
                    return $"public {prop.ElementType!.Name}[] {fieldName};";
                default:
                    throw new ArgumentOutOfRangeException(nameof(prop));
            }
        }

        internal static string FieldName(string propertyName)
        {
            var name = char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);
            return ReservedWords.Contains(name) ? name + "Value" : name;
        }

        private static void AppendJavadoc(StringBuilder sb, string indent, string? summary)
        {
            if (summary == null)
                return;
            sb.AppendLine($"{indent}/**");
            foreach (var line in PhpEmitter.Wrap(summary, 100 - indent.Length))
                sb.AppendLine($"{indent} * {line}");
            sb.AppendLine($"{indent} */");
        }

        private static string JavaString(string value) =>
            "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
    }
}
