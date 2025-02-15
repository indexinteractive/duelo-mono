namespace Ind3x.Serialization
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine;

    /// <summary>
    /// Assists in the serialization and deserialization of Unity's <see cref="UnityEngine.Vector3"/> class
    /// </summary>
    public class Vector3Converter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector = (Vector3)value;
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(vector.x);
            writer.WritePropertyName("y");
            writer.WriteValue(vector.y);
            writer.WritePropertyName("z");
            writer.WriteValue(vector.z);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default(Vector3);

            var x = 0f;
            var y = 0f;
            var z = 0f;

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var propertyName = (string)reader.Value;
                    if (propertyName == "x" && reader.Read())
                    {
                        x = Convert.ToSingle(reader.Value);
                    }
                    else if (propertyName == "y" && reader.Read())
                    {
                        y = Convert.ToSingle(reader.Value);
                    }
                    else if (propertyName == "z" && reader.Read())
                    {
                        z = Convert.ToSingle(reader.Value);
                    }
                }

                if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }
            }

            return new Vector3(x, y, z);
        }
    }
}
