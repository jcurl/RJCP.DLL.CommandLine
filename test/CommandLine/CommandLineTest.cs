using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RJCP.Core.CommandLine
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class CommandLineTest
    {
        private class NoArguments
        {
            public bool NoOption;
        }
        
        private class OptionalArguments
        {
            [Option('a', "along", false)]
            public bool OptionA;

            [Option('b', "blong", false)]
            public bool OptionB;

            [Option('c', "clong", false)]
            public string OptionC;
        }

        private class RequiredArguments
        {
            [Option('i', "insensitive")]
            private bool m_CaseInsensitive;

            [Option('f', "printfiles")]
            private bool m_PrintFiles;

            [Option('s', "search")]
            private string m_SearchString;

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
            [Option("along")]
            public bool OptionA;

            [Option("blong")]
            public bool OptionB;

            [Option("clong")]
            public string OptionC;
        }

        private class DefaultValueOption
        {
            [Option('a', "along", false)]
            public bool OptionA { get; set; }

            [Option('b', "blong", false)]
            public bool OptionB { get; set; }

            [Option('v', "verbosity")]
            [OptionDefault("0")]
            public string Verbosity;
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CmdLine_NullOptions()
        {
            Options options = new Options(null);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_NullArguments()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(null);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_NoArguments()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new string[0]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_NullArgumentsEmptyOptions()
        {
            NoArguments myOptions = new NoArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(null);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_NoArgumentsEmptyOptions()
        {
            NoArguments myOptions = new NoArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new string[0]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_SingleShortOption()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] {"-a"});

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.IsNull(myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsJoined()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-ab" });

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.IsNull(myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsSeparated()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-a", "-b" });

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.IsNull(myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsOnProperties()
        {
            PropertyOptions myOptions = new PropertyOptions();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-a", "-b" });

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.IsNull(myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsStringOneArg()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-cfoo" });

            Assert.IsFalse(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.AreEqual("foo", myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsStringTwoArgs()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-c", "foo" });

            Assert.IsFalse(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.AreEqual("foo", myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsStringOneArgAssigned()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-c=foo" });

            Assert.IsFalse(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.AreEqual("foo", myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsStringOneArgJoined()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-abcfoo" });

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("foo", myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsStringTwoArgsJoined1()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-abc", "foo" });

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("foo", myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsStringTwoArgsJoined2()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-cab", "foo" });

            Assert.IsFalse(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.AreEqual("ab", myOptions.OptionC);
            Assert.AreEqual(1, options.Arguments.Count);
            Assert.AreEqual("foo", options.Arguments[0]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsRequired()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-ifs", "string" });

            Assert.IsTrue(myOptions.CaseInsensitive);
            Assert.IsTrue(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsRequiredOneArgument1()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-ifsstring" });

            Assert.IsTrue(myOptions.CaseInsensitive);
            Assert.IsTrue(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsRequiredOneArgument2()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-if", "-sstring" });

            Assert.IsTrue(myOptions.CaseInsensitive);
            Assert.IsTrue(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsRequiredOneArgument3()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-if", "-s=string" });

            Assert.IsTrue(myOptions.CaseInsensitive);
            Assert.IsTrue(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionMissingArgumentException))]
        public void CmdLineUnix_ShortOptionsRequiredMissingArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-ifs" });

            Assert.Fail("Exception not thrown");
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof (OptionMissingException))]
        public void CmdLineUnix_MissingOption()
        {
            RequiredOptions myOptions = new RequiredOptions();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-ab" });

            Assert.Fail("Exception not thrown");
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_RequiredOption()
        {
            RequiredOptions myOptions = new RequiredOptions();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-c=bar" });

            Assert.AreEqual("bar", myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_LongOptionOneArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "--search=string" });

            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsFalse(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_LongOptionTwoArguments()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "--search", "string" });

            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsFalse(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionMissingArgumentException))]
        public void CmdLineUnix_LongOptionsRequiredMissingArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "--search" });

            Assert.Fail("Exception not thrown");
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_LongOptionsBoolean()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "--printfiles" });

            Assert.IsTrue(myOptions.PrintFiles);
            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsNull(myOptions.SearchString);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void cmdLineUnix_LongOptionOnly()
        {
            OptionalLongArguments myOptions = new OptionalLongArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "--along" });

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.IsNull(myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionUnknownException))]
        public void cmdLineUnix_LongOptionOnlyShort()
        {
            OptionalLongArguments myOptions = new OptionalLongArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-a" });

            Assert.Fail("Exception not thrown");
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionUnknownException))]
        public void CmdLineUnix_UnknownOption1()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "--search", "string", "-ab", "-c" });

            Assert.Fail("Exception not thrown");
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionUnknownException))]
        public void CmdLineUnix_UnknownOption2()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-dabc" });

            Assert.Fail("Exception not thrown");
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionUnknownException))]
        public void CmdLineUnix_UnknownLongOption()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "--foobar" });

            Assert.Fail("Exception not thrown");
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionException))]
        public void CmdLineUnix_ExtraArgument()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-ab", "argument", "-c" });

            Assert.Fail("Exception not thrown");
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ListOption()
        {
            ListOptions myOptions = new ListOptions();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-l", "test1,test2,test3" });

            Assert.AreEqual(3, myOptions.List.Count);
            Assert.AreEqual("test1", myOptions.List[0]);
            Assert.AreEqual("test2", myOptions.List[1]);
            Assert.AreEqual("test3", myOptions.List[2]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ListOptionMultipleArguments()
        {
            ListOptions myOptions = new ListOptions();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-l", "test1", "-l", "test2", "-l", "test3" });

            Assert.AreEqual(3, myOptions.List.Count);
            Assert.AreEqual("test1", myOptions.List[0]);
            Assert.AreEqual("test2", myOptions.List[1]);
            Assert.AreEqual("test3", myOptions.List[2]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ListOptionEscaped()
        {
            ListOptions myOptions = new ListOptions();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-l", @"te\st1", "-l", @"tes\t2", "-l", @"\test3" });

            Assert.AreEqual(3, myOptions.List.Count);
            Assert.AreEqual("test1", myOptions.List[0]);
            Assert.AreEqual("test2", myOptions.List[1]);
            Assert.AreEqual("test3", myOptions.List[2]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ListOptionQuoted1()
        {
            ListOptions myOptions = new ListOptions();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-l", @"test1,'test2','test 3'" });

            Assert.AreEqual(3, myOptions.List.Count);
            Assert.AreEqual("test1", myOptions.List[0]);
            Assert.AreEqual("test2", myOptions.List[1]);
            Assert.AreEqual("test 3", myOptions.List[2]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ListOptionQuoted2()
        {
            ListOptions myOptions = new ListOptions();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-l", @"test1,'test2,test3b','test 3'" });

            Assert.AreEqual(3, myOptions.List.Count);
            Assert.AreEqual("test1", myOptions.List[0]);
            Assert.AreEqual("test2,test3b", myOptions.List[1]);
            Assert.AreEqual("test 3", myOptions.List[2]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionException))]
        public void CmdLineUnix_ListOptionQuotedInvalid1()
        {
            ListOptions myOptions = new ListOptions();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-l", @"test1,'test2,test3b','test 3'x" });
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionException))]
        public void CmdLineUnix_ListOptionQuotedInvalid2()
        {
            ListOptions myOptions = new ListOptions();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-l", @"test1,test2,testx\" });
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionException))]
        public void CmdLineUnix_ListOptionQuotedInvalid3()
        {
            ListOptions myOptions = new ListOptions();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-l", "test1,test2,\"test3" });
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_StopParsing()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-abc", "argument", "--", "-c" });

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("argument", myOptions.OptionC);
            Assert.AreEqual(1, options.Arguments.Count);
            Assert.AreEqual("-c", options.Arguments[0]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueProvidedShort()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] {"-abv3"} );

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("3", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueProvidedShort2()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-abv", "3" });

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("3", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueShort()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-abv" });

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("0", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueShortProvidedExistingOption()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-avb" });

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.AreEqual("b", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueProvidedLongEquals()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-ab", "--verbosity=2" });

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("2", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueProvidedLong()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-ab", "--verbosity", "2" });

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("2", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueLong()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "-ab", "--verbosity" });

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("0", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueLong2()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = new Options(OptionsStyle.Unix, myOptions);
            options.ParseCommandLine(new[] { "--verbosity", "-ab" });

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("0", myOptions.Verbosity);
        }
    }
}
