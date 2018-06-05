namespace RJCP.Core.CommandLine
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture(Category = "Utilities.CommandLine")]
    public class CommandLineUnixTest
    {
        private class NoArguments
        {
#pragma warning disable 0649
            // This field is set via reflection, so the compiler doesn't know
            public bool NoOption;
#pragma warning restore 0649
        }

        private class OptionalArguments
        {
#pragma warning disable 0649
            [Option('a', "along", false)]
            public bool OptionA;

            [Option('b', "blong", false)]
            public bool OptionB;

            [Option('c', "clong", false)]
            public string OptionC;
#pragma warning restore 0649
        }

        private class RequiredArguments
        {
#pragma warning disable 0649
            [Option('i', "insensitive")]
            private bool m_CaseInsensitive;

            [Option('f', "printfiles")]
            private bool m_PrintFiles;

            [Option('s', "search")]
            private string m_SearchString;
#pragma warning restore 0649

            public bool CaseInsensitive { get { return m_CaseInsensitive; } }

            public bool PrintFiles { get { return m_PrintFiles; } }

            public string SearchString { get { return m_SearchString; } }
        }

        private class PropertyOptions
        {
            [Option('a', "along", false)]
            public bool OptionA { get; set; }

            [Option('b', "blong", false)]
            public bool OptionB { get; set; }

            [Option('c', "clong", false)]
            public string OptionC { get; set; }
        }

        private class RequiredOptions
        {
            [Option('a', "along", false)]
            public bool OptionA { get; set; }

            [Option('b', "blong", false)]
            public bool OptionB { get; set; }

            [Option('c', "clong", true)]
            public string OptionC { get; set; }
        }

        private class ListOptions
        {
            public ListOptions()
            {
                List = new List<string>();
            }

            [Option('l', "list", false)]
            public List<string> List { get; private set; }
        }

        private class OptionalLongArguments
        {
#pragma warning disable 0649
            [Option("along")]
            public bool OptionA;

            [Option("blong")]
            public bool OptionB;

            [Option("clong")]
            public string OptionC;

            [Option("level42")]
            public string Level42;
#pragma warning restore 0649
        }

        private class InvalidLongArgumentWithDigit1
        {
#pragma warning disable 0649
            [Option("6502")]
            public bool Option6502;
#pragma warning restore 0649
        }

        private class InvalidLongArgumentWithDigit2
        {
#pragma warning disable 0649
            [Option("6502level")]
            public bool Option6502;
#pragma warning restore 0649
        }

        private class ShortOptionWithDigit
        {
#pragma warning disable 0649
            [Option('9')]
            public bool Level;
#pragma warning restore 0649
        }

        private class DefaultValueOption
        {
            [Option('a', "along", false)]
            public bool OptionA { get; set; }

            [Option('b', "blong", false)]
            public bool OptionB { get; set; }

#pragma warning disable 0649
            [Option('v', "verbosity")]
            [OptionDefault("0")]
            public string Verbosity;
#pragma warning restore 0649
        }

        private enum BasicColor
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

        private class TypesOptions
        {
            [Option('c', "color")]
            public BasicColor Color { get; private set; }

            [Option('O', "opacity")]
            public int Opacity { get; private set; }
        }

        private class ArgumentsAttributeOptions
        {
#pragma warning disable 0649
            [Option('a', "along", false)]
            public bool OptionA;

            [Option('b', "blong", false)]
            public bool OptionB;

            [Option('c', "clong", false)]
            public string OptionC;
#pragma warning restore 0649

            private List<string> m_Arguments = new List<string>();

            [OptionArguments]
            public IList<string> Arguments { get { return m_Arguments; } }
        }

        [Test]
        [Platform(Include = "Unix")]
        public void CmdLineUnix_OptionsType()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, null);
            Assert.That(options.OptionsStyle, Is.EqualTo(OptionsStyle.Unix));
        }

        [Test]
        public void CmdLineUnix_NullOptions()
        {
            Assert.That(() => { Options.Parse(null, null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void CmdLineUnix_NullArguments()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, null);
        }

        [Test]
        public void CmdLineUnix_NoArguments()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new string[0], OptionsStyle.Unix);
        }

        [Test]
        public void CmdLineUnix_NullArgumentsEmptyOptions()
        {
            NoArguments myOptions = new NoArguments();
            Options options = Options.Parse(myOptions, null, OptionsStyle.Unix);
        }

        [Test]
        public void CmdLineUnix_NoArgumentsEmptyOptions()
        {
            NoArguments myOptions = new NoArguments();
            Options options = Options.Parse(myOptions, new string[0], OptionsStyle.Unix);
        }

        [Test]
        public void CmdLineUnix_SingleShortOption()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-a" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.Null);
        }

        [Test]
        public void CmdLineUnix_ShortOptionsJoined()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-ab" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.Null);
        }

        [Test]
        public void CmdLineUnix_ShortOptionsSeparated()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-a", "-b" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.Null);
        }

        [Test]
        public void CmdLineUnix_ShortOptionsOnProperties()
        {
            PropertyOptions myOptions = new PropertyOptions();
            Options options = Options.Parse(myOptions, new[] { "-a", "-b" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.Null);
        }

        [Test]
        public void CmdLineUnix_ShortOptionsStringOneArg()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-cfoo" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
        }

        [Test]
        public void CmdLineUnix_ShortOptionsStringTwoArgs()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-c", "foo" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
        }

        [Test]
        public void CmdLineUnix_ShortOptionsStringOneArgAssigned()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-c=foo" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
        }

        [Test]
        public void CmdLineUnix_ShortOptionsStringOneArgJoined()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-abcfoo" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
        }

        [Test]
        public void CmdLineUnix_ShortOptionsStringTwoArgsJoined1()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-abc", "foo" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
        }

        [Test]
        public void CmdLineUnix_ShortOptionsStringTwoArgsJoined2()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-cab", "foo" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.EqualTo("ab"));
            Assert.That(options.Arguments.Count, Is.EqualTo(1));
            Assert.That(options.Arguments[0], Is.EqualTo("foo"));
        }

        [Test]
        public void CmdLineUnix_ShortOptionsRequired()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "-ifs", "string" }, OptionsStyle.Unix);

            Assert.That(myOptions.CaseInsensitive, Is.True);
            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
        }

        [Test]
        public void CmdLineUnix_ShortOptionsRequiredOneArgument1()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "-ifsstring" }, OptionsStyle.Unix);

            Assert.That(myOptions.CaseInsensitive, Is.True);
            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
        }

        [Test]
        public void CmdLineUnix_ShortOptionsRequiredOneArgument2()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "-if", "-sstring" }, OptionsStyle.Unix);

            Assert.That(myOptions.CaseInsensitive, Is.True);
            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
        }

        [Test]
        public void CmdLineUnix_ShortOptionsRequiredOneArgument3()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "-if", "-s=string" }, OptionsStyle.Unix);

            Assert.That(myOptions.CaseInsensitive, Is.True);
            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
        }

        [Test]
        public void CmdLineUnix_ShortOptionsRequiredMissingArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "-ifs" }, OptionsStyle.Unix); }, Throws.TypeOf<OptionMissingArgumentException>());
        }

        [Test]
        public void CmdLineUnix_MissingOption()
        {
            RequiredOptions myOptions = new RequiredOptions();
            Assert.That(() => { Options.Parse(myOptions, new[] { "-ab" }, OptionsStyle.Unix); }, Throws.TypeOf<OptionMissingException>());
        }

        [Test]
        public void CmdLineUnix_RequiredOption()
        {
            RequiredOptions myOptions = new RequiredOptions();
            Options options = Options.Parse(myOptions, new[] { "-c=bar" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionC, Is.EqualTo("bar"));
        }

        [Test]
        public void CmdLineUnix_LongOptionOneArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "--search=string" }, OptionsStyle.Unix);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
        }

        [Test]
        public void CmdLineUnix_LongOptionTwoArguments()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "--search", "string" }, OptionsStyle.Unix);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
        }

        [Test]
        public void CmdLineUnix_LongOptionWithSpace()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "--search= string " }, OptionsStyle.Unix);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo(" string "));
        }

        [Test]
        public void CmdLineUnix_LongOptionWithSpace2()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "--search= string" }, OptionsStyle.Unix);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo(" string"));
        }

        [Test]
        public void CmdLineUnix_LongOptionWithSpace3()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "--search=string " }, OptionsStyle.Unix);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("string "));
        }

        [Test]
        public void CmdLineUnix_LongOptionWithSpace4()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "--search", " string " }, OptionsStyle.Unix);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo(" string "));
        }

        [Test]
        public void CmdLineUnix_LongOptionWithSpace5()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "--search", "=", "string" }, OptionsStyle.Unix);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("="));
        }

        [Test]
        public void CmdLineUnix_LongOptionWithSpace6()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "--search", "=", " string " }, OptionsStyle.Unix);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("="));
        }

        [Test]
        public void CmdLineUnix_LongOptionsRequiredMissingArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "--search" }, OptionsStyle.Unix); }, Throws.TypeOf<OptionMissingArgumentException>());
        }

        [Test]
        public void CmdLineUnix_LongOptionsBoolean()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "--printfiles" }, OptionsStyle.Unix);

            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.SearchString, Is.Null);
        }

        [Test]
        public void cmdLineUnix_LongOptionOnly()
        {
            OptionalLongArguments myOptions = new OptionalLongArguments();
            Options options = Options.Parse(myOptions, new[] { "--along" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.Null);
        }

        [Test]
        public void cmdLineUnix_LongOptionOnlyShort()
        {
            OptionalLongArguments myOptions = new OptionalLongArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "-a" }, OptionsStyle.Unix); }, Throws.TypeOf<OptionUnknownException>());
        }

        [Test]
        public void CmdLineUnix_UnknownOption1()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "--search", "string", "-ab", "-c" }, OptionsStyle.Unix); }, Throws.TypeOf<OptionUnknownException>());
        }

        [Test]
        public void CmdLineUnix_UnknownOption2()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "-dabc" }, OptionsStyle.Unix); }, Throws.TypeOf<OptionUnknownException>());
        }

        [Test]
        public void CmdLineUnix_UnknownLongOption()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "--foobar" }, OptionsStyle.Unix); }, Throws.TypeOf<OptionUnknownException>());
        }

        [Test]
        public void CmdLineUnix_InvalidLongOption1a()
        {
            InvalidLongArgumentWithDigit1 myOptions = new InvalidLongArgumentWithDigit1();
            Assert.That(() => { Options.Parse(myOptions, new string[] { }, OptionsStyle.Unix); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CmdLineUnix_InvalidLongOption1b()
        {
            InvalidLongArgumentWithDigit1 myOptions = new InvalidLongArgumentWithDigit1();
            Assert.That(() => { Options.Parse(myOptions, new string[] { "--6502" }, OptionsStyle.Unix); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CmdLineUnix_InvalidLongOption2a()
        {
            InvalidLongArgumentWithDigit2 myOptions = new InvalidLongArgumentWithDigit2();
            Assert.That(() => { Options.Parse(myOptions, new string[] { }, OptionsStyle.Unix); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CmdLineUnix_InvalidLongOption2b()
        {
            InvalidLongArgumentWithDigit2 myOptions = new InvalidLongArgumentWithDigit2();
            Assert.That(() => { Options.Parse(myOptions, new string[] { "--6502level" }, OptionsStyle.Unix); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CmdLineUnix_ShortOptionWithDigit()
        {
            ShortOptionWithDigit myOptions = new ShortOptionWithDigit();
            Options options = Options.Parse(myOptions, new string[] { "-9" }, OptionsStyle.Unix);
            Assert.That(myOptions.Level, Is.True);
        }

        [Test]
        public void CmdLineUnix_ExtraArgument()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "-ab", "argument", "-c" }, OptionsStyle.Unix); }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void CmdLineUnix_ListOption()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "-l", "test1,test2,test3" }, OptionsStyle.Unix);

            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo("test3"));
        }

        [Test]
        public void CmdLineUnix_ListOptionSingleElement()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "-l", "test1" }, OptionsStyle.Unix);

            Assert.That(myOptions.List.Count, Is.EqualTo(1));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
        }

        [Test]
        public void CmdLineUnix_ListOptionMultipleArguments()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "-l", "test1", "-l", "test2", "-l", "test3" }, OptionsStyle.Unix);

            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo("test3"));
        }

        [Test]
        public void CmdLineUnix_ListOptionQuoted1()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "-l", @"test1,'test2','test 3'" }, OptionsStyle.Unix);

            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo("test 3"));
        }

        [Test]
        public void CmdLineUnix_ListOptionQuoted2()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "-l", @"test1,'test2,test3b','test 3'" }, OptionsStyle.Unix);

            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2,test3b"));
            Assert.That(myOptions.List[2], Is.EqualTo("test 3"));
        }

        [Test]
        public void CmdLineUnix_ListOptionQuotedInvalid1()
        {
            ListOptions myOptions = new ListOptions();
            Assert.That(() => { Options.Parse(myOptions, new[] { "-l", @"test1,'test2,test3b','test 3'x" }, OptionsStyle.Unix); }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void CmdLineUnix_ListOptionWindowsPath()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "-l", @"c:\users\homeuser" }, OptionsStyle.Unix);
            Assert.That(myOptions.List.Count, Is.EqualTo(1));
            Assert.That(myOptions.List[0], Is.EqualTo(@"c:\users\homeuser"));
        }

        [Test]
        public void CmdLineUnix_ListOptionQuotedEndEscape()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "-l", @"test1,test2,testx\" }, OptionsStyle.Unix);
            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo(@"testx\"));
        }

        [Test]
        public void CmdLineUnix_ListOptionQuotedInvalid3()
        {
            ListOptions myOptions = new ListOptions();
            Assert.That(() => { Options.Parse(myOptions, new[] { "-l", "test1,test2,\"test3" }, OptionsStyle.Unix); }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void CmdLineUnix_StopParsing()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-abc", "argument", "--", "-c" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.EqualTo("argument"));
            Assert.That(options.Arguments.Count, Is.EqualTo(1));
            Assert.That(options.Arguments[0], Is.EqualTo("-c"));
        }

        [Test]
        public void CmdLineUnix_DefaultValueProvidedShort()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "-abv3" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("3"));
        }

        [Test]
        public void CmdLineUnix_DefaultValueProvidedShort2()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "-abv", "3" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("3"));
        }

        [Test]
        public void CmdLineUnix_DefaultValueShort()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "-abv" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("0"));
        }

        [Test]
        public void CmdLineUnix_DefaultValueShortProvidedExistingOption()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "-avb" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.Verbosity, Is.EqualTo("b"));
        }

        [Test]
        public void CmdLineUnix_DefaultValueProvidedLongEquals()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "-ab", "--verbosity=2" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("2"));
        }

        [Test]
        public void CmdLineUnix_DefaultValueProvidedLong()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "-ab", "--verbosity", "2" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("2"));
        }

        [Test]
        public void CmdLineUnix_DefaultValueLong()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "-ab", "--verbosity" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("0"));
        }

        [Test]
        public void CmdLineUnix_DefaultValueLong2()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "--verbosity", "-ab" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("0"));
        }

        [Test]
        public void CmdLineUnix_TypesEnum()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = Options.Parse(myOptions, new[] { "-c", "yellow" }, OptionsStyle.Unix);

            Assert.That(myOptions.Color, Is.EqualTo(BasicColor.Yellow));
        }

        [Test]
        public void CmdLineUnix_TypesEnumInt()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = Options.Parse(myOptions, new[] { "-c", "4" }, OptionsStyle.Unix);

            Assert.That(myOptions.Color, Is.EqualTo(BasicColor.Cyan));
        }

        [Test]
        public void CmdLineUnix_TypesInt()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = Options.Parse(myOptions, new[] { "-O", "100" }, OptionsStyle.Unix);

            Assert.That(myOptions.Opacity, Is.EqualTo(100));
        }

        [Test]
        public void CmdLineUnix_TypesIntInvalid()
        {
            TypesOptions myOptions = new TypesOptions();
            Assert.That(() => { Options.Parse(myOptions, new[] { "-O", "xxx" }, OptionsStyle.Unix); }, Throws.TypeOf<OptionFormatException>());
        }

        [Test]
        public void CmdLineUnix_ArgumentsAttribute()
        {
            ArgumentsAttributeOptions myOptions = new ArgumentsAttributeOptions();
            Options options = Options.Parse(myOptions, new[] { "-a", "arg1", "arg2" }, OptionsStyle.Unix);

            Assert.That(options.Arguments.Count, Is.EqualTo(2));
            Assert.That(myOptions.Arguments.Count, Is.EqualTo(2));
            Assert.That(myOptions.Arguments[0], Is.EqualTo("arg1"));
            Assert.That(myOptions.Arguments[1], Is.EqualTo("arg2"));
        }
    }
}
