namespace Ind3x.Util
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Nearly identical to <see cref="StringEnumConverter"/> but allows a null
    /// or "" json value to default to the first enum value in the conversion.
    /// </summary>
    public class DefaultStringEnumConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null || string.IsNullOrEmpty(reader.Value.ToString()))
            {
                Type enumType = Nullable.GetUnderlyingType(objectType) ?? objectType;

                Array enumValues = Enum.GetValues(enumType);
                if (enumValues.Length > 0)
                {
                    return enumValues.GetValue(0);
                }

                return null;
            }

            return base.ReadJson(reader, objectType, existingValue, serializer);
        }
    }
}
