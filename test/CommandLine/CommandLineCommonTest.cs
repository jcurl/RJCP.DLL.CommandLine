namespace RJCP.Core.CommandLine
{
    using System;
    using NUnit.Framework;
    using RJCP.Core.Environment;

    [TestFixture]
    public class CommandLineCommonTest
    {
        [Test]
        public void NullOptions()
        {
            Assert.That(() => { Options.Parse(null, null); }, Throws.TypeOf<ArgumentNullException>());
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
