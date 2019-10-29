namespace RJCP.Core.CommandLine
{
    using System;
    using System.Collections.Generic;

    internal class NoArguments
    {
#pragma warning disable CS0649  // This field is set via reflection, so the compiler doesn't know
        public bool NoOption;
#pragma warning restore CS0649
    }

    internal class OptionalArguments
    {
#pragma warning disable CS0649  // This field is set via reflection, so the compiler doesn't know
        [Option('a', "along", false)]
        public bool OptionA;

        [Option('b', "blong", false)]
        public bool OptionB;

        [Option('c', "clong", false)]
        public string OptionC;
#pragma warning restore CS0649
    }

    internal class RequiredArguments
    {
#pragma warning disable CS0649  // This field is set via reflection, so the compiler doesn't know
        [Option('i', "insensitive")]
        private bool m_CaseInsensitive;

        [Option('f', "printfiles")]
        private bool m_PrintFiles;

        [Option('s', "search")]
        private string m_SearchString;
#pragma warning restore CS0649

        public bool CaseInsensitive { get { return m_CaseInsensitive; } }

        public bool PrintFiles { get { return m_PrintFiles; } }

        public string SearchString { get { return m_SearchString; } }
    }

    internal class PropertyOptions
    {
        [Option('a', "along", false)]
        public bool OptionA { get; set; }

        [Option('b', "blong", false)]
        public bool OptionB { get; set; }

        [Option('c', "clong", false)]
        public string OptionC { get; set; }
    }

    internal class RequiredOptions
    {
        [Option('a', "along", false)]
        public bool OptionA { get; set; }

        [Option('b', "blong", false)]
        public bool OptionB { get; set; }

        [Option('c', "clong", true)]
        public string OptionC { get; set; }
    }

    internal class ListOptions
    {
        public ListOptions()
        {
            List = new List<string>();
        }

        [Option('l', "list", false)]
        public List<string> List { get; private set; }
    }

    internal class OptionalLongArguments
    {
#pragma warning disable CS0649  // This field is set via reflection, so the compiler doesn't know
        [Option("along")]
        public bool OptionA;

        [Option("blong")]
        public bool OptionB;

        [Option("clong")]
        public string OptionC;

        [Option("level42")]
        public string Level42;
#pragma warning restore CS0649
    }

    internal class InvalidLongArgumentWithDigit1
    {
#pragma warning disable CS0649  // This field is set via reflection, so the compiler doesn't know
        [Option("6502")]
        public bool Option6502;
#pragma warning restore CS0649
    }

    internal class InvalidLongArgumentWithDigit2
    {
#pragma warning disable CS0649  // This field is set via reflection, so the compiler doesn't know
        [Option("6502level")]
        public bool Option6502;
#pragma warning restore CS0649
    }

    internal class ShortOptionWithDigit
    {
#pragma warning disable CS0649  // This field is set via reflection, so the compiler doesn't know
        [Option('9')]
        public bool Level;
#pragma warning restore CS0649
    }

    internal class DefaultValueOption
    {
        [Option('a', "along", false)]
        public bool OptionA { get; set; }

        [Option('b', "blong", false)]
        public bool OptionB { get; set; }

#pragma warning disable CS0649  // This field is set via reflection, so the compiler doesn't know
        [Option('v', "verbosity")]
        [OptionDefault("0")]
        public string Verbosity;
#pragma warning restore CS0649
    }

    internal enum BasicColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        Cyan,
        Purple,
        Black,
        White
    }

    internal class TypesOptions
    {
        [Option('c', "color")]
        public BasicColor Color { get; private set; }

        [Option('O', "opacity")]
        public int Opacity { get; private set; }
    }

    internal class ArgumentsAttributeOptions
    {
#pragma warning disable CS0649  // This field is set via reflection, so the compiler doesn't know
        [Option('a', "along", false)]
        public bool OptionA;

        [Option('b', "blong", false)]
        public bool OptionB;

        [Option('c', "clong", false)]
        public string OptionC;
#pragma warning restore CS0649

        private List<string> m_Arguments = new List<string>();

        [OptionArguments]
        public IList<string> Arguments { get { return m_Arguments; } }
    }

    internal class BaseOptionsPrivate
    {
        [Option('a', "along")]
#pragma warning disable CS0649  // This field is set via reflection, so the compiler doesn't know
        private bool m_OptionA;
#pragma warning restore CS0649

        public bool OptionA { get { return m_OptionA; } }
    }

    internal class DerivedOptionsPrivate : BaseOptionsPrivate
    {
        [Option('b', "blong")]
#pragma warning disable CS0649  // This field is set via reflection, so the compiler doesn't know
        private bool m_OptionB;
#pragma warning restore CS0649

        public bool OptionB { get { return m_OptionB; } }
    }

    internal class BaseOptionsProtected
    {
        [Option('a', "along")]
#pragma warning disable CS0649  // This field is set via reflection, so the compiler doesn't know
        protected bool m_OptionA;
#pragma warning restore CS0649

        public bool OptionA { get { return m_OptionA; } }
    }

    internal class DerivedOptionsProtected : BaseOptionsProtected
    {
        [Option('b', "blong")]
#pragma warning disable CS0649  // This field is set via reflection, so the compiler doesn't know
        protected bool m_OptionB;
#pragma warning restore CS0649

        public bool OptionB { get { return m_OptionB; } }
    }

    internal class BaseOptionsProtectedList
    {
        [Option('a', "along")]
        protected List<string> m_OptionA = new List<string>();

        public IList<string> OptionA { get { return m_OptionA; } }
    }

    internal class DerivedOptionsProtectedList : BaseOptionsProtectedList
    {
        [Option('b', "blong")]
        protected List<string> m_OptionB = new List<string>();

        public IList<string> OptionB { get { return m_OptionB; } }
    }

    internal class EventWithOption
    {
        [Option('e', "event")]
#pragma warning disable CS0067  // This field is set via reflection, so the compiler doesn't know
        public event EventHandler<EventArgs> MyEvent;
#pragma warning restore CS0067
    }
}
