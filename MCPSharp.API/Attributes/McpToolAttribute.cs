using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace MCPSharp.API.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class McpToolAttribute : Attribute
{
    public string Name { get; set; }
    public string Description { get; set; }
    // Schema object and JSON string generated at runtime
    public object? InputSchema { get; private set; }
    public string? InputSchemaJson { get; private set; }
    public Type? InputSchemaType { get; private set; }
    // Allows passing typeof(T) as the third parameter
    public McpToolAttribute(string name = "", string description = "", Type? inputSchemaType = null)
    {
        Name = name;
        Description = description;
        InputSchemaType = inputSchemaType;
        if (inputSchemaType != null)
        {
            var schema = BuildSchemaFromType(inputSchemaType);
            InputSchema = schema;
            InputSchemaJson = JsonSerializer.Serialize(schema);
        }
    }
    private static object BuildSchemaFromType(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var propsDict = new Dictionary<string, object>();
        foreach (var prop in properties)
        {
            var (jsonType, format) = MapToJsonType(prop.PropertyType);
            var desc = prop.GetCustomAttribute<DescriptionAttribute>()?.Description;
            var propSchema = new Dictionary<string, object>
            {
                ["type"] = jsonType
            };
            if (!string.IsNullOrEmpty(format))
                propSchema["format"] = format!;
            if (!string.IsNullOrEmpty(desc))
                propSchema["description"] = desc!;
            propsDict[prop.Name] = propSchema;
        }
        return new Dictionary<string, object>
        {
            ["type"] = "object",
            ["properties"] = propsDict
        };
    }
    private static (string jsonType, string? format) MapToJsonType(Type t)
    {
        t = Nullable.GetUnderlyingType(t) ?? t;
        if (t == typeof(string)) return ("string", null);
        if (t == typeof(bool)) return ("boolean", null);
        if (t == typeof(byte) || t == typeof(sbyte) || t == typeof(short) || t == typeof(ushort) ||
            t == typeof(int) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong))
            return ("integer", null);
        if (t == typeof(float) || t == typeof(double) || t == typeof(decimal))
            return ("number", null);
        if (t == typeof(DateTime) || t == typeof(DateTimeOffset))
            return ("string", "date-time");
        if (t.IsEnum) return ("string", null);
        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(t) && t != typeof(string))
            return ("array", null);
        return ("object", null);
    }
}