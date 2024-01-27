namespace RJCP.Core.CommandLine
{
    using NUnit.Framework;
    using RJCP.Core.Environment;

    [TestFixture]
    public class CommandLineCommonTest
    {
        [Test]
        public void NullOptions()
        {
            Options options = Options.Parse(null, null);
            Assert.That(options, Is.Not.Null);
        }

        [Test]
        public void NullOptionsWithArgs()
        {
            Options options = Options.Parse(null, new string[] { "arg" });
            Assert.That(options, Is.Not.Null);
            Assert.That(options.Arguments, Is.Empty);
        }

        [Test]
        public void NullArguments()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Assert.That(() => { _ = Options.Parse(myOptions, null); }, Throws.Nothing);
        }

        [Test]
        public void OptionsType_Windows()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, null);
            if (Platform.IsMSys() || Platform.IsUnix()) {
                Assert.That(options.OptionsStyle, Is.EqualTo(OptionsStyle.Unix));
            } else {
                Assert.That(options.OptionsStyle, Is.EqualTo(OptionsStyle.Windows));
            }
        }
    }
}
