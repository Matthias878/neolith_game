using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System;

public class Vector2Converter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector2);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Vector2 v = (Vector2)value;

        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(v.x);
        writer.WritePropertyName("y");
        writer.WriteValue(v.y);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);
        float x = obj["x"]?.Value<float>() ?? 0f;
        float y = obj["y"]?.Value<float>() ?? 0f;
        return new Vector2(x, y);
    }
}
