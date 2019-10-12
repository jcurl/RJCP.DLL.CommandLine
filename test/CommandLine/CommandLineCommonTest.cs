namespace RJCP.Core.CommandLine
{
    using System;
    using NUnit.Framework;

    [TestFixture(Category = "Utilities.CommandLine")]
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
        [Platform(Include = "Unix")]
        public void OptionsType_Unix()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, null);
            Assert.That(options.OptionsStyle, Is.EqualTo(OptionsStyle.Unix));
        }

        [Test]
        [Platform(Include = "Win")]
        public void OptionsType_Windows()
        {
            OptionalArguments myOptions = new OptionalArguments();
            Options options = Options.Parse(myOptions, null);
            Assert.That(options.OptionsStyle, Is.EqualTo(OptionsStyle.Windows));
        }
    }
}
