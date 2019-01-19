using System;
using System.Reflection;

namespace osu.Framework.Design
{
    public class PropertyInfoHelper : IPropertyInfo
    {
        readonly PropertyInfo _property;

        public PropertyInfoHelper(PropertyInfo property)
        {
            _property = property;
        }

        public string Name => _property.Name;

        public Type PropertyType => _property.PropertyType;
        public Type DeclaringType => _property.DeclaringType;
        public MemberTypes MemberTypes => MemberTypes.Property;

        public object GetValue(object obj) => _property.GetValue(obj);
        public void SetValue(object obj, object value) => _property.SetValue(obj, value);
    }
}