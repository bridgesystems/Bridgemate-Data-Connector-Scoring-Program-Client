using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DtoGenerator
{
    /// <summary>
    /// Reads the compiler-generated XML documentation file of the client assembly so the
    /// generated PHP/Java code carries the same summaries as the C# source.
    /// </summary>
    internal sealed class XmlDocs
    {
        private readonly Dictionary<string, string> _summaries = new();

        public static XmlDocs Load(Assembly assembly)
        {
            var docs = new XmlDocs();
            var xmlPath = Path.ChangeExtension(assembly.Location, ".xml");
            if (!File.Exists(xmlPath))
            {
                Console.WriteLine($"Warning: no XML documentation file found at {xmlPath}; generated code will lack doc comments.");
                return docs;
            }
            var document = XDocument.Load(xmlPath);
            foreach (var member in document.Descendants("member"))
            {
                var name = member.Attribute("name")?.Value;
                var summary = member.Element("summary");
                if (name == null || summary == null)
                    continue;
                var text = Flatten(summary);
                if (text.Length > 0)
                    docs._summaries[name] = text;
            }
            return docs;
        }

        public string? ForType(Type type) => Lookup($"T:{type.FullName}");

        public string? ForProperty(PropertyInfo prop) =>
            Lookup($"P:{prop.DeclaringType!.FullName}.{prop.Name}");

        public string? ForField(FieldInfo field) =>
            Lookup($"F:{field.DeclaringType!.FullName}.{field.Name}");

        public string? ForEnumMember(Type enumType, string memberName) =>
            Lookup($"F:{enumType.FullName}.{memberName}");

        private string? Lookup(string key) => _summaries.TryGetValue(key, out var value) ? value : null;

        private static string Flatten(XElement summary)
        {
            var parts = new List<string>();
            foreach (var node in summary.DescendantNodes())
            {
                if (node is XText text)
                    parts.Add(text.Value);
                else if (node is XElement { Name.LocalName: "see" } see && see.IsEmpty)
                {
                    var cref = see.Attribute("cref")?.Value ?? see.Attribute("langword")?.Value ?? "";
                    parts.Add(cref.Split('.').Last());
                }
            }
            var flattened = string.Join("", parts);
            return Regex.Replace(flattened, @"\s+", " ").Trim();
        }
    }
}
