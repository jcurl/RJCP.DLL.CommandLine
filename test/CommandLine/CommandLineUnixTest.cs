﻿namespace RJCP.Core.CommandLine
{
    using NUnit.Framework;

    [TestFixture]
    public class CommandLineUnixTest
    {
        [Test]
        public void ShortOptionsStringOneArgJoined()
        {
            OptionalArguments myOptions = new();
            Options options = Options.Parse(myOptions, new[] { "-abcfoo" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ShortOptionsStringTwoArgsJoined1()
        {
            OptionalArguments myOptions = new();
            Options options = Options.Parse(myOptions, new[] { "-abc", "foo" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.EqualTo("foo"));
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void ShortOptionsStringTwoArgsJoined2()
        {
            OptionalArguments myOptions = new();
            Options options = Options.Parse(myOptions, new[] { "-cab", "foo" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.False);
            Assert.That(myOptions.OptionB, Is.False);
            Assert.That(myOptions.OptionC, Is.EqualTo("ab"));
            Assert.That(options.Arguments, Has.Count.EqualTo(1));
            Assert.That(options.Arguments[0], Is.EqualTo("foo"));
        }

        [Test]
        public void StopParsing()
        {
            OptionalArguments myOptions = new();
            Options options = Options.Parse(myOptions, new[] { "-abc", "argument", "--", "-c" }, OptionsStyle.Unix);

            Assert.That(myOptions.OptionA, Is.True);
            Assert.That(myOptions.OptionB, Is.True);
            Assert.That(myOptions.OptionC, Is.EqualTo("argument"));
            Assert.That(options.Arguments, Has.Count.EqualTo(1));
            Assert.That(options.Arguments[0], Is.EqualTo("-c"));
        }
    }
}
