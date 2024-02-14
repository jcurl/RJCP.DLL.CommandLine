namespace RJCP.Core.Terminal
{
    using System;
    using NUnit.Framework;
    using RJCP.Core.Terminal.Log;

    [TestFixture]
    internal class ConsoleTerminalTest
    {
        [Test]
        public void WriteConsoleDefault()
        {
            ConsoleTerminal console = new();
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
            if (!TestOsColoursFirst())
                Assert.Ignore("OS Console Colour has undefined behaviour.");

            ConsoleTerminal console = new();
            ConsoleColor foreground = console.ForegroundColor;
            ConsoleColor background = console.BackgroundColor;
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

        private static bool TestOsColoursFirst()
        {
#if NET45_OR_GREATER || NET6_0_OR_GREATER
            // We don't test the System.Console implementation as we're redirecting. Then we
            // have our own shadow copy.
            if (Console.IsOutputRedirected) {
                Console.WriteLine("Redirection enabled, so ignoring OS test");
                return true;
            }
#endif

            // Mono sources have -1 as an "UnknownColor" when tracking.
            // See https://github.com/mono/mono/blob/38b0227c1ce0c53058a5d78d080923435132773a/mcs/class/corlib/System/Console.cs#L787
            if (!Enum.IsDefined(typeof(ConsoleColor), Console.ForegroundColor)) return false;
            if (!Enum.IsDefined(typeof(ConsoleColor), Console.BackgroundColor)) return false;

            // Just make sure we can set it and it isn't ignored when not redirecting.
            ConsoleColor foreground = Console.ForegroundColor;
            ConsoleColor background = Console.BackgroundColor;
            try {
                foreach (ConsoleColor newFg in Enum.GetValues(typeof(ConsoleColor))) {
                    Console.ForegroundColor = newFg;
                    if (Console.ForegroundColor != newFg)
                        return false;
                }
            } finally {
                Console.ForegroundColor = foreground;
                Console.BackgroundColor = background;
            }
            return true;
        }

        [Test]
        public void WriteConsole()
        {
            ConsoleTerminal console = new();
            console.StdOut.Write("This is a line");
        }

        [Test]
        public void WriteConsoleErr()
        {
            ConsoleTerminal console = new();
            console.StdErr.Write("This is an error line");
        }

        [Test]
        public void WriteLineConsole()
        {
            ConsoleTerminal console = new();
            console.StdOut.WriteLine("This is a line");
        }

        [Test]
        public void WriteLineConsoleErr()
        {
            ConsoleTerminal console = new();
            console.StdErr.WriteLine("This is an error line");
        }

        [Test]
        public void WrapLineConsole()
        {
            ConsoleTerminal console = new();
            console.StdOut.WrapLine(
                "This is a line that should be more than eighty-characters long, so that " +
                "it can be checked if the line is really being wrapped into multiple lines. " +
                "The proper test will need to be done using a virtual console so that we can " +
                "check the precise behaviour of wrapping.");
        }

        [Test]
        public void WriteEvent()
        {
            ConsoleTerminal console = new();

            TerminalWriteEventArgs args = null;
            console.ConsoleWriteEvent += (s, e) => {
                args = e;
            };

            console.StdOut.Write("Line");
            Assert.That(args, Is.Not.Null);
            Assert.That(args.Channel, Is.EqualTo(ConsoleLogChannel.StdOut));
            Assert.That(args.NewLine, Is.False);
            Assert.That(args.Line, Is.EqualTo("Line"));

            console.StdErr.Write("Line Error");
            Assert.That(args, Is.Not.Null);
            Assert.That(args.Channel, Is.EqualTo(ConsoleLogChannel.StdErr));
            Assert.That(args.NewLine, Is.False);
            Assert.That(args.Line, Is.EqualTo("Line Error"));
        }

        [Test]
        public void WriteLineEvent()
        {
            ConsoleTerminal console = new();

            TerminalWriteEventArgs args = null;
            console.ConsoleWriteEvent += (s, e) => {
                args = e;
            };

            console.StdOut.WriteLine("Line");
            Assert.That(args, Is.Not.Null);
            Assert.That(args.Channel, Is.EqualTo(ConsoleLogChannel.StdOut));
            Assert.That(args.NewLine, Is.True);
            Assert.That(args.Line, Is.EqualTo("Line"));

            console.StdErr.WriteLine("Line Error");
            Assert.That(args, Is.Not.Null);
            Assert.That(args.Channel, Is.EqualTo(ConsoleLogChannel.StdErr));
            Assert.That(args.NewLine, Is.True);
            Assert.That(args.Line, Is.EqualTo("Line Error"));
        }

        [Test]
        public void WrapLineEvent()
        {
            ConsoleTerminal console = new();

            TerminalWriteEventArgs args = null;
            console.ConsoleWriteEvent += (s, e) => {
                args = e;
            };

            console.StdOut.WrapLine("This is a Line");
            Assert.That(args, Is.Not.Null);
            Assert.That(args.Channel, Is.EqualTo(ConsoleLogChannel.StdOut));
            Assert.That(args.NewLine, Is.True);
            Assert.That(args.Line, Is.Not.Null.And.Length.GreaterThan(0));

            console.StdErr.WriteLine("This is a Line for an Error");
            Assert.That(args, Is.Not.Null);
            Assert.That(args.Channel, Is.EqualTo(ConsoleLogChannel.StdErr));
            Assert.That(args.NewLine, Is.True);
            Assert.That(args.Line, Is.Not.Null.And.Length.GreaterThan(0));
        }
    }
}
