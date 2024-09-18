using Newtonsoft.Json;
using UnityEngine;
using System;

public class Vector2Converter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Vector2 vector = (Vector2)value;
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(vector.x);
        writer.WritePropertyName("y");
        writer.WriteValue(vector.y);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        float x = 0, y = 0;
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.PropertyName)
            {
                string propertyName = (string)reader.Value;
                if (!reader.Read()) continue;

                if (propertyName == "x") x = (float)Convert.ToDouble(reader.Value);
                if (propertyName == "y") y = (float)Convert.ToDouble(reader.Value);
            }
            if (reader.TokenType == JsonToken.EndObject) break;
        }
        return new Vector2(x, y);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector2);
    }
}
