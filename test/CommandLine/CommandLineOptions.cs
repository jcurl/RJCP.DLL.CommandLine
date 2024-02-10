#pragma warning disable CS0649  // This field is set via reflection, so the compiler doesn't know

namespace RJCP.Core.CommandLine
{
    using System.Collections;
    using System.Collections.Generic;

    internal class NoArguments
    {
        public bool NoOption;
    }

    internal class OptionalArguments
    {
        [Option('a', "along", false)]
        public bool OptionA;

        [Option('b', "blong", false)]
        public bool OptionB;

        [Option('c', "clong", false)]
        public string OptionC;
    }

    internal class RequiredArguments
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Set on reflection")]
        [Option('i', "insensitive")]
        private bool m_CaseInsensitive;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Set on reflection")]
        [Option('f', "printfiles")]
        private bool m_PrintFiles;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Set on reflection")]
        [Option('s', "search")]
        private string m_SearchString;

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

    internal class ListOptionsInterfaceGeneric
    {
        [Option('l', "list", false)]
        public IList<string> List { get; } = new List<string>();
    }

    internal class ListOptionsInterface
    {
        [Option('l', "list", false)]
        public IList List { get; } = new List<string>();
    }

    internal class CollectionOptionsInterfaceGeneric
    {
        [Option('l', "list", false)]
        public ICollection<string> List { get; } = new List<string>();
    }

    internal class CollectionOptionsInterface
    {
        [Option('l', "list", false)]
        public ICollection List { get; } = new List<string>();
    }

    internal class ListOptionsIntegers
    {
        [Option('l', "list", false)]
        public IList<int> List { get; } = new List<int>();
    }

    internal class OptionalLongArguments
    {
        [Option("along")]
        public bool OptionA;

        [Option("blong")]
        public bool OptionB;

        [Option("clong")]
        public string OptionC;

        [Option("level42")]
        public string Level42;
    }

    internal class InvalidLongArgumentWithDigit1
    {
        [Option("6502")]
        public bool Option6502;
    }

    internal class InvalidLongArgumentWithDigit2
    {
        [Option("6502level")]
        public bool Option6502;
    }

    internal class ShortOptionWithDigit
    {
        [Option('9')]
        public bool Level;
    }

    internal class DefaultValueOption
    {
        [Option('a', "along", false)]
        public bool OptionA { get; set; }

        [Option('b', "blong", false)]
        public bool OptionB { get; set; }

        [Option('v', "verbosity")]
        [OptionDefault("0")]
        public string Verbosity;
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
        [Option('a', "along", false)]
        public bool OptionA;

        [Option('b', "blong", false)]
        public bool OptionB;

        [Option('c', "clong", false)]
        public string OptionC;

        private readonly List<string> m_Arguments = new List<string>();

        [OptionArguments]
        public IList<string> Arguments { get { return m_Arguments; } }
    }

    internal class ArgumentsListGenericStringAttributeOptions
    {
        [OptionArguments]
        private readonly List<string> m_Arguments = new List<string>();

        public IList<string> Arguments { get { return m_Arguments; } }
    }

    internal class ArgumentsListCollGenericStringAttributeOptions
    {
        private readonly List<string> m_Arguments = new List<string>();

        [OptionArguments]
        public ICollection<string> Arguments { get { return m_Arguments; } }
    }

    internal class ArgumentsListCollGenericIntAttributeOptions
    {
        private readonly List<int> m_Arguments = new List<int>();

        [OptionArguments]
        public ICollection<int> Arguments { get { return m_Arguments; } }
    }

    internal class ArgumentsListAttributeOptions
    {
        [OptionArguments]
        private readonly ArrayList m_Arguments = new ArrayList();

        public IList Arguments { get { return m_Arguments; } }
    }

    internal class ArgumentsListCollAttributeOptions
    {
        private readonly ArrayList m_Arguments = new ArrayList();

        [OptionArguments]
        public ICollection Arguments { get { return m_Arguments; } }
    }

    internal class BaseOptionsPrivate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Set on reflection")]
        [Option('a', "along")]
        private bool m_OptionA;

        public bool OptionA { get { return m_OptionA; } }
    }

    internal class DerivedOptionsPrivate : BaseOptionsPrivate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Set on reflection")]
        [Option('b', "blong")]
        private bool m_OptionB;

        public bool OptionB { get { return m_OptionB; } }
    }

    internal class BaseOptionsProtected
    {
        [Option('a', "along")]
        protected bool m_OptionA;

        public bool OptionA { get { return m_OptionA; } }
    }

    internal class DerivedOptionsProtected : BaseOptionsProtected
    {
        [Option('b', "blong")]
        protected bool m_OptionB;

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

    internal class DuplicateOptionsShort
    {
        [Option('a')]
        public bool OptionA { get; set; }

        [Option('a')]
        public bool OptionADup { get; set; }
    }

    internal class DuplicateOptionsLong
    {
        [Option("along")]
        public bool OptionA { get; set; }

        [Option("along")]
        public bool OptionADup { get; set; }
    }

    internal class OptionHandling : IOptions
    {
        [Option('a', "along", false)]
        public bool OptionA;

        [Option('b', "blong", false)]
        public bool OptionB;

        [Option('c', "clong", false)]
        public string OptionC;

        public void Check() { /* Nothing to check */ }

        public IList<string> InvalidOptions { get; private set; } = new List<string>();

        public void InvalidOption(string option)
        {
            InvalidOptions.Add(option);
        }

        public IList<string> MissingOptions { get; private set; } = new List<string>();

        public void Missing(IList<string> missingOptions)
        {
            foreach (string missing in missingOptions) {
                MissingOptions.Add(missing);
            }
        }

        public void Usage() { /* Nothing to show */ }
    }

    internal class OptionShortSymbol
    {
        [Option('#', "hash")]
        public bool Hash;

        [Option('!', "bang")]
        public bool Bang;

        [Option('?', "help")]
        public bool Help;
    }

    internal class OptionShortPlus
    {
        [Option('+', "plus")]
        public bool Plus;
    }

    internal class OptionShortMinus
    {
        [Option('-', "minus")]
        public bool Minus;
    }

    internal class OptionShortUnder
    {
        [Option('_', "under")]
        public bool Under;
    }

    internal class OptionShortHash
    {
        [Option('#', "hash")]
        public bool Hash;
    }

    internal class OptionShortBang
    {
        [Option('!', "bang")]
        public bool Bang;
    }

    internal class OptionShortHelp
    {
        [Option('?', "help")]
        public bool Help;
    }

    internal class OptionShortStar
    {
        [Option('*', "star")]
        public bool Star;
    }

    internal class OptionPropertySetRaiseError
    {
        private int m_Value;

        [Option('v', "value")]
        public int Value
        {
            get { return m_Value; }
            set
            {
                if (value < 0)
                    throw new OptionException("Value out of range");
                m_Value = value;
            }
        }
    }

    internal class OptionOnlyGetterClass
    {
        [Option('x', "extended")]
        public bool Extended { get; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0250:Make struct 'readonly'", Justification = "Test Case")]
    internal struct OptionOnlyGetterStruct
    {
        [Option('x', "extended")]
        public bool Extended { get; }
    }

    internal readonly struct OptionOnlyReadOnlyStruct
    {
        [Option('x', "extended")]
        public readonly bool Extended;
    }
}
