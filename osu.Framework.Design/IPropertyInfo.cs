using System;
using System.Reflection;

namespace osu.Framework.Design
{
    public interface IPropertyInfo
    {
        string Name { get; }

        Type PropertyType { get; }
        Type DeclaringType { get; }
        MemberTypes MemberTypes { get; }

        object GetValue(object obj);
        void SetValue(object obj, object value);
    }
}