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
        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLine_SingleOption_Optional()
        {
            Option[] optlist = {
                new Option('l', "longoption")
            };

            // No command line
            string[] args1 = new string[0];
            Options options = new Options(optlist, args1);
            Assert.AreEqual(0, options.RawOptions.Keys.Count);

            // Command line, short option
            string[] args2 = { "-l" };
            options = new Options(optlist, args2);
            Assert.IsNull(options.RawOptions["longoption"]);

            // Command line, long option
            string[] args3 = { "--longoption" };
            options = new Options(optlist, args3);
            Assert.IsNull(options.RawOptions["longoption"]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLine_SingleOption_Required()
        {
            Options options;

            Option[] optlist = {
                new Option('l', "longoption", OptRequired.Required)
            };

            // No command line
            try {
                string[] args1 = new string[0];
                options = new Options(optlist, args1);
                Assert.Fail("Required Option didn't throw exception");
            } catch (System.Exception ex) {
                if (ex is AssertFailedException) throw;
            }

            // Command line, short option
            string[] args2 = { "-l" };
            options = new Options(optlist, args2);
            Assert.IsNull(options.RawOptions["longoption"]);

            // Command line, long option
            string[] args3 = { "--longoption" };
            options = new Options(optlist, args3);
            Assert.IsNull(options.RawOptions["longoption"]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLine_SingleOption_LongOption_NoParameterRequired()
        {
            Option[] optlist = {
                new Option('l', "longoption", OptRequired.Required)
            };

            // No parameters
            string[] args1 = { "--longoption" };
            Options options = new Options(optlist, args1);
            Assert.IsNull(options.RawOptions["longoption"]);

            // Empty Parameter
            try {
                string[] args2 = { "--longoption=" };
                options = new Options(optlist, args2);
                Assert.Fail("Exception expected when passing a parameter for parameterless option");
            } catch (System.Exception ex) {
                if (ex is AssertFailedException) throw;
            }

            // Empty Parameter
            try {
                string[] args2 = { "--longoption=foo" };
                options = new Options(optlist, args2);
                Assert.Fail("Exception expected when passing a parameter for parameterless option");
            } catch (System.Exception ex) {
                if (ex is AssertFailedException) throw;
            }
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLine_SingleOption_LongOption_ParameterRequired()
        {
            Options options;

            Option[] optlist1 = {
                new Option('l', "longoption", OptRequired.Required, OptParamRequired.Required, OptParamType.String)
            };

            // No parameters
            try {
                string[] args1 = { "--longoption" };
                options = new Options(optlist1, args1);
                Assert.Fail("Exception expected as no parameter provided");
            } catch (System.Exception ex) {
                if (ex is AssertFailedException) throw;
            }

            // Empty Parameter
            string[] args2 = { "--longoption=" };
            options = new Options(optlist1, args2);
            Assert.AreEqual("", options.RawOptions["longoption"]);

            // Full Parameter
            string[] args3 = { "--longoption=foo" };
            options = new Options(optlist1, args3);
            Assert.AreEqual("foo", options.RawOptions["longoption"]);

            // Full Parameter
            string[] args4 = { "--longoption", "foobar" };
            options = new Options(optlist1, args4);
            Assert.AreEqual("foobar", options.RawOptions["longoption"]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLine_SingleOption_LongOption_ParameterOptional()
        {
            Option[] optlist1 = {
                new Option('l', "longoption", OptRequired.Required, OptParamRequired.Optional, OptParamType.String)
            };

            // No parameters
            string[] args1 = { "--longoption" };
            Options options = new Options(optlist1, args1);
            Assert.IsNull(options.RawOptions["longoption"]);

            // Empty Parameter
            string[] args2 = { "--longoption=" };
            options = new Options(optlist1, args2);
            Assert.AreEqual("", options.RawOptions["longoption"]);

            // Full Parameter
            string[] args3 = { "--longoption=foo" };
            options = new Options(optlist1, args3);
            Assert.AreEqual("foo", options.RawOptions["longoption"]);

            // Full Parameter
            string[] args4 = { "--longoption", "foobar" };
            options = new Options(optlist1, args4);
            Assert.AreEqual("foobar", options.RawOptions["longoption"]);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLine_UnsupportedOption()
        {
            Option[] optlist1 = {
                new Option('l', "longoption", OptRequired.Optional, OptParamRequired.Optional, OptParamType.String)
            };

            // No parameters
            string[] args1 = { "--longoption" };
            Options options = new Options(optlist1, args1);
            Assert.IsNull(options.RawOptions["longoption"]);

            // Unknown Option
            // No parameters
            try {
                string[] args2 = { "--verylongtoption" };
                options = new Options(optlist1, args2);
                Assert.Fail("Unknown option didn't raise an exception");
            } catch (System.Exception ex) {
                if (ex is AssertFailedException) throw;
            }
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLine_RemainingArguments()
        {
            Option[] optlist1 = {
                new Option('l', "longoption", OptRequired.Optional, OptParamRequired.Optional, OptParamType.String)
            };

            // No parameters
            string[] args1 = { "--longoption" };
            Options options = new Options(optlist1, args1);
            Assert.IsNull(options.RawOptions["longoption"]);
            Assert.AreEqual(0, options.RemainingArguments.Length);

            // No parameters, remaining arguments
            string[] args2 = { "--longoption", "--", "File1" };
            options = new Options(optlist1, args2);
            Assert.IsNull(options.RawOptions["longoption"]);
            Assert.AreEqual(1, options.RemainingArguments.Length);

            // No parameters, remaining arguments
            string[] args3 = { "--longoption", "--" };
            options = new Options(optlist1, args3);
            Assert.IsNull(options.RawOptions["longoption"]);
            Assert.AreEqual(0, options.RemainingArguments.Length);
        }

        [TestMethod]
        [TestCategory("CommandLine")]
        public void CmdLine_MultipleShortOptions()
        {
            Option[] optlist = {
                new Option('a', "optiona", OptRequired.Optional, OptParamRequired.Optional, OptParamType.String),
                new Option('b', "optionb", OptRequired.Optional, OptParamRequired.None, OptParamType.None),
                new Option('c', "optionc", OptRequired.Optional, OptParamRequired.Optional, OptParamType.String),
                new Option('d', "optiond", OptRequired.Optional, OptParamRequired.None, OptParamType.None),
                new Option('e', "optione", OptRequired.Optional, OptParamRequired.Optional, OptParamType.String),
                new Option('f', "optionf", OptRequired.Optional, OptParamRequired.None, OptParamType.None)
            };

            // No options, just normal arguments
            string[] args1 = { "file1", "file2" };
            Options options = new Options(optlist, args1);
            Assert.AreEqual(0, options.RawOptions.Count);
            Assert.AreEqual(2, options.RemainingArguments.Length);

            // All optional arguments without parameters
            string[] args2 = { "-bdf" };
            options = new Options(optlist, args2);
            Assert.AreEqual(3, options.RawOptions.Count);

            // All optional arguments without parameters
            string[] args3 = { "-bdf", "file1" };
            options = new Options(optlist, args3);
            Assert.AreEqual(3, options.RawOptions.Count);
            Assert.AreEqual(1, options.RemainingArguments.Length);

            // All optional arguments without parameters
            string[] args4 = { "-bdf", "--", "-ace", "file1" };
            options = new Options(optlist, args4);
            Assert.AreEqual(3, options.RawOptions.Count);
            Assert.AreEqual(2, options.RemainingArguments.Length);
            Assert.AreEqual("-ace", options.RemainingArguments[0]);
            Assert.AreEqual("file1", options.RemainingArguments[1]);

            // A single option that requires an optional argument
            string[] args5 = { "-abd", "--", "arg1" };
            options = new Options(optlist, args5);
            Assert.AreEqual(3, options.RawOptions.Count);
            Assert.IsNull(options.RawOptions["optiona"]);
            Assert.IsNull(options.RawOptions["optionb"]);
            Assert.IsNull(options.RawOptions["optiond"]);
            Assert.AreEqual(1, options.RemainingArguments.Length);
            Assert.AreEqual("arg1", options.RemainingArguments[0]);

            // A single option that requires an optional argument
            string[] args6 = { "-abd", "arg1" };
            options = new Options(optlist, args6);
            Assert.AreEqual(3, options.RawOptions.Count);
            Assert.AreEqual("arg1", options.RawOptions["optiona"]);
            Assert.IsNull(options.RawOptions["optionb"]);
            Assert.IsNull(options.RawOptions["optiond"]);
            Assert.AreEqual(0, options.RemainingArguments.Length);

            // Ambiguous options
            try {
                string[] args7 = { "-adc", "arg1", "arg2" };
                options = new Options(optlist, args7);
                Assert.Fail("Ambiguous options accepted");
            } catch (System.Exception e) {
                if (e is AssertFailedException) throw;
            }

            // Undefined option
            try {
                string[] args7 = { "-adx", "arg1", "arg2" };
                options = new Options(optlist, args7);
                Assert.Fail("Undefined options -x accepted");
            } catch (System.Exception e) {
                if (e is AssertFailedException) throw;
            }
        }
    }
}
