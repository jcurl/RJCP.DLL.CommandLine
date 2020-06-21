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

            public string SearchString { get { return m_SearchString;  } }
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

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.IsNull(myOptions.OptionC);
        }

        [Test]
        [ExpectedException(typeof(OptionUnknownException))]
        public void CmdLineWin_ShortOptionsJoined()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "/ab" }, OptionsStyle.Windows);
        }

        [Test]
        public void CmdLineWin_ShortOptionsSeparated()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b" }, OptionsStyle.Windows);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.IsNull(myOptions.OptionC);
        }

        [Test]
        public void CmdLineWin_ShortOptionsOnProperties()
        {
            PropertyOptions myOptions = new PropertyOptions();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b" }, OptionsStyle.Windows);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.IsNull(myOptions.OptionC);
        }

        [Test]
        public void CmdLineWin_ShortOptionsStringOneArg()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "/c:foo" }, OptionsStyle.Windows);

            Assert.IsFalse(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.AreEqual("foo", myOptions.OptionC);
        }

        [Test]
        public void CmdLineWin_ShortOptionsStringTwoArgs()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "/c", "foo" }, OptionsStyle.Windows);

            Assert.IsFalse(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.AreEqual("foo", myOptions.OptionC);
        }

        [Test]
        public void CmdLineWin_ShortOptionsRequired()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/i", "/f", "/s", "string" }, OptionsStyle.Windows);

            Assert.IsTrue(myOptions.CaseInsensitive);
            Assert.IsTrue(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [Test]
        public void CmdLineWin_ShortOptionsRequiredOneArgument2()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/i", "/f", "/s:string" }, OptionsStyle.Windows);

            Assert.IsTrue(myOptions.CaseInsensitive);
            Assert.IsTrue(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [Test]
        [ExpectedException(typeof(OptionMissingArgumentException))]
        public void CmdLineWin_ShortOptionsRequiredMissingArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/i", "/f", "/s" }, OptionsStyle.Windows);

            Assert.Fail("Exception not thrown");
        }

        [Test]
        [ExpectedException(typeof (OptionMissingException))]
        public void CmdLineWin_MissingOption()
        {
            RequiredOptions myOptions = new RequiredOptions();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b" }, OptionsStyle.Windows);

            Assert.Fail("Exception not thrown");
        }

        [Test]
        public void CmdLineWin_RequiredOption()
        {
            RequiredOptions myOptions = new RequiredOptions();
            Options options = Options.Parse(myOptions, new[] { "/c:bar" }, OptionsStyle.Windows);

            Assert.AreEqual("bar", myOptions.OptionC);
        }

        [Test]
        public void CmdLineWin_LongOptionOneArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search:string" }, OptionsStyle.Windows);

            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsFalse(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [Test]
        public void CmdLineWin_LongOptionTwoArguments()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search", "string" }, OptionsStyle.Windows);

            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsFalse(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search: string " }, OptionsStyle.Windows);

            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsFalse(myOptions.PrintFiles);
            Assert.AreEqual(" string ", myOptions.SearchString);
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace2()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search: string" }, OptionsStyle.Windows);

            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsFalse(myOptions.PrintFiles);
            Assert.AreEqual(" string", myOptions.SearchString);
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace3()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search:string " }, OptionsStyle.Windows);

            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsFalse(myOptions.PrintFiles);
            Assert.AreEqual("string ", myOptions.SearchString);
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace4()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search", " string " }, OptionsStyle.Windows);

            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsFalse(myOptions.PrintFiles);
            Assert.AreEqual(" string ", myOptions.SearchString);
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace5()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search", ":", "string" }, OptionsStyle.Windows);

            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsFalse(myOptions.PrintFiles);
            Assert.AreEqual(":", myOptions.SearchString);
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace6()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search", ":", " string " }, OptionsStyle.Windows);

            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsFalse(myOptions.PrintFiles);
            Assert.AreEqual(":", myOptions.SearchString);
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace7()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search", "=", "string" }, OptionsStyle.Windows);

            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsFalse(myOptions.PrintFiles);
            Assert.AreEqual("=", myOptions.SearchString);
        }

        [Test]
        public void CmdLineWin_LongOptionWithSpace8()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search", "=", " string " }, OptionsStyle.Windows);

            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsFalse(myOptions.PrintFiles);
            Assert.AreEqual("=", myOptions.SearchString);
        }

        [Test]
        [ExpectedException(typeof(OptionMissingArgumentException))]
        public void CmdLineWin_LongOptionsRequiredMissingArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search" }, OptionsStyle.Windows);

            Assert.Fail("Exception not thrown");
        }

        [Test]
        public void CmdLineWin_LongOptionsBoolean()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/printfiles" }, OptionsStyle.Windows);

            Assert.IsTrue(myOptions.PrintFiles);
            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsNull(myOptions.SearchString);
        }

        [Test]
        public void CmdLineWin_LongOptionOnly()
        {
            OptionalLongArguments myOptions = new OptionalLongArguments();
            Options options = Options.Parse(myOptions, new[] { "/along" }, OptionsStyle.Windows);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.IsNull(myOptions.OptionC);
        }

        [Test]
        [ExpectedException(typeof(OptionUnknownException))]
        public void CmdLineWin_LongOptionOnlyShort()
        {
            OptionalLongArguments myOptions = new OptionalLongArguments();
            Options options = Options.Parse(myOptions, new[] { "/a" }, OptionsStyle.Windows);
        }

        [Test]
        [ExpectedException(typeof(OptionUnknownException))]
        public void CmdLineWin_UnknownOption1()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "/search", "string", "/a", "/b", "/c" }, OptionsStyle.Windows);
        }

        [Test]
        [ExpectedException(typeof(OptionUnknownException))]
        public void CmdLineWin_UnknownOption2()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "/d", "/a", "/b", "/c" }, OptionsStyle.Windows);
        }

        [Test]
        [ExpectedException(typeof(OptionUnknownException))]
        public void CmdLineWin_UnknownLongOption()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "/foobar" }, OptionsStyle.Windows);
        }

        [Test]
        [ExpectedException(typeof(OptionException))]
        public void CmdLineWin_ExtraArgument()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b", "argument", "/c" }, OptionsStyle.Windows);
        }

        [Test]
        public void CmdLineWin_ListOption()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", "test1,test2,test3" }, OptionsStyle.Windows);

            Assert.AreEqual(3, myOptions.List.Count);
            Assert.AreEqual("test1", myOptions.List[0]);
            Assert.AreEqual("test2", myOptions.List[1]);
            Assert.AreEqual("test3", myOptions.List[2]);
        }

        [Test]
        public void CmdLineWin_ListOptionSingleElement()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", "test1" }, OptionsStyle.Windows);

            Assert.AreEqual(1, myOptions.List.Count);
            Assert.AreEqual("test1", myOptions.List[0]);
        }

        [Test]
        public void CmdLineWin_ListOptionMultipleArguments()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", "test1", "/l", "test2", "/l", "test3" }, OptionsStyle.Windows);

            Assert.AreEqual(3, myOptions.List.Count);
            Assert.AreEqual("test1", myOptions.List[0]);
            Assert.AreEqual("test2", myOptions.List[1]);
            Assert.AreEqual("test3", myOptions.List[2]);
        }

        [Test]
        public void CmdLineWin_ListOptionQuoted1()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", @"test1,'test2','test 3'" }, OptionsStyle.Windows);

            Assert.AreEqual(3, myOptions.List.Count);
            Assert.AreEqual("test1", myOptions.List[0]);
            Assert.AreEqual("test2", myOptions.List[1]);
            Assert.AreEqual("test 3", myOptions.List[2]);
        }

        [Test]
        public void CmdLineWin_ListOptionQuoted2()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", @"test1,'test2,test3b','test 3'" }, OptionsStyle.Windows);

            Assert.AreEqual(3, myOptions.List.Count);
            Assert.AreEqual("test1", myOptions.List[0]);
            Assert.AreEqual("test2,test3b", myOptions.List[1]);
            Assert.AreEqual("test 3", myOptions.List[2]);
        }

        [Test]
        [ExpectedException(typeof(OptionException))]
        public void CmdLineWin_ListOptionQuotedInvalid1()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", @"test1:'test2:test3b':'test 3'x" }, OptionsStyle.Windows);
        }

        [Test]
        public void CmdLineWin_ListOptionWindowsPath()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", @"c:\users\homeuser" }, OptionsStyle.Windows);
            Assert.AreEqual(1, myOptions.List.Count);
            Assert.AreEqual(@"c:\users\homeuser", myOptions.List[0]);
        }

        [Test]
        public void CmdLineWin_ListOptionQuotedEndEscape()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", @"test1,test2,testx\" }, OptionsStyle.Windows);
            Assert.AreEqual(3, myOptions.List.Count);
            Assert.AreEqual("test1", myOptions.List[0]);
            Assert.AreEqual("test2", myOptions.List[1]);
            Assert.AreEqual(@"testx\", myOptions.List[2]);
        }

        [Test]
        [ExpectedException(typeof(OptionException))]
        public void CmdLineWin_ListOptionQuotedInvalid3()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "/l", "test1:test2:\"test3" }, OptionsStyle.Windows);
        }

        [Test]
        public void CmdLineWin_DefaultValueProvidedShort()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b", "/v:3" }, OptionsStyle.Windows);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("3", myOptions.Verbosity);
        }

        [Test]
        public void CmdLineWin_DefaultValueProvidedShort2()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b", "/v", "3" }, OptionsStyle.Windows);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("3", myOptions.Verbosity);
        }

        [Test]
        public void CmdLineWin_DefaultValueShort()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b", "/v" }, OptionsStyle.Windows);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("0", myOptions.Verbosity);
        }

        [Test]
        public void CmdLineWin_DefaultValueProvidedLongEquals()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b", "/verbosity:2" }, OptionsStyle.Windows);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("2", myOptions.Verbosity);
        }

        [Test]
        public void CmdLineWin_DefaultValueProvidedLong()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b", "/verbosity", "2" }, OptionsStyle.Windows);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("2", myOptions.Verbosity);
        }

        [Test]
        public void CmdLineWin_DefaultValueLong()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "/a", "/b", "/verbosity" }, OptionsStyle.Windows);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("0", myOptions.Verbosity);
        }

        [Test]
        public void CmdLineWin_DefaultValueLong2()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "/verbosity", "/a", "/b" }, OptionsStyle.Windows);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("0", myOptions.Verbosity);
        }

        [Test]
        public void CmdLineWin_TypesEnum()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = Options.Parse(myOptions, new[] { "/c", "yellow" }, OptionsStyle.Windows);

            Assert.AreEqual(BasicColor.Yellow, myOptions.Color);
        }

        [Test]
        public void CmdLineWin_TypesEnumInt()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = Options.Parse(myOptions, new[] { "/c", "4" }, OptionsStyle.Windows);

            Assert.AreEqual(BasicColor.Cyan, myOptions.Color);
        }

        [Test]
        public void CmdLineWin_TypesInt()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = Options.Parse(myOptions, new[] { "/O", "100" }, OptionsStyle.Windows);

            Assert.AreEqual(100, myOptions.Opacity);
        }

        [Test]
        [ExpectedException(typeof(OptionFormatException))]
        public void CmdLineWin_TypesIntInvalid()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = Options.Parse(myOptions, new[] { "/O", "xxx" }, OptionsStyle.Windows);
        }
    }
}
