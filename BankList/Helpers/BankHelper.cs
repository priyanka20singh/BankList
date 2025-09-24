using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BankList.Helpers
{
    public class BankHelper : JsonConverter<string> //it will handle serialization and deserialization of string values.
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    // Convert number to string
                    if (reader.TryGetInt64(out long l))
                        return l.ToString();
                    else if (reader.TryGetDouble(out double d))
                        return d.ToString();
                    break;

                case JsonTokenType.String:
                    return reader.GetString();

                case JsonTokenType.Null:
                    return null;
            }

            // fallback
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}

