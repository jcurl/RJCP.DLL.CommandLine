namespace RJCP.Core.CommandLine
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture(Category = "Utilities.CommandLine")]
    public class CommandLineWinTest
    {
        private class NoArguments
        {
#pragma warning disable 0649
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

        [Test]
        [Platform(Include = "Win")]
        public void CmdLineWin_OptionsType()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, null);
            Assert.That(options.OptionsStyle, Is.EqualTo(OptionsStyle.Windows));
        }

        [Test]
        public void CmdLineWin_NullArguments()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, null, OptionsStyle.Windows);
        }

        [Test]
        public void CmdLineWin_NoArguments()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new string[0], OptionsStyle.Windows);
        }

        [Test]
        public void CmdLineWin_NullArgumentsEmptyOptions()
        {
            NoArguments myOptions = new NoArguments();
            Options options = Options.Parse(myOptions, null, OptionsStyle.Windows);
        }

        [Test]
        public void CmdLineWin_NoArgumentsEmptyOptions()
        {
            NoArguments myOptions = new NoArguments();
            Options options = Options.Parse(myOptions, new string[0], OptionsStyle.Windows);
        }

        [Test]
        public void CmdLineWin_SingleShortOption()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "/a" }, OptionsStyle.Windows);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.Null);
        }

        [Test]
        public void CmdLineWin_ShortOptionsJoined()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "/ab" }, OptionsStyle.Windows); }, Throws.TypeOf<OptionUnknownException>());
        }

        [Test]
        public void CmdLineWin_ShortOptionsSeparated()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b" }, OptionsStyle.Windows);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.Null);
        }

        [Test]
        public void CmdLineWin_ShortOptionsOnProperties()
        {
            PropertyOptions myOptions = new PropertyOptions();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b" }, OptionsStyle.Windows);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.Null);
        }

        [Test]
        public void CmdLineWin_ShortOptionsStringOneArg()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "/c:foo" }, OptionsStyle.Windows);

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
        }

        [Test]
        public void CmdLineWin_ShortOptionsStringTwoArgs()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "/c", "foo" }, OptionsStyle.Windows);

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
        }

        [Test]
        public void CmdLineWin_ShortOptionsRequired()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/i", "/f", "/s", "string" }, OptionsStyle.Windows);

            Assert.That(myOptions.CaseInsensitive, Is.True);
            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
        }

        [Test]
        public void CmdLineWin_ShortOptionsRequiredOneArgument2()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/i", "/f", "/s:string" }, OptionsStyle.Windows);

            Assert.That(myOptions.CaseInsensitive, Is.True);
            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
        }

        [Test]
        public void CmdLineWin_ShortOptionsRequiredMissingArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "/i", "/f", "/s" }, OptionsStyle.Windows); }, Throws.TypeOf<OptionMissingArgumentException>());

        }

        [Test]
        public void CmdLineWin_MissingOption()
        {
            RequiredOptions myOptions = new RequiredOptions();
            Assert.That(() => { Options.Parse(myOptions, new[] { "/a", "/b" }, OptionsStyle.Windows); }, Throws.TypeOf<OptionMissingException>());
        }

        [Test]
        public void CmdLineWin_RequiredOption()
        {
            RequiredOptions myOptions = new RequiredOptions();
            Options options = Options.Parse(myOptions, new[] { "/c:bar" }, OptionsStyle.Windows);

            Assert.That(myOptions.OptionC, Is.EqualTo("bar"));
        }

        [Test]
        public void CmdLineWin_LongOptionOneArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search:string" }, OptionsStyle.Windows);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
        }

        [Test]
        public void CmdLineWin_LongOptionTwoArguments()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search", "string" }, OptionsStyle.Windows);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search: string " }, OptionsStyle.Windows);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo(" string "));
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace2()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search: string" }, OptionsStyle.Windows);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo(" string"));
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace3()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search:string " }, OptionsStyle.Windows);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("string "));
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace4()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search", " string " }, OptionsStyle.Windows);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo(" string "));
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace5()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search", ":", "string" }, OptionsStyle.Windows);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo(":"));
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace6()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search", ":", " string " }, OptionsStyle.Windows);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo(":"));
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace7()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search", "=", "string" }, OptionsStyle.Windows);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("="));
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace8()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search", "=", " string " }, OptionsStyle.Windows);

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("="));
        }

        [Test]
        public void CmdLineWin_LongOptionsRequiredMissingArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "/search" }, OptionsStyle.Windows); }, Throws.TypeOf<OptionMissingArgumentException>());
        }

        [Test]
        public void CmdLineWin_LongOptionsBoolean()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/printfiles" }, OptionsStyle.Windows);

            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.SearchString, Is.Null);
        }

        [Test]
        public void CmdLineWin_LongOptionOnly()
        {
            OptionalLongArguments myOptions = new OptionalLongArguments();
            Options options = Options.Parse(myOptions, new[] { "/along" }, OptionsStyle.Windows);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.Null);
        }

        [Test]
        public void CmdLineWin_LongOptionOnlyShort()
        {
            OptionalLongArguments myOptions = new OptionalLongArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "/a" }, OptionsStyle.Windows); }, Throws.TypeOf<OptionUnknownException>());
        }

        [Test]
        public void CmdLineWin_UnknownOption1()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "/search", "string", "/a", "/b", "/c" }, OptionsStyle.Windows); }, Throws.TypeOf<OptionUnknownException>());
        }

        [Test]
        public void CmdLineWin_UnknownOption2()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "/d", "/a", "/b", "/c" }, OptionsStyle.Windows); }, Throws.TypeOf<OptionUnknownException>());
        }

        [Test]
        public void CmdLineWin_UnknownLongOption()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "/foobar" }, OptionsStyle.Windows); }, Throws.TypeOf<OptionUnknownException>());
        }

        [Test]
        public void CmdLineWin_ExtraArgument()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Assert.That(() => { Options.Parse(myOptions, new[] { "/a", "/b", "argument", "/c" }, OptionsStyle.Windows); }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void CmdLineWin_ListOption()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", "test1,test2,test3" }, OptionsStyle.Windows);

            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo("test3"));
        }

        [Test]
        public void CmdLineWin_ListOptionSingleElement()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", "test1" }, OptionsStyle.Windows);

            Assert.That(myOptions.List.Count, Is.EqualTo(1));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
        }

        [Test]
        public void CmdLineWin_ListOptionMultipleArguments()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", "test1", "/l", "test2", "/l", "test3" }, OptionsStyle.Windows);

            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo("test3"));
        }

        [Test]
        public void CmdLineWin_ListOptionQuoted1()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", @"test1,'test2','test 3'" }, OptionsStyle.Windows);

            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo("test 3"));
        }

        [Test]
        public void CmdLineWin_ListOptionQuoted2()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", @"test1,'test2,test3b','test 3'" }, OptionsStyle.Windows);

            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2,test3b"));
            Assert.That(myOptions.List[2], Is.EqualTo("test 3"));
        }

        [Test]
        public void CmdLineWin_ListOptionQuotedInvalid1()
        {
            ListOptions myOptions = new ListOptions();
            Assert.That(() => { Options.Parse(myOptions, new[] { "/l", @"test1:'test2:test3b':'test 3'x" }, OptionsStyle.Windows); }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void CmdLineWin_ListOptionWindowsPath()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", @"c:\users\homeuser" }, OptionsStyle.Windows);
            Assert.That(myOptions.List.Count, Is.EqualTo(1));
            Assert.That(myOptions.List[0], Is.EqualTo(@"c:\users\homeuser"));
        }

        [Test]
        public void CmdLineWin_ListOptionQuotedEndEscape()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", @"test1,test2,testx\" }, OptionsStyle.Windows);
            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo(@"testx\"));
        }

        [Test]
        public void CmdLineWin_ListOptionQuotedInvalid3()
        {
            ListOptions myOptions = new ListOptions();
            Assert.That(() => { Options.Parse(myOptions, new[] { "/l", "test1:test2:\"test3" }, OptionsStyle.Windows); }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void CmdLineWin_DefaultValueProvidedShort()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b", "/v:3" }, OptionsStyle.Windows);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("3"));
        }

        [Test]
        public void CmdLineWin_DefaultValueProvidedShort2()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b", "/v", "3" }, OptionsStyle.Windows);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("3"));
        }

        [Test]
        public void CmdLineWin_DefaultValueShort()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b", "/v" }, OptionsStyle.Windows);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("0"));
        }

        [Test]
        public void CmdLineWin_DefaultValueProvidedLongEquals()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b", "/verbosity:2" }, OptionsStyle.Windows);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("2"));
        }

        [Test]
        public void CmdLineWin_DefaultValueProvidedLong()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b", "/verbosity", "2" }, OptionsStyle.Windows);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("2"));
        }

        [Test]
        public void CmdLineWin_DefaultValueLong()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b", "/verbosity" }, OptionsStyle.Windows);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("0"));
        }

        [Test]
        public void CmdLineWin_DefaultValueLong2()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "/verbosity", "/a", "/b" }, OptionsStyle.Windows);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("0"));
        }

        [Test]
        public void CmdLineWin_TypesEnum()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = Options.Parse(myOptions, new[] { "/c", "yellow" }, OptionsStyle.Windows);

            Assert.That(myOptions.Color, Is.EqualTo(BasicColor.Yellow));
        }

        [Test]
        public void CmdLineWin_TypesEnumInt()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = Options.Parse(myOptions, new[] { "/c", "4" }, OptionsStyle.Windows);

            Assert.That(myOptions.Color, Is.EqualTo(BasicColor.Cyan));
        }

        [Test]
        public void CmdLineWin_TypesInt()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = Options.Parse(myOptions, new[] { "/O", "100" }, OptionsStyle.Windows);

            Assert.That(myOptions.Opacity, Is.EqualTo(100));
        }

        [Test]
        public void CmdLineWin_TypesIntInvalid()
        {
            TypesOptions myOptions = new TypesOptions();
            Assert.That(() => { Options.Parse(myOptions, new[] { "/O", "xxx" }, OptionsStyle.Windows); }, Throws.TypeOf<OptionFormatException>());
        }
    }
}
