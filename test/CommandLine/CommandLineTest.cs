﻿using System;
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
            [Option('a', "along", false)]
            public bool OptionA;

            [Option('b', "blong", false)]
            public bool OptionB;

            [Option('c', "clong", false)]
            public string OptionC;

            private List<string> m_Arguments = new List<string>();

            [OptionArguments]
            public IList<string> Arguments { get { return m_Arguments; } }
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CmdLine_NullOptions()
        {
            Options options = Options.Parse(null, null);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_NullArguments()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, null);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_NoArguments()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new string[0], OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_NullArgumentsEmptyOptions()
        {
            NoArguments myOptions = new NoArguments();
            Options options = Options.Parse(myOptions, null, OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_NoArgumentsEmptyOptions()
        {
            NoArguments myOptions = new NoArguments();
            Options options = Options.Parse(myOptions, new string[0], OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_SingleShortOption()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-a" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.IsNull(myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsJoined()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-ab" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.IsNull(myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsSeparated()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-a", "-b" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.IsNull(myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsOnProperties()
        {
            PropertyOptions myOptions = new PropertyOptions();
            Options options = Options.Parse(myOptions, new[] { "-a", "-b" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.IsNull(myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsStringOneArg()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-cfoo" }, OptionsStyle.Unix);

            Assert.IsFalse(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.AreEqual("foo", myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsStringTwoArgs()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-c", "foo" }, OptionsStyle.Unix);

            Assert.IsFalse(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.AreEqual("foo", myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsStringOneArgAssigned()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-c=foo" }, OptionsStyle.Unix);

            Assert.IsFalse(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.AreEqual("foo", myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsStringOneArgJoined()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-abcfoo" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("foo", myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsStringTwoArgsJoined1()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-abc", "foo" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("foo", myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsStringTwoArgsJoined2()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-cab", "foo" }, OptionsStyle.Unix);

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
            Options options = Options.Parse(myOptions, new[] { "-ifs", "string" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.CaseInsensitive);
            Assert.IsTrue(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsRequiredOneArgument1()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "-ifsstring" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.CaseInsensitive);
            Assert.IsTrue(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsRequiredOneArgument2()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "-if", "-sstring" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.CaseInsensitive);
            Assert.IsTrue(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsRequiredOneArgument3()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "-if", "-s=string" }, OptionsStyle.Unix);

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
            Options options = Options.Parse(myOptions, new[] { "-ifs" }, OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof (OptionMissingException))]
        public void CmdLineUnix_MissingOption()
        {
            RequiredOptions myOptions = new RequiredOptions();
            Options options = Options.Parse(myOptions, new[] { "-ab" }, OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_RequiredOption()
        {
            RequiredOptions myOptions = new RequiredOptions();
            Options options = Options.Parse(myOptions, new[] { "-c=bar" }, OptionsStyle.Unix);

            Assert.AreEqual("bar", myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_LongOptionOneArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "--search=string" }, OptionsStyle.Unix);

            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsFalse(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_LongOptionTwoArguments()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "--search", "string" }, OptionsStyle.Unix);

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
            Options options = Options.Parse(myOptions, new[] { "--search" }, OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_LongOptionsBoolean()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "--printfiles" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.PrintFiles);
            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsNull(myOptions.SearchString);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void cmdLineUnix_LongOptionOnly()
        {
            OptionalLongArguments myOptions = new OptionalLongArguments();
            Options options = Options.Parse(myOptions, new[] { "--along" }, OptionsStyle.Unix);

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
            Options options = Options.Parse(myOptions, new[] { "-a" }, OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionUnknownException))]
        public void CmdLineUnix_UnknownOption1()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = Options.Parse(myOptions, new[] { "--search", "string", "-ab", "-c" }, OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionUnknownException))]
        public void CmdLineUnix_UnknownOption2()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-dabc" }, OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionUnknownException))]
        public void CmdLineUnix_UnknownLongOption()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "--foobar" }, OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionException))]
        public void CmdLineUnix_ExtraArgument()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-ab", "argument", "-c" }, OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ListOption()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "-l", "test1,test2,test3" }, OptionsStyle.Unix);

            Assert.AreEqual(3, myOptions.List.Count);
            Assert.AreEqual("test1", myOptions.List[0]);
            Assert.AreEqual("test2", myOptions.List[1]);
            Assert.AreEqual("test3", myOptions.List[2]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ListOptionSingleElement()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "-l", "test1" }, OptionsStyle.Unix);

            Assert.AreEqual(1, myOptions.List.Count);
            Assert.AreEqual("test1", myOptions.List[0]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ListOptionMultipleArguments()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "-l", "test1", "-l", "test2", "-l", "test3" }, OptionsStyle.Unix);

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
            Options options = Options.Parse(myOptions, new[] { "-l", @"te\st1", "-l", @"tes\t2", "-l", @"\test3" }, OptionsStyle.Unix);

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
            Options options = Options.Parse(myOptions, new[] { "-l", @"test1,'test2','test 3'" }, OptionsStyle.Unix);

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
            Options options = Options.Parse(myOptions, new[] { "-l", @"test1,'test2,test3b','test 3'" }, OptionsStyle.Unix);

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
            Options options = Options.Parse(myOptions, new[] { "-l", @"test1,'test2,test3b','test 3'x" }, OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionException))]
        public void CmdLineUnix_ListOptionQuotedInvalid2()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "-l", @"test1,test2,testx\" }, OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionException))]
        public void CmdLineUnix_ListOptionQuotedInvalid3()
        {
            ListOptions myOptions = new ListOptions();
            Options options = Options.Parse(myOptions, new[] { "-l", "test1,test2,\"test3" }, OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_StopParsing()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new[] { "-abc", "argument", "--", "-c" }, OptionsStyle.Unix);

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
            Options options = Options.Parse(myOptions, new[] { "-abv3" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("3", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueProvidedShort2()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "-abv", "3" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("3", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueShort()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "-abv" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("0", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueShortProvidedExistingOption()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "-avb" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.AreEqual("b", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueProvidedLongEquals()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "-ab", "--verbosity=2" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("2", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueProvidedLong()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "-ab", "--verbosity", "2" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("2", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueLong()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "-ab", "--verbosity" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("0", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_DefaultValueLong2()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = Options.Parse(myOptions, new[] { "--verbosity", "-ab" }, OptionsStyle.Unix);

            Assert.IsTrue(myOptions.OptionA);
            Assert.IsTrue(myOptions.OptionB);
            Assert.AreEqual("0", myOptions.Verbosity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_TypesEnum()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = Options.Parse(myOptions, new[] { "-c", "yellow" }, OptionsStyle.Unix);

            Assert.AreEqual(BasicColor.Yellow, myOptions.Color);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_TypesEnumInt()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = Options.Parse(myOptions, new[] { "-c", "4" }, OptionsStyle.Unix);

            Assert.AreEqual(BasicColor.Cyan, myOptions.Color);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_TypesInt()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = Options.Parse(myOptions, new[] { "-O", "100" }, OptionsStyle.Unix);

            Assert.AreEqual(100, myOptions.Opacity);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionFormatException))]
        public void CmdLineUnix_TypesIntInvalid()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = Options.Parse(myOptions, new[] { "-O", "xxx" }, OptionsStyle.Unix);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ArgumentsAttribute()
        {
            ArgumentsAttributeOptions myOptions = new ArgumentsAttributeOptions();
            Options options = Options.Parse(myOptions, new[] { "-a", "arg1", "arg2" }, OptionsStyle.Unix);

            Assert.AreEqual(2, options.Arguments.Count);
            Assert.AreEqual(2, myOptions.Arguments.Count);
            Assert.AreEqual("arg1", myOptions.Arguments[0]);
            Assert.AreEqual("arg2", myOptions.Arguments[1]);
        }
    }
}
