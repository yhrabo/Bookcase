using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebMVC.Areas.Shelves.ViewModels;

namespace WebMVC.Areas.Shelves.Infrastructure
{
    public class AccessLevelJsonConverter : JsonConverter<AccessLevel>
    {
        public override AccessLevel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string enumValue = reader.GetString();
            return enumValue switch
            {
                "All" => AccessLevel.All,
                "Private" => AccessLevel.Private,
                _ => throw new JsonException()
            };
        }

        public override void Write(Utf8JsonWriter writer, AccessLevel value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
