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

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_NullArguments()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(myOptions);
            options.ParseCommandLine(null);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_NoArguments()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(myOptions);
            options.ParseCommandLine(new string[0]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_NullArgumentsEmptyOptions()
        {
            NoArguments myOptions = new NoArguments();
            Options options = new Options(myOptions);
            options.ParseCommandLine(null);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_NoArgumentsEmptyOptions()
        {
            NoArguments myOptions = new NoArguments();
            Options options = new Options(myOptions);
            options.ParseCommandLine(new string[0]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_SingleShortOption()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(myOptions);
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
            Options options = new Options(myOptions);
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
            Options options = new Options(myOptions);
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
            Options options = new Options(myOptions);
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
            Options options = new Options(myOptions);
            options.ParseCommandLine(new[] { "-c", "foo" });

            Assert.IsFalse(myOptions.OptionA);
            Assert.IsFalse(myOptions.OptionB);
            Assert.AreEqual("foo", myOptions.OptionC);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_ShortOptionsStringOneArgJoined()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(myOptions);
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
            Options options = new Options(myOptions);
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
            Options options = new Options(myOptions);
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
            Options options = new Options(myOptions);
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
            Options options = new Options(myOptions);
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
            Options options = new Options(myOptions);
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
            Options options = new Options(myOptions);
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
            Options options = new Options(myOptions);
            options.ParseCommandLine(new[] { "-ifs" });

            Assert.Fail("Exception not thrown");
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLineUnix_LongOptionOneArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = new Options(myOptions);
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
            Options options = new Options(myOptions);
            options.ParseCommandLine(new[] { "--search", "string" });

            Assert.IsFalse(myOptions.CaseInsensitive);
            Assert.IsFalse(myOptions.PrintFiles);
            Assert.AreEqual("string", myOptions.SearchString);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionUnknownException))]
        public void CmdLineUnix_UnknownOption1()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = new Options(myOptions);
            options.ParseCommandLine(new[] { "--search", "string", "-ab", "-c" });

            Assert.Fail("Exception not thrown");
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionUnknownException))]
        public void CmdLineUnix_UnknownOption2()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(myOptions);
            options.ParseCommandLine(new[] { "-dabc" });

            Assert.Fail("Exception not thrown");
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionUnknownException))]
        public void CmdLineUnix_UnknownLongOption()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(myOptions);
            options.ParseCommandLine(new[] { "--foobar" });

            Assert.Fail("Exception not thrown");
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        [ExpectedException(typeof(OptionException))]
        public void CmdLineUnix_ExtraArgument()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = new Options(myOptions);
            options.ParseCommandLine(new[] { "-ab", "argument", "-c" });

            Assert.Fail("Exception not thrown");
        }
    }
}
