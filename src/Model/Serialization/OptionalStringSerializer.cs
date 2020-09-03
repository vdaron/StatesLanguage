using System;
using Newtonsoft.Json;
using StatesLanguage.Model.States;

namespace StatesLanguage.Model.Serialization
{
    public class OptionalStringSerializer : JsonConverter<OptionalString>
    {
        public override void WriteJson(JsonWriter writer, OptionalString value, JsonSerializer serializer)
        {
            if (value.IsSet)
            {
                if (value.HasValue)
                {
                    writer.WriteValue(value.Value);
                }
                else
                {
                    writer.WriteNull();
                }
            }
        }

        public override OptionalString ReadJson(
            JsonReader reader,
            Type objectType,
            OptionalString existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var s = (string) reader.Value;
            return new OptionalString(s);
        }
    }
}