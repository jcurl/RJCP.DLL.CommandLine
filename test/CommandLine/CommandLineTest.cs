﻿namespace RJCP.Core.CommandLine
{
    using System;
    using NUnit.Framework;

    [TestFixture(OptionsStyle.Unix, Category = "Utilities.CommandLine")]
    [TestFixture(OptionsStyle.Windows, Category = "Utilities.CommandLine")]
    public class CommandLineTest
    {
        public CommandLineTest(OptionsStyle style)
        {
            CommandLineStyle = style;
        }

        public OptionsStyle CommandLineStyle { get; private set; }

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
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void NoArguments()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, new string[0], CommandLineStyle);
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void NullArgumentsEmptyOptions()
        {
            NoArguments myOptions = new NoArguments();
            Options options = Options.Parse(myOptions, null, CommandLineStyle);
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void NoArgumentsEmptyOptions()
        {
            NoArguments myOptions = new NoArguments();
            Options options = Options.Parse(myOptions, new string[0], CommandLineStyle);
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void SingleShortOption()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = GetOptions(myOptions, new[] { "/a" }, new[] { "-a" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.Null);
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
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
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ShortOptionsSeparated()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = GetOptions(myOptions, new[] { "/a", "/b" }, new[] { "-a", "-b" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.Null);
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ShortOptionsOnProperties()
        {
            PropertyOptions myOptions = new PropertyOptions();
            Options options = GetOptions(myOptions, new[] { "/a", "/b" }, new[] { "-a", "-b" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.Null);
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ShortOptionsStringOneArg()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = GetOptions(myOptions, new[] { "/c:foo" }, new[] { "-cfoo" });

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ShortOptionsStringOneArgAssigned()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = GetOptions(myOptions, new[] { "/c:foo" }, new[] { "-c=foo" });

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ShortOptionsStringTwoArgs()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = GetOptions(myOptions, new[] { "/c", "foo" }, new[] { "-c", "foo" });

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ShortOptionsRequired()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/i", "/f", "/s", "string" }, new[] { "-ifs", "string" });

            Assert.That(myOptions.CaseInsensitive, Is.True);
            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ShortOptionsRequiredOneArgument1()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/i", "/f", "/s:string" }, new[] { "-ifsstring" });

            Assert.That(myOptions.CaseInsensitive, Is.True);
            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ShortOptionsRequiredOneArgument2()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/i", "/f", "/s:string" }, new[] { "-if", "-sstring" });

            Assert.That(myOptions.CaseInsensitive, Is.True);
            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ShortOptionsRequiredOneArgument3()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/i", "/f", "/s:string" }, new[] { "-if", "-s=string" });

            Assert.That(myOptions.CaseInsensitive, Is.True);
            Assert.That(myOptions.PrintFiles, Is.True);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
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
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void LongOptionOneArgument()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search:string" }, new[] { "--search=string" });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void LongOptionTwoArguments()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search", "string" }, new[] { "--search", "string" });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("string"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void LongOptionWithSpace()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search: string " }, new[] { "--search= string " });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo(" string "));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void LongOptionWithSpace2()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search: string" }, new[] { "--search= string" });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo(" string"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void LongOptionWithSpace3()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search:string " }, new[] { "--search=string " });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo("string "));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void LongOptionWithSpace4()
        {
            RequiredArguments myOptions = new RequiredArguments();
            Options options = GetOptions(myOptions, new[] { "/search", " string " }, new[] { "--search", " string " });

            Assert.That(myOptions.CaseInsensitive, Is.False);
            Assert.That(myOptions.PrintFiles, Is.False);
            Assert.That(myOptions.SearchString, Is.EqualTo(" string "));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
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
            Assert.That(options.Arguments.Count, Is.EqualTo(1));
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
            Assert.That(options.Arguments.Count, Is.EqualTo(1));
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
            Assert.That(options.Arguments.Count, Is.EqualTo(1));
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
            Assert.That(options.Arguments.Count, Is.EqualTo(1));
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
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
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
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
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
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
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

            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo("test3"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ListOptionSingleElement()
        {
            ListOptions myOptions = new ListOptions();
            Options options = GetOptions(myOptions, new[] { "/l", "test1" }, new[] { "-l", "test1" });

            Assert.That(myOptions.List.Count, Is.EqualTo(1));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ListOptionMultipleArguments()
        {
            ListOptions myOptions = new ListOptions();
            Options options = GetOptions(myOptions, new[] { "/l", "test1", "/l", "test2", "/l", "test3" }, new[] { "-l", "test1", "-l", "test2", "-l", "test3" });

            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo("test3"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ListOptionQuoted1()
        {
            ListOptions myOptions = new ListOptions();
            Options options = GetOptions(myOptions, new[] { "/l", @"test1,'test2','test 3'" }, new[] { "-l", @"test1,'test2','test 3'" });

            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo("test 3"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ListOptionQuoted2()
        {
            ListOptions myOptions = new ListOptions();
            Options options = GetOptions(myOptions, new[] { "/l", @"test1,'test2,test3b','test 3'" }, new[] { "-l", @"test1,'test2,test3b','test 3'" });

            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2,test3b"));
            Assert.That(myOptions.List[2], Is.EqualTo("test 3"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
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
            Assert.That(myOptions.List.Count, Is.EqualTo(1));
            Assert.That(myOptions.List[0], Is.EqualTo(@"c:\users\homeuser"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ListOptionQuotedEndEscape()
        {
            ListOptions myOptions = new ListOptions();
            Options options = GetOptions(myOptions, new[] { "/l", @"test1,test2,testx\" }, new[] { "-l", @"test1,test2,testx\" });
            Assert.That(myOptions.List.Count, Is.EqualTo(3));
            Assert.That(myOptions.List[0], Is.EqualTo("test1"));
            Assert.That(myOptions.List[1], Is.EqualTo("test2"));
            Assert.That(myOptions.List[2], Is.EqualTo(@"testx\"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
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
        public void DefaultValueProvidedShort()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/a", "/b", "/v:3" }, new[] { "-abv3" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("3"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void DefaultValueProvidedShort2()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/a", "/b", "/v", "3" }, new[] { "-abv", "3" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("3"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void DefaultValueShortProvidedExistingOption()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/a", "/v:b" }, new[] { "-avb" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.Verbosity, Is.EqualTo("b"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void DefaultValueShort()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/a", "/b", "/v" }, new[] { "-abv" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("0"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void DefaultValueProvidedLongEquals()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/a", "/b", "/verbosity:2" }, new[] { "-ab", "--verbosity=2" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("2"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void DefaultValueProvidedLong()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/a", "/b", "/verbosity", "2" }, new[] { "-ab", "--verbosity", "2" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("2"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void DefaultValueLong()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/a", "/b", "/verbosity" }, new[] { "-ab", "--verbosity" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("0"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void DefaultValueLong2()
        {
            DefaultValueOption myOptions = new DefaultValueOption();
            Options options = GetOptions(myOptions, new[] { "/verbosity", "/a", "/b" }, new[] { "--verbosity", "-ab" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.Verbosity, Is.EqualTo("0"));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void TypesEnum()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = GetOptions(myOptions, new[] { "/c", "yellow" }, new[] { "-c", "yellow" });

            Assert.That(myOptions.Color, Is.EqualTo(BasicColor.Yellow));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void TypesEnumInt()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = GetOptions(myOptions, new[] { "/c", "4" }, new[] { "-c", "4" });

            Assert.That(myOptions.Color, Is.EqualTo(BasicColor.Cyan));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void TypesInt()
        {
            TypesOptions myOptions = new TypesOptions();
            Options options = GetOptions(myOptions, new[] { "/O", "100" }, new[] { "-O", "100" });

            Assert.That(myOptions.Opacity, Is.EqualTo(100));
            Assert.That(options.Arguments.Count, Is.EqualTo(0));
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

            Assert.That(options.Arguments.Count, Is.EqualTo(2));
            Assert.That(options.Arguments[0], Is.EqualTo("arg1"));
            Assert.That(options.Arguments[1], Is.EqualTo("arg2"));
            Assert.That(myOptions.Arguments.Count, Is.EqualTo(2));
            Assert.That(myOptions.Arguments[0], Is.EqualTo("arg1"));
            Assert.That(myOptions.Arguments[1], Is.EqualTo("arg2"));
        }

        [Test]
        public void DerivedOptionsPrivate1()
        {
            DerivedOptionsPrivate myOptions = new DerivedOptionsPrivate();
            Options options = GetOptions(myOptions, new[] { "/a" }, new[] { "-a" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.False);
        }

        [Test]
        public void DerivedOptionsPrivate2()
        {
            DerivedOptionsPrivate myOptions = new DerivedOptionsPrivate();
            Options options = GetOptions(myOptions, new[] { "/b" }, new[] { "-b" });

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.True);
        }

        [Test]
        public void DerivedOptionsProtected1()
        {
            DerivedOptionsProtected myOptions = new DerivedOptionsProtected();
            Options options = GetOptions(myOptions, new[] { "/a" }, new[] { "-a" });

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.False);
        }

        [Test]
        public void DerivedOptionsProtected2()
        {
            DerivedOptionsProtected myOptions = new DerivedOptionsProtected();
            Options options = GetOptions(myOptions, new[] { "/b" }, new[] { "-b" });

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.True);
        }

        [Test]
        public void DerivedOptionsProtectedList1()
        {
            DerivedOptionsProtectedList myOptions = new DerivedOptionsProtectedList();
            Options options = GetOptions(myOptions, new[] { "/a", "foo,bar" }, new[] { "-a", "foo,bar" });

            Assert.That(myOptions.OptionA.Count, Is.EqualTo(2));
            Assert.That(myOptions.OptionB.Count, Is.EqualTo(0));
        }

        [Test]
        public void DerivedOptionsProtectedList2()
        {
            DerivedOptionsProtectedList myOptions = new DerivedOptionsProtectedList();
            Options options = GetOptions(myOptions, new[] { "/b", "foo,bar" }, new[] { "-b", "foo,bar" });

            Assert.That(myOptions.OptionA.Count, Is.EqualTo(0));
            Assert.That(myOptions.OptionB.Count, Is.EqualTo(2));
        }
    }
}