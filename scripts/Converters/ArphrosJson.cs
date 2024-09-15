using Godot;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arphros;

public class ArphrosJson
{
    public static string Serialize(object obj, bool formatted = false)
    {
        var serializeOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new Vector2Converter(), new Vector3Converter(), new Vector4Converter(), new ColorConverter() },
        };
        return JsonSerializer.Serialize(obj, serializeOptions);
    }

    public static T Deserialize<T>(string json)
    {
        var deserializeOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new Vector2Converter(), new Vector3Converter(), new Vector4Converter(), new ColorConverter() },
        };
        return JsonSerializer.Deserialize<T>(json, deserializeOptions);
    }

    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement jsonObject = doc.RootElement;
                float x = jsonObject.GetProperty("x").GetSingle();
                float y = jsonObject.GetProperty("y").GetSingle();

                return new Vector2(x, y);
            }
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("x", value.X);
            writer.WriteNumber("y", value.Y);
            writer.WriteEndObject();
        }
    }

    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement jsonObject = doc.RootElement;
                float x = jsonObject.GetProperty("x").GetSingle();
                float y = jsonObject.GetProperty("y").GetSingle();
                float z = jsonObject.GetProperty("z").GetSingle();

                return new Vector3(x, y, z);
            }
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("x", value.X);
            writer.WriteNumber("y", value.Y);
            writer.WriteNumber("z", value.Z);
            writer.WriteEndObject();
        }
    }

    public class Vector4Converter : JsonConverter<Vector4>
    {
        public override Vector4 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement jsonObject = doc.RootElement;
                float x = jsonObject.GetProperty("x").GetSingle();
                float y = jsonObject.GetProperty("y").GetSingle();
                float z = jsonObject.GetProperty("z").GetSingle();
                float w = jsonObject.GetProperty("w").GetSingle();

                return new Vector4(x, y, z, w);
            }
        }

        public override void Write(Utf8JsonWriter writer, Vector4 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("x", value.X);
            writer.WriteNumber("y", value.Y);
            writer.WriteNumber("z", value.Z);
            writer.WriteNumber("w", value.W);
            writer.WriteEndObject();
        }
    }

    public class ColorConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement jsonObject = doc.RootElement;
                float x = jsonObject.GetProperty("r").GetSingle();
                float y = jsonObject.GetProperty("g").GetSingle();
                float z = jsonObject.GetProperty("b").GetSingle();
                float w = jsonObject.GetProperty("a").GetSingle();

                return new Color(x, y, z, w);
            }
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("r", value.R);
            writer.WriteNumber("g", value.G);
            writer.WriteNumber("b", value.B);
            writer.WriteNumber("a", value.A);
            writer.WriteEndObject();
        }
    }
}
