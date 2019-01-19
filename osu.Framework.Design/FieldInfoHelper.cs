using System;
using System.Reflection;

namespace osu.Framework.Design
{
    public class FieldInfoHelper : IPropertyInfo
    {
        readonly FieldInfo _field;

        public FieldInfoHelper(FieldInfo field)
        {
            _field = field;
        }

        public string Name => _field.Name;

        public Type PropertyType => _field.FieldType;
        public Type DeclaringType => _field.DeclaringType;
        public MemberTypes MemberTypes => MemberTypes.Field;

        public object GetValue(object obj) => _field.GetValue(obj);
        public void SetValue(object obj, object value) => _field.SetValue(obj, value);
    }
}