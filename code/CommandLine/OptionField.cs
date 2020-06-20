namespace RJCP.Core.CommandLine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    internal class OptionField
    {
        public OptionField(MemberInfo member)
        {
            if (member is FieldInfo field) {
                Type = field.FieldType;
            } else if (member is PropertyInfo property) {
                Type = property.PropertyType;
            } else {
                throw new ArgumentException("Not a property or field", nameof(member));
            }
            Member = member;

            GetCollectionType(Type);

#if DEBUG
            if (ListType != null && m_ListAddMethod == null)
                throw new ApplicationException("Collection identified without an Add method");
#endif
        }

        private MethodInfo m_ListAddMethod;

        private void GetCollectionType(Type type)
        {
            if (type.IsGenericType) {
                if (Type.IsGenericTypeDefinition)
                    throw new ArgumentException("Member must be a concrete type, not a generic type definition", nameof(type));
                if (type.GetGenericTypeDefinition() == typeof(ICollection<>)) {
                    ListType = GetCollectionTypeDirect(type);
                    m_ListAddMethod = GetCollectionMethodDirect(type, ListType);
                    return;
                }
            }

            foreach (Type intf in type.GetInterfaces()) {
                if (intf.IsGenericType && intf.GetGenericTypeDefinition() == typeof(ICollection<>)) {
                    ListType = GetCollectionTypeDirect(intf);
                    m_ListAddMethod = GetCollectionMethodDirect(intf, ListType);
                    return;
                }
            }

            if (typeof(IList).IsAssignableFrom(type)) {
                ListType = typeof(object);
                m_ListAddMethod = GetCollectionMethodDirect(type, ListType);
            }
        }

        private static Type GetCollectionTypeDirect(Type type)
        {
            Type[] typeParams = type.GetGenericArguments();
            if (typeParams.Length != 1)
                throw new ArgumentException("Expected only one Generic parameter type argument", nameof(type));
            if (typeParams[0].IsGenericParameter) return null;
            return typeParams[0];
        }

        private static MethodInfo GetCollectionMethodDirect(Type type, Type valueType)
        {
            const string methodName = nameof(IList.Add);
            return type.GetMethod(methodName, new Type[] { valueType });
        }

        public Type Type { get; private set; }

        public bool IsList { get { return ListType != null; } }

        public Type ListType { get; private set; }

        public MemberInfo Member { get; private set; }

        public object GetValue(object obj)
        {
            if (Member is FieldInfo field) {
                return field.GetValue(obj);
            } else if (Member is PropertyInfo property) {
                return property.GetValue(obj, null);
            } else {
                throw new ApplicationException("Not a property or field");
            }
        }

        public void SetValue(object obj, object value)
        {
            if (Member is FieldInfo field) {
                field.SetValue(obj, value);
            } else if (Member is PropertyInfo property) {
                property.SetValue(obj, value, null);
            } else {
                throw new ApplicationException("Not a property or field");
            }
        }

        public void Add(object obj, object value)
        {
            if (!IsList)
                throw new InvalidOperationException("Adding a value to an option that is not a collection");

            m_ListAddMethod.Invoke(GetValue(obj), new object[] { value });
        }
    }
}
