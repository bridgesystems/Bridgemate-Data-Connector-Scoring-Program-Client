using System.Reflection;
using BridgeSystems.Bridgemate.DataConnector.ScoringProgramClient;
using BridgeSystems.Bridgemate.DataConnectorClasses.SharedDTO;

namespace DtoGenerator
{
    /// <summary>
    /// The kind of a DTO property as far as the emitters are concerned.
    /// </summary>
    internal enum PropKind
    {
        String,
        Int,
        Bool,
        StringArray,
        Enum,
        Dto,
        DtoArray
    }

    internal sealed record PropModel(PropertyInfo Info, PropKind Kind, Type? ElementType)
    {
        public string Name => Info.Name;
    }

    internal sealed record ConstModel(FieldInfo Info)
    {
        public string Name => Info.Name;
        public object? Value => Info.GetRawConstantValue();
    }

    internal static class TypeGraph
    {
        /// <summary>
        /// The v1 surface: the envelope, its enums and the DTOs used by the core workflow
        /// (initialize/continue, send/update, poll and accept). Management DTOs are deferred.
        /// </summary>
        public static readonly Type[] Seeds =
        {
            typeof(ScoringProgramRequest),
            typeof(ScoringProgramResponse),
            typeof(ScoringProgramDataConnectorCommands),
            typeof(DataConnectorResponseData),
            typeof(ErrorType),
            typeof(TableDirection),
            typeof(InitDTO),
            typeof(ContinueDTO),
            typeof(SessionDTO),
            typeof(ScoringGroupDTO),
            typeof(SectionDTO),
            typeof(SectionUpdateDTO),
            typeof(TableDTO),
            typeof(RoundDTO),
            typeof(ResultDTO),
            typeof(PlayerDataDTO),
            typeof(ParticipationDTO),
            typeof(HandrecordDTO),
            typeof(TdCallDTO),
            typeof(Bridgemate2SettingsDTO),
            typeof(Bridgemate3SettingsDTO)
        };

        /// <summary>
        /// Returns the transitive closure of the seed types over property types and base classes,
        /// classes first (base before derived), then enums, alphabetically within each group.
        /// </summary>
        public static List<Type> Collect()
        {
            var assembly = typeof(ScoringProgramRequest).Assembly;
            var found = new HashSet<Type>();
            var queue = new Queue<Type>(Seeds);
            while (queue.Count > 0)
            {
                var type = queue.Dequeue();
                if (type.IsArray)
                    type = type.GetElementType()!;
                if (type.Assembly != assembly || !found.Add(type))
                    continue;
                if (type.IsEnum)
                    continue;
                if (type.BaseType != null && type.BaseType.Assembly == assembly)
                    queue.Enqueue(type.BaseType);
                foreach (var prop in Properties(type))
                    queue.Enqueue(prop.PropertyType);
            }
            return found
                .OrderBy(t => t.IsEnum ? 1 : 0)
                .ThenBy(t => InheritanceDepth(t))
                .ThenBy(t => t.Name, StringComparer.Ordinal)
                .ToList();
        }

        private static int InheritanceDepth(Type type)
        {
            var depth = 0;
            for (var t = type.BaseType; t != null; t = t.BaseType)
                depth++;
            return depth;
        }

        /// <summary>
        /// The public read/write instance properties declared on the type itself, in declaration order.
        /// A getter-only property would silently change the wire format, so it is rejected loudly.
        /// </summary>
        public static IReadOnlyList<PropertyInfo> Properties(Type type)
        {
            var props = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .OrderBy(p => p.MetadataToken)
                .ToList();
            var readOnly = props.Where(p => p.GetGetMethod() == null || p.GetSetMethod() == null).ToList();
            if (readOnly.Count > 0)
                throw new NotSupportedException(
                    $"{type.Name} has non-read/write public properties ({string.Join(", ", readOnly.Select(p => p.Name))}); " +
                    "the generator only understands plain get/set properties.");
            return props;
        }

        /// <summary>
        /// The properties of the type including inherited ones, base class first. Used for
        /// (de)serialization code, which must cover the whole object.
        /// </summary>
        public static IReadOnlyList<PropModel> AllPropModels(Type type)
        {
            var chain = new List<Type>();
            for (var t = type; t != null && t != typeof(object); t = t.BaseType)
                chain.Insert(0, t);
            return chain.SelectMany(t => Properties(t).Select(Classify)).ToList();
        }

        public static IReadOnlyList<PropModel> OwnPropModels(Type type) =>
            Properties(type).Select(Classify).ToList();

        public static PropModel Classify(PropertyInfo prop)
        {
            var type = prop.PropertyType;
            if (type == typeof(string))
                return new PropModel(prop, PropKind.String, null);
            if (type == typeof(int) || type == typeof(byte))
                return new PropModel(prop, PropKind.Int, null);
            if (type == typeof(bool))
                return new PropModel(prop, PropKind.Bool, null);
            if (type == typeof(string[]))
                return new PropModel(prop, PropKind.StringArray, null);
            if (type.IsEnum)
                return new PropModel(prop, PropKind.Enum, type);
            if (type.IsArray && type.GetElementType() is { IsClass: true } element && element.Assembly == prop.DeclaringType!.Assembly)
                return new PropModel(prop, PropKind.DtoArray, element);
            if (type.IsClass && type.Assembly == prop.DeclaringType!.Assembly)
                return new PropModel(prop, PropKind.Dto, type);
            throw new NotSupportedException($"Unsupported property type {type} on {prop.DeclaringType!.Name}.{prop.Name}.");
        }

        /// <summary>
        /// The public constants declared on the type itself (the int/string protocol constants on the DTOs).
        /// </summary>
        public static IReadOnlyList<ConstModel> Constants(Type type) =>
            type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(f => f.IsLiteral && (f.FieldType == typeof(int) || f.FieldType == typeof(string)))
                .Where(f => f.GetCustomAttribute<ObsoleteAttribute>() == null)
                .OrderBy(f => f.MetadataToken)
                .Select(f => new ConstModel(f))
                .ToList();

        /// <summary>
        /// The enum member representing value 0, or null when the enum has none. Ports use it as
        /// the default so that an unset property serializes as 0, exactly like the C# default.
        /// </summary>
        public static string? ZeroCase(Type enumType) =>
            Enum.GetValues(enumType).Cast<object>()
                .Where(v => Convert.ToInt32(v) == 0)
                .Select(v => v.ToString())
                .FirstOrDefault();

        public static IReadOnlyList<(string Name, int Value)> EnumMembers(Type enumType)
        {
            var names = Enum.GetNames(enumType);
            return names
                .Select(n => (Name: n, Value: Convert.ToInt32(Enum.Parse(enumType, n))))
                .ToList();
        }
    }
}
