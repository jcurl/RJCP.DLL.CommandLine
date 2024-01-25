namespace RJCP.Core.Terminal
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    internal class ConsoleTerminalTest
    {
        [Test]
        public void WriteConsoleDefault()
        {
            ITerminal console = new ConsoleTerminal();
            Assert.That(console.StdOut, Is.Not.Null);
            Assert.That(console.StdErr, Is.Not.Null);

            if (!Console.IsOutputRedirected) {
                Console.WriteLine("Using Console - Testing Console");
                // We're in a console. So check against the terminal settings.
                Assert.That(console.Width, Is.EqualTo(Console.BufferWidth));
                Assert.That(console.Height, Is.EqualTo(Console.WindowHeight));
                Assert.That(console.BackgroundColor, Is.EqualTo(Console.BackgroundColor));
                Assert.That(console.ForegroundColor, Is.EqualTo(Console.ForegroundColor));
            } else {
                Console.WriteLine("Redirected - Testing Defaults");
                // We're redirecting (e.g. Visual Studio environment).
                Assert.That(console.Width, Is.EqualTo(80));
                Assert.That(console.Height, Is.EqualTo(25));
                Assert.That(console.BackgroundColor, Is.EqualTo(ConsoleColor.Black));
                Assert.That(console.ForegroundColor, Is.EqualTo(ConsoleColor.Gray));
            }
        }

        [Test]
        public void ConsoleColors()
        {
            ITerminal console = new ConsoleTerminal();
            ConsoleColor foreground = Console.ForegroundColor;
            ConsoleColor background = Console.BackgroundColor;
            Assert.That(console.ForegroundColor, Is.EqualTo(foreground));
            Assert.That(console.BackgroundColor, Is.EqualTo(background));

            console.ForegroundColor = ConsoleColor.White;
            console.BackgroundColor = ConsoleColor.Blue;

            Assert.That(console.ForegroundColor, Is.EqualTo(ConsoleColor.White));
            Assert.That(console.BackgroundColor, Is.EqualTo(ConsoleColor.Blue));

            console.StdOut.WriteLine("Testing Console");

            console.ForegroundColor = foreground;
            console.BackgroundColor = background;

            Assert.That(console.ForegroundColor, Is.EqualTo(foreground));
            Assert.That(console.BackgroundColor, Is.EqualTo(background));
        }

        [Test]
        public void WriteConsole()
        {
            ITerminal console = new ConsoleTerminal();
            console.StdOut.Write("This is a line");
        }

        [Test]
        public void WriteConsoleErr()
        {
            ITerminal console = new ConsoleTerminal();
            console.StdErr.Write("This is an error line");
        }

        [Test]
        public void WriteLineConsole()
        {
            ITerminal console = new ConsoleTerminal();
            console.StdOut.WriteLine("This is a line");
        }

        [Test]
        public void WriteLineConsoleErr()
        {
            ITerminal console = new ConsoleTerminal();
            console.StdErr.WriteLine("This is an error line");
        }

        [Test]
        public void WrapLineConsole()
        {
            ITerminal console = new ConsoleTerminal();
            console.StdOut.WrapLine(
                "This is a line that should be more than eighty-characters long, so that " +
                "it can be checked if the line is really being wrapped into multiple lines. " +
                "The proper test will need to be done using a virtual console so that we can " +
                "check the precise behaviour of wrapping.");
        }
    }
}
