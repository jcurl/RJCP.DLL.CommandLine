﻿namespace RJCP.Core.CommandLine
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using Resources;

    internal class OptionMember
    {
        private readonly object m_Options;
        private readonly OptionField m_Field;

        public OptionMember(object options, OptionAttribute attribute, MemberInfo member)
        {
            ThrowHelper.ThrowIfNull(options);
            ThrowHelper.ThrowIfNull(attribute);
            ThrowHelper.ThrowIfNull(member);

            m_Field = new OptionField(member);
            m_Options = options;
            Attribute = attribute;
        }

        public OptionAttribute Attribute { get; private set; }

        public MemberInfo Member { get { return m_Field.Member; } }

        private bool m_Set;

        public bool IsSet
        {
            get { return m_Set; }
            set
            {
                if (m_Set && !value)
                    throw new InvalidOperationException(CmdLineStrings.InternalError_OptionIsAlreadySet);

                m_Set = value;
            }
        }

        public bool ExpectsValue
        {
            get
            {
                if (m_Field.Type == typeof(bool)) return false;
                return true;
            }
        }

        public bool IsList { get { return m_Field.IsList; } }

        public void AddValue(string value)
        {
            if (!IsList)
                throw new ArgumentException(CmdLineStrings.ArgException_InvalidCollection, nameof(value));

            m_Field.Add(m_Options, ChangeType(value, m_Field.ListType));
        }

        public void SetValue(string value)
        {
            m_Field.SetValue(m_Options, ChangeType(value, m_Field.Type));
            IsSet = true;
        }

        public void SetValue(bool value)
        {
            m_Field.SetValue(m_Options, value);
            IsSet = true;
        }

        private static object ChangeType(string value, Type type)
        {
            if (type == typeof(object)) return value;

            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return converter.ConvertFromInvariantString(value);
        }
    }
}
