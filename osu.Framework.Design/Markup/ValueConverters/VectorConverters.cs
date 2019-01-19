using System;
using osuTK;

namespace osu.Framework.Design.Markup.ValueConverters
{
    public class Vector2Converter : IValueConverter
    {
        public Type ConvertingType => typeof(Vector2);

        public void Serialize(object value, Type type, out string data) => data = Serialize((Vector2)value);
        public static string Serialize(Vector2 v) => $"{v.X}, {v.Y}";

        public void Deserialize(string data, Type type, out object value) => value = Deserialize(data);
        public static Vector2 Deserialize(string data)
        {
            var array = data.Split(',', 2);
            var x = float.Parse(array[0]);
            var y = float.Parse(array[1]);

            return new Vector2(x, y);
        }
    }

    public class Vector3Converter : IValueConverter
    {
        public Type ConvertingType => typeof(Vector3);

        public void Serialize(object value, Type type, out string data) => data = Serialize((Vector3)value);
        public static string Serialize(Vector3 v) => $"{v.X}, {v.Y}, {v.Z}";

        public void Deserialize(string data, Type type, out object value) => value = Deserialize(data);
        public static Vector3 Deserialize(string data)
        {
            var array = data.Split(',', 3);
            var x = float.Parse(array[0]);
            var y = float.Parse(array[1]);
            var z = float.Parse(array[2]);

            return new Vector3(x, y, z);
        }
    }

    public class Vector4Converter : IValueConverter
    {
        public Type ConvertingType => typeof(Vector4);

        public void Serialize(object value, Type type, out string data) => data = Serialize((Vector4)value);
        public static string Serialize(Vector4 v) => $"{v.X}, {v.Y}, {v.Z}, {v.W}";

        public void Deserialize(string data, Type type, out object value) => value = Deserialize(data);
        public static Vector4 Deserialize(string data)
        {
            var array = data.Split(',', 4);
            var x = float.Parse(array[0]);
            var y = float.Parse(array[1]);
            var z = float.Parse(array[2]);
            var w = float.Parse(array[3]);

            return new Vector4(x, y, z, w);
        }
    }
}