namespace RJCP.Core.CommandLine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Resources;

    internal class OptionField
    {
        public OptionField(MemberInfo member)
        {
            InitializeOptionField(member, false, null);
        }

        public OptionField(MemberInfo member, bool expectList, Type listType)
        {
            InitializeOptionField(member, expectList, listType);
        }

        private void InitializeOptionField(MemberInfo member, bool expectList, Type listType)
        {
            bool isCollection;
            if (member is FieldInfo field) {
                Type = field.FieldType;
                isCollection = SetCollectionType(Type);
                if (!isCollection) {
                    if (expectList)
                        throw new OptionException(CmdLineStrings.OptionArguments_RequiresCollection);
                    if (!IsSimpleType(Type)) {
                        string message = string.Format(CmdLineStrings.ArgException_FieldNotPrimitive, field.Name);
                        throw new ArgumentException(message);
                    }
                    if (field.IsInitOnly) {
                        string message = string.Format(CmdLineStrings.ArgException_ReadOnlyField, field.Name);
                        throw new ArgumentException(message);
                    }
                } else if (!IsSimpleType(ListType)) {
                    string message = string.Format(CmdLineStrings.ArgException_CollectionNotPrimitive, field.Name);
                    throw new ArgumentException(message);
                }
            } else if (member is PropertyInfo property) {
                Type = property.PropertyType;
                isCollection = SetCollectionType(Type);
                if (!isCollection) {
                    if (expectList)
                        throw new OptionException(CmdLineStrings.OptionArguments_RequiresCollection);
                    if (!IsSimpleType(Type)) {
                        string message = string.Format(CmdLineStrings.ArgException_PropertyNotPrimitive, property.Name);
                        throw new ArgumentException(message);
                    }
                    if (!property.CanWrite) {
                        string message = string.Format(CmdLineStrings.ArgException_ReadOnlyField, property.Name);
                        throw new ArgumentException(message);
                    }
                } else if (!IsSimpleType(ListType)) {
                    string message = string.Format(CmdLineStrings.ArgException_CollectionNotPrimitive, property.Name);
                    throw new ArgumentException(message);
                }
            } else {
                throw new ArgumentException(CmdLineStrings.ArgException_NotPropertyOrField, nameof(member));
            }

            if (isCollection && expectList) {
                if (ListType != typeof(object) && listType != null && !listType.IsAssignableFrom(ListType))
                    throw new OptionException(CmdLineStrings.OptionArguments_GenTypeString);
            }
            Member = member;
        }

        private MethodInfo m_ListAddMethod;

        private bool SetCollectionType(Type type)
        {
            if (type.IsGenericType) {
                if (Type.IsGenericTypeDefinition)
                    throw new ArgumentException(CmdLineStrings.ArgException_InvalidTypeNoGeneric, nameof(type));
                if (type.GetGenericTypeDefinition() == typeof(ICollection<>)) {
                    ListType = GetCollectionTypeDirect(type);
                    m_ListAddMethod = GetCollectionMethodDirect(type, ListType);
                    return true;
                }
            }

            foreach (Type intf in type.GetInterfaces()) {
                if (intf.IsGenericType && intf.GetGenericTypeDefinition() == typeof(ICollection<>)) {
                    ListType = GetCollectionTypeDirect(intf);
                    m_ListAddMethod = GetCollectionMethodDirect(intf, ListType);
                    return true;
                }
            }

            if (typeof(IList).IsAssignableFrom(type)) {
                ListType = typeof(object);
                m_ListAddMethod = GetCollectionMethodDirect(type, ListType);
                return true;
            }

            return false;
        }

        private static Type GetCollectionTypeDirect(Type type)
        {
            Type[] typeParams = type.GetGenericArguments();
            if (typeParams.Length != 1)
                throw new ArgumentException(CmdLineStrings.ArgException_InvalidTypeMultipleGenTypeParams, nameof(type));
            if (typeParams[0].IsGenericParameter) return null;
            return typeParams[0];
        }

        private static MethodInfo GetCollectionMethodDirect(Type type, Type valueType)
        {
            const string methodName = nameof(IList.Add);
            return type.GetMethod(methodName, new Type[] { valueType });
        }
        private static bool IsSimpleType(Type type)
        {
            if (type == typeof(bool)) return true;
            if (type == typeof(string)) return true;
            if (type.IsPrimitive) return true;
            if (type.IsEnum) return true;
            if (type == typeof(object)) return true;
            return false;
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
                throw new InvalidOperationException(CmdLineStrings.ArgException_NotPropertyOrField);
            }
        }

        public void SetValue(object obj, object value)
        {
            if (Member is FieldInfo field) {
                field.SetValue(obj, value);
            } else if (Member is PropertyInfo property) {
                property.SetValue(obj, value, null);
            } else {
                throw new InvalidOperationException(CmdLineStrings.ArgException_NotPropertyOrField);
            }
        }

        public void Add(object obj, object value)
        {
            if (!IsList)
                throw new InvalidOperationException(CmdLineStrings.ArgException_InvalidCollection);

            m_ListAddMethod.Invoke(GetValue(obj), new object[] { value });
        }
    }
}
