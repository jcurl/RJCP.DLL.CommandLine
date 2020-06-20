namespace RJCP.Core.CommandLine
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    internal class OptionMember
    {
        private object m_Options;
        private OptionField m_Field;

        public OptionMember(object options, OptionAttribute attribute, MemberInfo member)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));
            if (member == null) throw new ArgumentNullException(nameof(member));

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
                    throw new InvalidOperationException("Internal Error: Setting an option to false, when it's already set");

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
                throw new InvalidOperationException("Adding a value to an option that is not a collection");

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
