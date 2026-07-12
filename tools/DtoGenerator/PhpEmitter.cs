using System.Text;

namespace DtoGenerator
{
    /// <summary>
    /// Emits PHP 8.1 classes and backed enums into the Dto namespace of the PHP client package.
    /// Property names stay PascalCase on purpose: they match the wire format and the developer's
    /// guide one to one, and json_encode/fromArray need no name mapping.
    /// </summary>
    internal sealed class PhpEmitter
    {
        private const string Namespace = @"Bridgemate\DataConnector\Dto";
        private readonly XmlDocs _docs;
        private readonly string _header;

        public PhpEmitter(XmlDocs docs, string header)
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
                File.WriteAllText(Path.Combine(outDir, type.Name + ".php"), code, new UTF8Encoding(false));
            }
        }

        private string FileHead()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?php");
            sb.AppendLine();
            sb.AppendLine("declare(strict_types=1);");
            sb.AppendLine();
            sb.AppendLine($"// {_header}");
            sb.AppendLine();
            sb.AppendLine($"namespace {Namespace};");
            sb.AppendLine();
            return sb.ToString();
        }

        private string EmitEnum(Type type)
        {
            var sb = new StringBuilder(FileHead());
            AppendDocBlock(sb, "", _docs.ForType(type));
            sb.AppendLine($"enum {type.Name}: int");
            sb.AppendLine("{");
            foreach (var (name, value) in TypeGraph.EnumMembers(type))
            {
                AppendDocBlock(sb, "    ", _docs.ForEnumMember(type, name));
                sb.AppendLine($"    case {name} = {value};");
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string EmitClass(Type type)
        {
            var sb = new StringBuilder(FileHead());
            var isAbstract = type.IsAbstract;
            var baseType = type.BaseType != null && type.BaseType != typeof(object) ? type.BaseType.Name : null;
            AppendDocBlock(sb, "", _docs.ForType(type));
            sb.Append(isAbstract ? "abstract class " : "class ").Append(type.Name);
            //An abstract PHP class may implement \JsonSerializable without defining the method;
            //the concrete subclasses provide it.
            sb.Append(baseType != null ? $" extends {baseType}" : " implements \\JsonSerializable");
            sb.AppendLine();
            sb.AppendLine("{");

            var first = true;
            foreach (var constant in TypeGraph.Constants(type))
            {
                if (!first)
                    sb.AppendLine();
                first = false;
                AppendDocBlock(sb, "    ", _docs.ForField(constant.Info));
                var value = constant.Value is string s ? PhpString(s) : constant.Value?.ToString() ?? "null";
                sb.AppendLine($"    public const {constant.Name} = {value};");
            }

            foreach (var prop in TypeGraph.OwnPropModels(type))
            {
                if (!first)
                    sb.AppendLine();
                first = false;
                var summary = _docs.ForProperty(prop.Info);
                var varTag = prop.Kind switch
                {
                    PropKind.StringArray => "string[]|null",
                    PropKind.DtoArray => $"{prop.ElementType!.Name}[]|null",
                    _ => null
                };
                AppendDocBlock(sb, "    ", summary, varTag);
                sb.AppendLine($"    {Declaration(prop)}");
            }

            if (!isAbstract)
            {
                var all = TypeGraph.AllPropModels(type);
                sb.AppendLine();
                sb.AppendLine("    /**");
                sb.AppendLine("     * @return array<string, mixed>");
                sb.AppendLine("     */");
                sb.AppendLine("    public function jsonSerialize(): array");
                sb.AppendLine("    {");
                sb.AppendLine("        return [");
                foreach (var prop in all)
                    sb.AppendLine($"            '{prop.Name}' => {SerializeExpression(prop)},");
                sb.AppendLine("        ];");
                sb.AppendLine("    }");

                sb.AppendLine();
                sb.AppendLine("    /**");
                sb.AppendLine("     * @param array<string, mixed> $data");
                sb.AppendLine("     */");
                sb.AppendLine("    public static function fromArray(array $data): self");
                sb.AppendLine("    {");
                sb.AppendLine("        $dto = new self();");
                foreach (var prop in all)
                    sb.AppendLine($"        {FromArrayStatement(prop)}");
                sb.AppendLine("        return $dto;");
                sb.AppendLine("    }");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string Declaration(PropModel prop)
        {
            switch (prop.Kind)
            {
                case PropKind.String:
                    return $"public ?string ${prop.Name} = null;";
                case PropKind.Int:
                    return $"public int ${prop.Name} = 0;";
                case PropKind.Bool:
                    return $"public bool ${prop.Name} = false;";
                case PropKind.StringArray:
                case PropKind.DtoArray:
                    return $"public ?array ${prop.Name} = null;";
                case PropKind.Enum:
                    var zeroCase = TypeGraph.ZeroCase(prop.ElementType!);
                    return zeroCase != null
                        ? $"public {prop.ElementType!.Name} ${prop.Name} = {prop.ElementType.Name}::{zeroCase};"
                        : $"public ?{prop.ElementType!.Name} ${prop.Name} = null;";
                case PropKind.Dto:
                    return $"public ?{prop.ElementType!.Name} ${prop.Name} = null;";
                default:
                    throw new ArgumentOutOfRangeException(nameof(prop));
            }
        }

        private static string SerializeExpression(PropModel prop)
        {
            //json_encode serializes backed enums to their value and JsonSerializable objects through
            //jsonSerialize, so most properties pass through as-is. A null enum must become 0 on the
            //wire: that is what .NET serializes for an unset enum.
            if (prop.Kind == PropKind.Enum && TypeGraph.ZeroCase(prop.ElementType!) == null)
                return $"$this->{prop.Name}?->value ?? 0";
            return $"$this->{prop.Name}";
        }

        private static string FromArrayStatement(PropModel prop)
        {
            var name = prop.Name;
            switch (prop.Kind)
            {
                case PropKind.String:
                case PropKind.StringArray:
                    return $"$dto->{name} = $data['{name}'] ?? null;";
                case PropKind.Int:
                    return $"$dto->{name} = $data['{name}'] ?? 0;";
                case PropKind.Bool:
                    return $"$dto->{name} = $data['{name}'] ?? false;";
                case PropKind.Enum:
                    var enumName = prop.ElementType!.Name;
                    var zeroCase = TypeGraph.ZeroCase(prop.ElementType!);
                    //tryFrom keeps the client working when a newer Data Connector introduces enum
                    //values this package does not know yet.
                    return zeroCase != null
                        ? $"$dto->{name} = {enumName}::tryFrom($data['{name}'] ?? 0) ?? {enumName}::{zeroCase};"
                        : $"$dto->{name} = isset($data['{name}']) ? {enumName}::tryFrom($data['{name}']) : null;";
                case PropKind.Dto:
                    return $"$dto->{name} = isset($data['{name}']) ? {prop.ElementType!.Name}::fromArray($data['{name}']) : null;";
                case PropKind.DtoArray:
                    return $"$dto->{name} = isset($data['{name}']) " +
                           $"? array_map(static fn (array $item): {prop.ElementType!.Name} => {prop.ElementType.Name}::fromArray($item), $data['{name}']) " +
                           ": null;";
                default:
                    throw new ArgumentOutOfRangeException(nameof(prop));
            }
        }

        private static void AppendDocBlock(StringBuilder sb, string indent, string? summary, string? varTag = null)
        {
            if (summary == null && varTag == null)
                return;
            sb.AppendLine($"{indent}/**");
            if (summary != null)
                foreach (var line in Wrap(summary, 100 - indent.Length))
                    sb.AppendLine($"{indent} * {line}");
            if (varTag != null)
                sb.AppendLine($"{indent} * @var {varTag}");
            sb.AppendLine($"{indent} */");
        }

        private static string PhpString(string value) =>
            "'" + value.Replace("\\", "\\\\").Replace("'", "\\'") + "'";

        internal static IEnumerable<string> Wrap(string text, int width)
        {
            var words = text.Split(' ');
            var line = new StringBuilder();
            foreach (var word in words)
            {
                if (line.Length > 0 && line.Length + word.Length + 1 > width)
                {
                    yield return line.ToString();
                    line.Clear();
                }
                if (line.Length > 0)
                    line.Append(' ');
                line.Append(word);
            }
            if (line.Length > 0)
                yield return line.ToString();
        }
    }
}
