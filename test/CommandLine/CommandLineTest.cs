namespace RJCP.Core.CommandLine
{
    using System;
    using NUnit.Framework;

    [TestFixture(OptionsStyle.Unix)]
    [TestFixture(OptionsStyle.Windows)]
    public class CommandLineTest
    {
        public CommandLineTest(OptionsStyle style)
        {
            CommandLineStyle = style;
        }

        public OptionsStyle CommandLineStyle { get; }

        private Options GetOptions(object optionsConfig, string[] windowsArgs, string[] unixArgs)
        {
            switch (CommandLineStyle) {
            case OptionsStyle.Windows:
                return Options.Parse(optionsConfig, windowsArgs, OptionsStyle.Windows);
            case OptionsStyle.Unix:
                return Options.Parse(optionsConfig, unixArgs, OptionsStyle.Unix);
            default:
                Assert.Fail("Unknown options style");
                return null;
            }
        }

        [Test]
        public void NullArguments()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, null, CommandLineStyle);
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void NoArguments()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new string[0], CommandLineStyle);
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void NullArgumentsEmptyOptions()
        {
            NoArguments myOptions = new NoArguments();
            Options options = Options.Parse(myOptions, null, CommandLineStyle);
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void NoArgumentsEmptyOptions()
        {
            NoArguments myOptions = new NoArguments();
            Options options = Options.Parse(myOptions, new string[0], CommandLineStyle);
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void SingleShortOption()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = GetOptions(myOptions, new[] { "/a" }, new[] { "-a" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.Null);
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void EmptyOption()
        {
            OptionalArguments myOptions = new OptionalArguments();

            Options options = GetOptions(myOptions, new[] { "/" }, new[] { "-" });
            Assert.That(options.Arguments, Has.Count.EqualTo(1));

            switch (CommandLineStyle) {
            case OptionsStyle.Windows:
                Assert.That(options.Arguments[0], Is.EqualTo("/"));
                return;
            case OptionsStyle.Unix:
                Assert.That(options.Arguments[0], Is.EqualTo("-"));
                return;
            default:
                Assert.Fail("Unknown options style");
                return;
            }
        }

        [Test]
        public void ShortOptionsJoined()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options;
            switch (CommandLineStyle) {
            case OptionsStyle.Windows:
                Assert.That(() => {
                    Options.Parse(myOptions, new[] { "/ab" }, OptionsStyle.Windows);
                }, Throws.TypeOf<OptionUnknownException>());
                return;
            case OptionsStyle.Unix:
                options = Options.Parse(myOptions, new[] { "-ab" }, OptionsStyle.Unix);
                break;
            default:
                Assert.Fail("Unknown options style");
                return;
            }

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.Null);
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ShortOptionsSeparated()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = GetOptions(myOptions, new[] { "/a", "/b" }, new[] { "-a", "-b" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.Null);
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ShortOptionsOnProperties()
        {
            PropertyOptions myOptions = new PropertyOptions();
            Options options = GetOptions(myOptions, new[] { "/a", "/b" }, new[] { "-a", "-b" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.Null);
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ShortOptionsStringOneArg()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = GetOptions(myOptions, new[] { "/c:foo" }, new[] { "-cfoo" });

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ShortOptionsStringOneArgAssigned()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = GetOptions(myOptions, new[] { "/c:foo" }, new[] { "-c=foo" });

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ShortOptionsStringTwoArgs()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = GetOptions(myOptions, new[] { "/c", "foo" }, new[] { "-c", "foo" });

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void OptionStringParameterSingleSlash()
        {
            OptionHandling myOptions = new OptionHandling();
            Options options = GetOptions(myOptions, new[] { "/clong:/" }, new[] { "--clong=/" });

            Assert.That(myOptions.OptionC, Is.EqualTo("/"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void OptionStringParameterSingleSlashTwoArgs()
        {
            OptionHandling myOptions = new OptionHandling();
            Options options = GetOptions(myOptions, new[] { "/clong", "/" }, new[] { "--clong", "/" });

            Assert.That(myOptions.OptionC, Is.EqualTo("/"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void OptionStringParameterSingleEqual()
        {
            OptionHandling myOptions = new OptionHandling();
            Options options = GetOptions(myOptions, new[] { "/clong:=" }, new[] { "--clong==" });

            Assert.That(myOptions.OptionC, Is.EqualTo("="));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void OptionStringParameterSingleEqualTwoArgs()
        {
            OptionHandling myOptions = new OptionHandling();
            Options options = GetOptions(myOptions, new[] { "/clong", "=" }, new[] { "--clong", "=" });

            Assert.That(myOptions.OptionC, Is.EqualTo("="));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void OptionStringParameterSingleColon()
        {
            OptionHandling myOptions = new OptionHandling();
            Options options = GetOptions(myOptions, new[] { "/clong::" }, new[] { "--clong=:" });

            Assert.That(myOptions.OptionC, Is.EqualTo(":"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void OptionStringParameterSingleColonTwoArgs()
        {
            OptionHandling myOptions = new OptionHandling();
            Options options = GetOptions(myOptions, new[] { "/clong", ":" }, new[] { "--clong", ":" });

            Assert.That(myOptions.OptionC, Is.EqualTo(":"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void OptionStringParameterDualSlash()
        {
            OptionHandling myOptions = new OptionHandling();
            Options options = GetOptions(myOptions, new[] { "/clong://" }, new[] { "--clong=//" });

            Assert.That(myOptions.OptionC, Is.EqualTo("//"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void OptionStringParameterDualSlashTwoArgs()
        {
            OptionHandling myOptions = new OptionHandling();
            Options options = null;
            switch (CommandLineStyle) {
            case OptionsStyle.Windows:
                // This throws an exception, as following '/clong' is '//' which is interpreted as a new option. If you
                // really want to pass '//' to '/clong', then use '/clong://'
                Assert.That(() => {
                    _ = Options.Parse(myOptions, new[] { "/clong", "//" }, OptionsStyle.Windows);
                }, Throws.TypeOf<OptionMissingArgumentException>());

                // Then the exception is raised because '/clong' must require a parameter and has no default.
                Assert.That(myOptions.InvalidOptions, Is.EquivalentTo(new[] { "/clong" }));
                break;
            case OptionsStyle.Unix:
                options = Options.Parse(myOptions, new[] { "--clong", "//" }, OptionsStyle.Unix);
                Assert.That(myOptions.OptionC, Is.EqualTo("//"));
                Assert.That(options.Arguments, Is.Empty);
                break;
            default:
                Assert.Fail("Unknown options style");
                break;
            }
        }

        [Test]
        public void OptionStringParameterSingleDash()
        {
            OptionHandling myOptions = new OptionHandling();
            Options options = GetOptions(myOptions, new[] { "/clong:-" }, new[] { "--clong=-" });

            Assert.That(myOptions.OptionC, Is.EqualTo("-"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void OptionStringParameterSingleDashTwoArgs()
        {
            OptionHandling myOptions = new OptionHandling();
            Options options = GetOptions(myOptions, new[] { "/clong", "-" }, new[] { "--clong", "-" });

            Assert.That(myOptions.OptionC, Is.EqualTo("-"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void OptionStringParameterDualDash()
        {
            OptionHandling myOptions = new OptionHandling();
            Options options = GetOptions(myOptions, new[] { "/clong:--" }, new[] { "--clong=--" });

            Assert.That(myOptions.OptionC, Is.EqualTo("--"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void OptionStringParameterDualDashTwoArgs()
        {
            OptionHandling myOptions = new OptionHandling();
            Options options = null;
            switch (CommandLineStyle) {
            case OptionsStyle.Windows:
                options = Options.Parse(myOptions, new[] { "/clong", "--" }, OptionsStyle.Windows);
                Assert.That(myOptions.OptionC, Is.EqualTo("--"));
                Assert.That(options.Arguments, Is.Empty);
                break;
            case OptionsStyle.Unix:
                // This throws an exception, as following '--clong' is '--' which is interpreted as a new option. If you
                // really want to pass '--' to '--clong', then use '--clong=--'
                Assert.That(() => {
                    _ = Options.Parse(myOptions, new[] { "--clong", "--" }, OptionsStyle.Unix);
                }, Throws.TypeOf<OptionMissingArgumentException>());

                // Then the exception is raised because '--clong' must require a parameter and has no default.
                Assert.That(myOptions.InvalidOptions, Is.EquivalentTo(new[] { "--clong" }));
                break;
            default:
                Assert.Fail("Unknown options style");
                break;
            }
        }

        [Test]
        public void OptionStringParameterDualDash2()
        {
            OptionHandling myOptions = new OptionHandling();
            Options options = GetOptions(myOptions, new[] { "/clong:--x" }, new[] { "--clong=--x" });

            Assert.That(myOptions.OptionC, Is.EqualTo("--x"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void OptionStringParameterDualDash2TwoArgs()
        {
            OptionHandling myOptions = new OptionHandling();
            Options options = null;
            switch (CommandLineStyle) {
            case OptionsStyle.Windows:
                options = Options.Parse(myOptions, new[] { "/clong", "--x" }, OptionsStyle.Windows);
                Assert.That(myOptions.OptionC, Is.EqualTo("--x"));
                Assert.That(options.Arguments, Is.Empty);
                break;
            case OptionsStyle.Unix:
                // This throws an exception, as following '--clong' is '--x' which is interpreted as a new option. If you
                // really want to pass '--x' to '--clong', then use '--clong=--x'
                Assert.That(() => {
                    _ = Options.Parse(myOptions, new[] { "--clong", "--x" }, OptionsStyle.Unix);
                }, Throws.TypeOf<OptionMissingArgumentException>());

                // Then the exception is raised because '--clong' must require a parameter and has no default.
                Assert.That(myOptions.InvalidOptions, Is.EquivalentTo(new[] { "--clong" }));
                break;
            default:
                Assert.Fail("Unknown options style");
                break;
            }
        }

        [Test]
        public void ShortOptionsRequired()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/i", "/f", "/s", "string" }, new[] { "-ifs", "string" });

            Assert.That(myOptions.CaseInsensitive, Is.True);
            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ShortOptionsRequiredOneArgument1()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/i", "/f", "/s:string" }, new[] { "-ifsstring" });

            Assert.That(myOptions.CaseInsensitive, Is.True);
            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ShortOptionsRequiredOneArgument2()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/i", "/f", "/s:string" }, new[] { "-if", "-sstring" });

            Assert.That(myOptions.CaseInsensitive, Is.True);
            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ShortOptionsRequiredOneArgument3()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/i", "/f", "/s:string" }, new[] { "-if", "-s=string" });

            Assert.That(myOptions.CaseInsensitive, Is.True);
            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ShortOptionsRequiredMissingArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Assert.That(() => {
                GetOptions(myOptions, new[] { "/i", "/f", "/s" }, new[] { "-ifs" });
            }, Throws.TypeOf<OptionMissingArgumentException>());
        }

        [Test]
        public void MissingOption()
        {
            RequiredOptions myOptions = new RequiredOptions();
            Assert.That(() => {
                GetOptions(myOptions, new[] { "/a", "/b" }, new[] { "-ab" });
            }, Throws.TypeOf<OptionMissingException>());
        }

        [Test]
        public void RequiredOption()
        {
            RequiredOptions myOptions = new RequiredOptions();
            Options options = GetOptions(myOptions, new[] { "/c:bar" }, new[] { "-c=bar" });

            Assert.That(myOptions.OptionC, Is.EqualTo("bar"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void LongOptionOneArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search:string" }, new[] { "--search=string" });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void LongOptionTwoArguments()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search", "string" }, new[] { "--search", "string" });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void LongOptionWithSpace()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search: string " }, new[] { "--search= string " });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo(" string "));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void LongOptionWithSpace2()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search: string" }, new[] { "--search= string" });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo(" string"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void LongOptionWithSpace3()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search:string " }, new[] { "--search=string " });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("string "));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void LongOptionWithSpace4()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search", " string " }, new[] { "--search", " string " });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo(" string "));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void LongOptionWithSpace5()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search", ":", "string" }, new[] { "--search", "=", "string" });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            if (CommandLineStyle == OptionsStyle.Windows)
                Assert.That(myOptions.SearchString, Is.EqualTo(":"));
            if (CommandLineStyle == OptionsStyle.Unix)
                Assert.That(myOptions.SearchString, Is.EqualTo("="));
            Assert.That(options.Arguments, Has.Count.EqualTo(1));
        }

        [Test]
        public void LongOptionWithSpace6()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search", ":", " string " }, new[] { "--search", "=", " string " });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            if (CommandLineStyle == OptionsStyle.Windows)
                Assert.That(myOptions.SearchString, Is.EqualTo(":"));
            if (CommandLineStyle == OptionsStyle.Unix)
                Assert.That(myOptions.SearchString, Is.EqualTo("="));
            Assert.That(options.Arguments, Has.Count.EqualTo(1));
        }

        [Test]
        public void LongOptionWithSpace7()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search", "=", "string" }, new[] { "--search", ":", "string" });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            if (CommandLineStyle == OptionsStyle.Windows)
                Assert.That(myOptions.SearchString, Is.EqualTo("="));
            if (CommandLineStyle == OptionsStyle.Unix)
                Assert.That(myOptions.SearchString, Is.EqualTo(":"));
            Assert.That(options.Arguments, Has.Count.EqualTo(1));
        }

        [Test]
        public void LongOptionWithSpace8()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search", "=", " string " }, new[] { "--search", ":", " string " });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            if (CommandLineStyle == OptionsStyle.Windows)
                Assert.That(myOptions.SearchString, Is.EqualTo("="));
            if (CommandLineStyle == OptionsStyle.Unix)
                Assert.That(myOptions.SearchString, Is.EqualTo(":"));
            Assert.That(options.Arguments, Has.Count.EqualTo(1));
        }

        [Test]
        public void LongOptionsRequiredMissingArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Assert.That(() => {
                GetOptions(myOptions, new[] { "/search" }, new[] { "--search" });
            }, Throws.TypeOf<OptionMissingArgumentException>());
        }

        [Test]
        public void LongOptionsBoolean()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/printfiles" }, new[] { "--printfiles" });

            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.SearchString, Is.Null);
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void LongOptionOnly()
        {
            OptionalLongArguments myOptions = new OptionalLongArguments();
            Options options = GetOptions(myOptions, new[] { "/along" }, new[] { "--along" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.Null);
            Assert.That(myOptions.Level42, Is.Null);
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void LongOptionOnlyShort()
        {
            OptionalLongArguments myOptions = new OptionalLongArguments();
            Assert.That(() => {
                GetOptions(myOptions, new[] { "/a" }, new[] { "-a" });
            }, Throws.TypeOf<OptionUnknownException>());
        }

        [Test]
        public void UnknownOption1()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Assert.That(() => {
                GetOptions(myOptions, new[] { "/search", "string", "/a", "/b", "/c" }, new[] { "--search", "string", "-ab", "-c" });
            }, Throws.TypeOf<OptionUnknownException>());
        }

        [Test]
        public void UnknownOption2()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Assert.That(() => {
                GetOptions(myOptions, new[] { "/d", "/a", "/b", "/c" }, new[] { "-dabc" });
            }, Throws.TypeOf<OptionUnknownException>());
        }

        [Test]
        public void UnknownLongOption()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Assert.That(() => {
                GetOptions(myOptions, new[] { "/foobar" }, new[] { "--foobar" });
            }, Throws.TypeOf<OptionUnknownException>());
        }

        [Test]
        public void InvalidLongOption1a()
        {
            InvalidLongArgumentWithDigit1 myOptions = new InvalidLongArgumentWithDigit1();
            Assert.That(() => {
                Options.Parse(myOptions, new string[] { }, CommandLineStyle);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void InvalidLongOption1b()
        {
            InvalidLongArgumentWithDigit1 myOptions = new InvalidLongArgumentWithDigit1();
            Assert.That(() => {
                GetOptions(myOptions, new[] { "/6502" }, new[] { "--6502" });
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void InvalidLongOption2a()
        {
            InvalidLongArgumentWithDigit2 myOptions = new InvalidLongArgumentWithDigit2();
            Assert.That(() => {
                Options.Parse(myOptions, new string[] { }, CommandLineStyle);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void InvalidLongOption2b()
        {
            InvalidLongArgumentWithDigit2 myOptions = new InvalidLongArgumentWithDigit2();
            Assert.That(() => {
                GetOptions(myOptions, new[] { "/6502level" }, new[] { "--6502level" });
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ShortOptionWithDigit()
        {
            ShortOptionWithDigit myOptions = new ShortOptionWithDigit();
            Options options = GetOptions(myOptions, new[] { "/9" }, new[] { "-9" });
            Assert.That(myOptions.Level, Is.True);
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ExtraArgument()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Assert.That(() => {
                GetOptions(myOptions, new[] { "/a", "/b", "argument", "/c" }, new[] { "-ab", "argument", "-c" });
            }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void ListOption()
        {
            ListOptions myOptions = new ListOptions();
            Options options = GetOptions(myOptions, new[] { "/l", "test1,test2,test3" }, new[] { "-l", "test1,test2,test3" });

            Assert.That(myOptions.List, Has.Count.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo("test3"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ListOptionSingleElement()
        {
            ListOptions myOptions = new ListOptions();
            Options options = GetOptions(myOptions, new[] { "/l", "test1" }, new[] { "-l", "test1" });

            Assert.That(myOptions.List, Has.Count.EqualTo(1));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ListOptionMultipleArguments()
        {
            ListOptions myOptions = new ListOptions();
            Options options = GetOptions(myOptions, new[] { "/l", "test1", "/l", "test2", "/l", "test3" }, new[] { "-l", "test1", "-l", "test2", "-l", "test3" });

            Assert.That(myOptions.List, Has.Count.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo("test3"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ListOptionQuoted1()
        {
            ListOptions myOptions = new ListOptions();
            Options options = GetOptions(myOptions, new[] { "/l", @"test1,'test2','test 3'" }, new[] { "-l", @"test1,'test2','test 3'" });

            Assert.That(myOptions.List, Has.Count.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo("test 3"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ListOptionQuoted2()
        {
            ListOptions myOptions = new ListOptions();
            Options options = GetOptions(myOptions, new[] { "/l", @"test1,'test2,test3b','test 3'" }, new[] { "-l", @"test1,'test2,test3b','test 3'" });

            Assert.That(myOptions.List, Has.Count.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2,test3b"));
            Assert.That(myOptions.List[2], Is.EqualTo("test 3"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ListOptionQuotedInvalid1()
        {
            ListOptions myOptions = new ListOptions();
            Assert.That(() => {
                GetOptions(myOptions, new[] { "/l", @"test1:'test2:test3b':'test 3'x" }, new[] { "-l", @"test1,'test2,test3b','test 3'x" });
            }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void ListOptionWindowsPath()
        {
            ListOptions myOptions = new ListOptions();
            Options options = GetOptions(myOptions, new[] { "/l", @"c:\users\homeuser" }, new[] { "-l", @"c:\users\homeuser" });
            Assert.That(myOptions.List, Has.Count.EqualTo(1));
            Assert.That(myOptions.List[0], Is.EqualTo(@"c:\users\homeuser"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ListOptionQuotedEndEscape()
        {
            ListOptions myOptions = new ListOptions();
            Options options = GetOptions(myOptions, new[] { "/l", @"test1,test2,testx\" }, new[] { "-l", @"test1,test2,testx\" });
            Assert.That(myOptions.List, Has.Count.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo(@"testx\"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ListOptionQuotedInvalid3()
        {
            ListOptions myOptions = new ListOptions();
            Assert.That(() => {
                GetOptions(myOptions, new[] { "/l", "test1:test2:\"test3" }, new[] { "-l", "test1,test2,\"test3" });
            }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void ListOptionWithGenericStringInterface()
        {
            ListOptionsInterfaceGeneric myOptions = new ListOptionsInterfaceGeneric();
            Options options = GetOptions(myOptions, new[] { "/l", "item" }, new[] { "-l", "item" });
            Assert.That(myOptions.List, Has.Count.EqualTo(1));
            Assert.That(myOptions.List[0], Is.EqualTo("item"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ListOptionWithInterface()
        {
            ListOptionsInterface myOptions = new ListOptionsInterface();
            Options options = GetOptions(myOptions, new[] { "/l", "item" }, new[] { "-l", "item" });
            Assert.That(myOptions.List, Has.Count.EqualTo(1));
            Assert.That(myOptions.List[0], Is.EqualTo("item"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ListCollectionOptionWithGenericStringInterface()
        {
            CollectionOptionsInterfaceGeneric myOptions = new CollectionOptionsInterfaceGeneric();
            Options options = GetOptions(myOptions, new[] { "/l", "item" }, new[] { "-l", "item" });
            Assert.That(myOptions.List, Has.Count.EqualTo(1));
            Assert.That(myOptions.List, Is.EquivalentTo(new[] { "item" }));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ListCollectionOptionWithInterface()
        {
            CollectionOptionsInterface myOptions = new CollectionOptionsInterface();
            Assert.That(() => {
                // Fails, because method List of of type ICollection, which doesn't have an 'Add' method. Collections
                // must implement IList or ICollection<object>. So it's not seen as a list or a primitive type.
                _ = GetOptions(myOptions, new[] { "/l", "item" }, new[] { "-l", "item" });
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ListOptionWithInteger()
        {
            ListOptionsIntegers myOptions = new ListOptionsIntegers();
            Options options = GetOptions(myOptions, new[] { "/l", "1" }, new[] { "-l", "1" });
            Assert.That(myOptions.List, Has.Count.EqualTo(1));
            Assert.That(myOptions.List, Is.EqualTo(new[] { 1 }));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ListOptionWithIntegers()
        {
            ListOptionsIntegers myOptions = new ListOptionsIntegers();
            Options options = GetOptions(myOptions, new[] { "/l", "3,1,4" }, new[] { "-l", "3,1,4" });
            Assert.That(myOptions.List, Has.Count.EqualTo(3));
            Assert.That(myOptions.List, Is.EqualTo(new[] { 3, 1, 4 }));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void DefaultValueProvidedShort()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/a", "/b", "/v:3" }, new[] { "-abv3" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("3"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void DefaultValueProvidedShort2()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/a", "/b", "/v", "3" }, new[] { "-abv", "3" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("3"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void DefaultValueShortProvidedExistingOption()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/a", "/v:b" }, new[] { "-avb" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.Verbosity, Is.EqualTo("b"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void DefaultValueShort()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/a", "/b", "/v" }, new[] { "-abv" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("0"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void DefaultValueProvidedLongEquals()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/a", "/b", "/verbosity:2" }, new[] { "-ab", "--verbosity=2" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("2"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void DefaultValueProvidedLong()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/a", "/b", "/verbosity", "2" }, new[] { "-ab", "--verbosity", "2" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("2"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void DefaultValueLong()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/a", "/b", "/verbosity" }, new[] { "-ab", "--verbosity" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("0"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void DefaultValueLong2()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/verbosity", "/a", "/b" }, new[] { "--verbosity", "-ab" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("0"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void TypesEnum()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = GetOptions(myOptions, new[] { "/c", "yellow" }, new[] { "-c", "yellow" });

            Assert.That(myOptions.Color, Is.EqualTo(BasicColor.Yellow));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void TypesEnumInt()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = GetOptions(myOptions, new[] { "/c", "4" }, new[] { "-c", "4" });

            Assert.That(myOptions.Color, Is.EqualTo(BasicColor.Cyan));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void TypesInt()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = GetOptions(myOptions, new[] { "/O", "100" }, new[] { "-O", "100" });

            Assert.That(myOptions.Opacity, Is.EqualTo(100));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void TypesIntInvalid()
        {
            TypesOptions myOptions = new TypesOptions();
            Assert.That(() => {
                GetOptions(myOptions, new[] { "/O", "xxx" }, new[] { "-O", "xxx" });
            }, Throws.TypeOf<OptionFormatException>());
        }

        [Test]
        public void ArgumentsAttribute()
        {
            ArgumentsAttributeOptions myOptions = new ArgumentsAttributeOptions();
            Options options = GetOptions(myOptions, new[] { "/a", "arg1", "arg2" }, new[] { "-a", "arg1", "arg2" });

            Assert.That(options.Arguments, Has.Count.EqualTo(2));
            Assert.That(options.Arguments[0], Is.EqualTo("arg1"));
            Assert.That(options.Arguments[1], Is.EqualTo("arg2"));
            Assert.That(myOptions.Arguments, Has.Count.EqualTo(2));
            Assert.That(myOptions.Arguments[0], Is.EqualTo("arg1"));
            Assert.That(myOptions.Arguments[1], Is.EqualTo("arg2"));
        }

        [Test]
        public void ArgumentsAttributeList()
        {
            ArgumentsListGenericStringAttributeOptions myOptions = new ArgumentsListGenericStringAttributeOptions();
            Options options = GetOptions(myOptions, new[] { "arg1", "arg2" }, new[] { "arg1", "arg2" });

            Assert.That(options.Arguments, Has.Count.EqualTo(2));
            Assert.That(options.Arguments[0], Is.EqualTo("arg1"));
            Assert.That(options.Arguments[1], Is.EqualTo("arg2"));
            Assert.That(myOptions.Arguments, Has.Count.EqualTo(2));
            Assert.That(myOptions.Arguments[0], Is.EqualTo("arg1"));
            Assert.That(myOptions.Arguments[1], Is.EqualTo("arg2"));
        }

        [Test]
        public void ArgumentsAttributeListInt()
        {
            ArgumentsListCollGenericIntAttributeOptions myOptions = new ArgumentsListCollGenericIntAttributeOptions();
            Assert.That(() => {
                _ = GetOptions(myOptions, new[] { "1", "2" }, new[] { "1", "2" });
            }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void ArgumentsAttributeListIntInvalidType()
        {
            ArgumentsListCollGenericIntAttributeOptions myOptions = new ArgumentsListCollGenericIntAttributeOptions();
            Assert.That(() => {
                _ = GetOptions(myOptions, new[] { "arg1", "arg2" }, new[] { "arg1", "arg2" });
            }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void ArgumentsAttributeListCollection()
        {
            ArgumentsListCollGenericStringAttributeOptions myOptions = new ArgumentsListCollGenericStringAttributeOptions();
            Options options = GetOptions(myOptions, new[] { "arg1", "arg2" }, new[] { "arg1", "arg2" });

            Assert.That(options.Arguments, Has.Count.EqualTo(2));
            Assert.That(options.Arguments[0], Is.EqualTo("arg1"));
            Assert.That(options.Arguments[1], Is.EqualTo("arg2"));
            Assert.That(myOptions.Arguments, Has.Count.EqualTo(2));
            Assert.That(myOptions.Arguments, Is.EquivalentTo(new[] { "arg1", "arg2" }));
        }

        [Test]
        public void ArgumentsAttributeListNonGeneric()
        {
            ArgumentsListAttributeOptions myOptions = new ArgumentsListAttributeOptions();
            Options options = GetOptions(myOptions, new[] { "arg1", "arg2" }, new[] { "arg1", "arg2" });

            Assert.That(options.Arguments, Has.Count.EqualTo(2));
            Assert.That(options.Arguments[0], Is.EqualTo("arg1"));
            Assert.That(options.Arguments[1], Is.EqualTo("arg2"));
            Assert.That(myOptions.Arguments, Has.Count.EqualTo(2));
            Assert.That(myOptions.Arguments[0], Is.EqualTo("arg1"));
            Assert.That(myOptions.Arguments[1], Is.EqualTo("arg2"));
        }

        [Test]
        public void ArgumentsAttributeListCollNonGeneric()
        {
            ArgumentsListCollAttributeOptions myOptions = new ArgumentsListCollAttributeOptions();
            Assert.That(() => {
                _ = GetOptions(myOptions, new[] { "arg1", "arg2" }, new[] { "arg1", "arg2" });
            }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void DerivedOptionsPrivate1()
        {
            DerivedOptionsPrivate myOptions = new DerivedOptionsPrivate();
            Options options = GetOptions(myOptions, new[] { "/a" }, new[] { "-a" });

            Assert.That(options, Is.Not.Null);
            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.False);
        }

        [Test]
        public void DerivedOptionsPrivate2()
        {
            DerivedOptionsPrivate myOptions = new DerivedOptionsPrivate();
            Options options = GetOptions(myOptions, new[] { "/b" }, new[] { "-b" });

            Assert.That(options, Is.Not.Null);
            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.True);
        }

        [Test]
        public void DerivedOptionsProtected1()
        {
            DerivedOptionsProtected myOptions = new DerivedOptionsProtected();
            Options options = GetOptions(myOptions, new[] { "/a" }, new[] { "-a" });

            Assert.That(options, Is.Not.Null);
            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.False);
        }

        [Test]
        public void DerivedOptionsProtected2()
        {
            DerivedOptionsProtected myOptions = new DerivedOptionsProtected();
            Options options = GetOptions(myOptions, new[] { "/b" }, new[] { "-b" });

            Assert.That(options, Is.Not.Null);
            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.True);
        }

        [Test]
        public void DerivedOptionsProtectedList1()
        {
            DerivedOptionsProtectedList myOptions = new DerivedOptionsProtectedList();
            Options options = GetOptions(myOptions, new[] { "/a", "foo,bar" }, new[] { "-a", "foo,bar" });

            Assert.That(options, Is.Not.Null);
            Assert.That(myOptions.OptionA, Has.Count.EqualTo(2));
            Assert.That(myOptions.OptionB, Is.Empty);
        }

        [Test]
        public void DerivedOptionsProtectedList2()
        {
            DerivedOptionsProtectedList myOptions = new DerivedOptionsProtectedList();
            Options options = GetOptions(myOptions, new[] { "/b", "foo,bar" }, new[] { "-b", "foo,bar" });

            Assert.That(options, Is.Not.Null);
            Assert.That(myOptions.OptionA, Is.Empty);
            Assert.That(myOptions.OptionB, Has.Count.EqualTo(2));
        }

        [Test]
        public void DuplicateShortOptionsNoArgs()
        {
            DuplicateOptionsShort options = new DuplicateOptionsShort();
            Assert.That(() => {
                _ = GetOptions(options, new string[0], new string[0]);
            }, Throws.TypeOf<OptionDuplicateException>());
        }

        [Test]
        public void DuplicateShortOptionsWithArgs()
        {
            DuplicateOptionsShort options = new DuplicateOptionsShort();
            Assert.That(() => {
                _ = GetOptions(options, new[] { "/a" }, new[] { "-a" });
            }, Throws.TypeOf<OptionDuplicateException>());
        }

        [Test]
        public void DuplicateLongOptionsNoArgs()
        {
            DuplicateOptionsLong options = new DuplicateOptionsLong();
            Assert.That(() => {
                _ = GetOptions(options, new string[0], new string[0]);
            }, Throws.TypeOf<OptionDuplicateException>());
        }

        [Test]
        public void DuplicateLongOptionsWithArgs()
        {
            DuplicateOptionsLong options = new DuplicateOptionsLong();
            Assert.That(() => {
                _ = GetOptions(options, new[] { "/along" }, new[] { "--along" });
            }, Throws.TypeOf<OptionDuplicateException>());
        }

        [Test]
        public void AllShortSymbols()
        {
            OptionShortSymbol options = new OptionShortSymbol();
            _ = GetOptions(options, new string[] { }, new string[] { });
        }

        [Test]
        public void ShortSymbolPlus()
        {
            OptionShortPlus options = new OptionShortPlus();
            Assert.That(() => {
                _ = GetOptions(options, new string[] { "/+" }, new string[] { "-+" });
            }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void ShortSymbolMinus()
        {
            OptionShortMinus options = new OptionShortMinus();
            Assert.That(() => {
                _ = GetOptions(options, new string[] { "/-" }, new string[] { "--" });
            }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void ShortSymbolUnder()
        {
            OptionShortUnder options = new OptionShortUnder();
            Assert.That(() => {
                _ = GetOptions(options, new string[] { "/_" }, new string[] { "-_" });
            }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void ShortSymbolHash()
        {
            OptionShortHash options = new OptionShortHash();
            _ = GetOptions(options, new string[] { "/#" }, new string[] { "-#" });
            Assert.That(options.Hash, Is.True);
        }

        [Test]
        public void ShortSymbolBang()
        {
            OptionShortBang options = new OptionShortBang();
            _ = GetOptions(options, new string[] { "/!" }, new string[] { "-!" });
            Assert.That(options.Bang, Is.True);
        }

        [Test]
        public void ShortSymbolHelp()
        {
            OptionShortHelp options = new OptionShortHelp();
            _ = GetOptions(options, new string[] { "/?" }, new string[] { "-?" });
            Assert.That(options.Help, Is.True);
        }

        [Test]
        public void ShortSymbolInvalid()
        {
            OptionShortStar options = new OptionShortStar();
            Assert.That(() => {
                _ = GetOptions(options, new string[] { "/*" }, new string[] { "-*" });
            }, Throws.TypeOf<OptionException>());
        }

        [Test]
        public void OptionSetPropagateException()
        {
            OptionPropertySetRaiseError options = new OptionPropertySetRaiseError();
            Assert.That(() => {
                _ = GetOptions(options, new[] { "/value:-10" }, new[] { "--value=-10" });
            }, Throws.TypeOf<OptionException>().With.Message.EqualTo("Value out of range"));
        }

        [Test]
        public void OptionSetGetterOnlyClass()
        {
            OptionOnlyGetterClass options = new OptionOnlyGetterClass();
            Assert.That(() => {
                _ = GetOptions(options, new[] { "/x" }, new[] { "-x" });
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void OptionSetGetterOnlyStruct()
        {
            OptionOnlyGetterStruct options = new OptionOnlyGetterStruct();
            Assert.That(() => {
                _ = GetOptions(options, new[] { "/x" }, new[] { "-x" });
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void OptionSetReadOnlyStruct()
        {
            OptionOnlyReadOnlyStruct options = new OptionOnlyReadOnlyStruct();
            Assert.That(() => {
                _ = GetOptions(options, new[] { "/x" }, new[] { "-x" });
            }, Throws.TypeOf<ArgumentException>());
        }
    }
}
