namespace RJCP.Core.Terminal
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    internal class VirtualTerminalTest
    {
        [Test]
        public void TerminalDefault()
        {
            VirtualTerminal terminal = new();
            Assert.That(terminal.Width, Is.EqualTo(80));
            Assert.That(terminal.Height, Is.EqualTo(25));
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(0));
            Assert.That(terminal.StdErrLines, Has.Count.EqualTo(0));
            Assert.That(terminal.ForegroundColor, Is.EqualTo(ConsoleColor.Gray));
            Assert.That(terminal.BackgroundColor, Is.EqualTo(ConsoleColor.Black));
            Assert.That(terminal.StdErr, Is.Not.Null);
            Assert.That(terminal.StdOut, Is.Not.Null);
        }

        [Test]
        public void ConsoleColors()
        {
            VirtualTerminal terminal = new();
            ConsoleColor foreground = terminal.ForegroundColor;
            ConsoleColor background = terminal.BackgroundColor;

            terminal.ForegroundColor = ConsoleColor.White;
            terminal.BackgroundColor = ConsoleColor.Blue;

            Assert.That(terminal.ForegroundColor, Is.EqualTo(ConsoleColor.White));
            Assert.That(terminal.BackgroundColor, Is.EqualTo(ConsoleColor.Blue));

            terminal.StdOut.WriteLine("Testing Console");

            terminal.ForegroundColor = foreground;
            terminal.BackgroundColor = background;

            Assert.That(terminal.ForegroundColor, Is.EqualTo(foreground));
            Assert.That(terminal.BackgroundColor, Is.EqualTo(background));
        }

        [TestCase(40)]
        [TestCase(1)]
        public void ConsoleWidthSet(int width)
        {
            VirtualTerminal terminal = new() {
                Width = width
            };
            Assert.That(terminal.Width, Is.EqualTo(width));
        }

        [TestCase(60)]
        [TestCase(1)]
        public void ConsoleHeightSet(int height)
        {
            VirtualTerminal terminal = new() {
                Height = height
            };
            Assert.That(terminal.Height, Is.EqualTo(height));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void ConsoleWidthInvalid(int width)
        {
            VirtualTerminal terminal = new();
            Assert.That(() => {
                terminal.Width = width;
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void ConsoleHeightInvalid(int height)
        {
            VirtualTerminal terminal = new();
            Assert.That(() => {
                terminal.Height = height;
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void WriteConsole()
        {
            VirtualTerminal terminal = new();
            terminal.StdOut.Write("This is a line");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(1));
            Assert.That(terminal.StdErrLines, Has.Count.EqualTo(0));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line"));

            terminal.StdOut.Write(". Continuing..");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(1));
            Assert.That(terminal.StdErrLines, Has.Count.EqualTo(0));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line. Continuing.."));
        }

        [Test]
        public void WriteConsoleErr()
        {
            VirtualTerminal terminal = new();
            terminal.StdErr.Write("This is an error line");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(0));
            Assert.That(terminal.StdErrLines, Has.Count.EqualTo(1));
            Assert.That(terminal.StdErrLines[0], Is.EqualTo("This is an error line"));

            terminal.StdErr.Write(". Continuing..");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(0));
            Assert.That(terminal.StdErrLines, Has.Count.EqualTo(1));
            Assert.That(terminal.StdErrLines[0], Is.EqualTo("This is an error line. Continuing.."));
        }

        [Test]
        public void WriteLineConsole()
        {
            VirtualTerminal terminal = new();
            terminal.StdOut.WriteLine("This is a line");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(1));
            Assert.That(terminal.StdErrLines, Has.Count.EqualTo(0));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line"));

            terminal.StdOut.Write(". Continuing..");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(2));
            Assert.That(terminal.StdErrLines, Has.Count.EqualTo(0));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo(". Continuing.."));
        }

        [Test]
        public void WriteLineConsoleErr()
        {
            VirtualTerminal terminal = new();
            terminal.StdErr.WriteLine("This is an error line");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(0));
            Assert.That(terminal.StdErrLines, Has.Count.EqualTo(1));
            Assert.That(terminal.StdErrLines[0], Is.EqualTo("This is an error line"));

            terminal.StdErr.Write(". Continuing..");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(0));
            Assert.That(terminal.StdErrLines, Has.Count.EqualTo(2));
            Assert.That(terminal.StdErrLines[0], Is.EqualTo("This is an error line"));
            Assert.That(terminal.StdErrLines[1], Is.EqualTo(". Continuing.."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WriteWithNewLine(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.Write($"This is a line with{sep}newlines{sep}in it.{sep}{sep}Thank you. ");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(5));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line with"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("newlines"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("in it."));
            Assert.That(terminal.StdOutLines[3], Is.Empty);
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("Thank you. "));

            terminal.StdOut.WriteLine("Cheers.");
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("Thank you. Cheers."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WriteLineWithNewLine(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WriteLine($"This is a line with{sep}newlines{sep}in it.{sep}{sep}Thank you.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(5));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line with"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("newlines"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("in it."));
            Assert.That(terminal.StdOutLines[3], Is.Empty);
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("Thank you."));
        }

        [TestCase("")]
        [TestCase(null)]
        public void WriteEmpty(string line)
        {
            VirtualTerminal terminal = new();
            terminal.StdOut.Write(line);
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(0));
        }

        [TestCase("")]
        [TestCase(null)]
        public void WriteLineEmpty(string line)
        {
            VirtualTerminal terminal = new();
            terminal.StdOut.WriteLine(line);
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(1));
            Assert.That(terminal.StdOutLines[0], Is.Empty);
        }

        [TestCase("")]
        [TestCase(null)]
        public void WrapLineEmpty(string line)
        {
            VirtualTerminal terminal = new();
            terminal.StdOut.WrapLine(line);
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(1));
            Assert.That(terminal.StdOutLines[0], Is.Empty);
        }

        [TestCase("")]
        [TestCase(null)]
        public void WriteEmptyMiddle(string line)
        {
            VirtualTerminal terminal = new();
            terminal.StdOut.Write("Lorem ipsum dolor sit amet, ");
            terminal.StdOut.Write(line);
            terminal.StdOut.Write("duo accumsan scripserit ad");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(1));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("Lorem ipsum dolor sit amet, duo accumsan scripserit ad"));
        }

        [TestCase("")]
        [TestCase(null)]
        public void WriteLineEmptyMiddle(string line)
        {
            VirtualTerminal terminal = new();
            terminal.StdOut.WriteLine("Lorem ipsum dolor sit amet, ");
            terminal.StdOut.WriteLine(line);
            terminal.StdOut.WriteLine("duo accumsan scripserit ad");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(3));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("Lorem ipsum dolor sit amet, "));
            Assert.That(terminal.StdOutLines[1], Is.Empty);
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("duo accumsan scripserit ad"));
        }

        [TestCase("")]
        [TestCase(null)]
        public void WrapLineEmptyMiddle(string line)
        {
            VirtualTerminal terminal = new();
            terminal.StdOut.WrapLine("Lorem ipsum dolor sit amet, ");
            terminal.StdOut.WrapLine(line);
            terminal.StdOut.WrapLine("duo accumsan scripserit ad");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(3));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("Lorem ipsum dolor sit amet,"));
            Assert.That(terminal.StdOutLines[1], Is.Empty);
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("duo accumsan scripserit ad"));
        }

        [Test]
        public void WrapLineConsole()
        {
            VirtualTerminal terminal = new();
            terminal.StdOut.WrapLine(
                //        1         2         3         4         5         6         7         8
                //   5    0    5    0    5    0    5    0    5    0    5    0    5    0    5    0
                "This is a line that should be more than eighty-characters long, so that it can " +
                "be checked if the line is really being wrapped into multiple lines. The proper " +
                "test will need to be done using a virtual console so that we can check the " +
                "precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(4));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line that should be more than eighty-characters long, so that it can"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("be checked if the line is really being wrapped into multiple lines. The proper"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("test will need to be done using a virtual console so that we can check the"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("precise behaviour of wrapping."));
        }

        [Test]
        public void WrapLineConsoleWidth40UpTo39()
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                //        1         2         3         4
                //   5    0    5    0    5    0    5    0
                "This is a line that should be more than " +      // 39 chars
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                "wrapped into multiple lines. The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +         // with 'the' would be 40.
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(7));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line that should be more than"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("eighty-characters long, so that it can"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("be checked if the line is really being"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("wrapped into multiple lines. The proper"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("test will need to be done using a"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("virtual console so that we can check"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("the precise behaviour of wrapping."));
        }

        [Test]
        public void WrapLineConsoleWidth40Is40Last()
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                //        1         2         3         4
                //   5    0    5    0    5    0    5    0
                "virtual console so that we can check " +
                "the");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(2));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("virtual console so that we can check"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("the"));
        }

        [Test]
        public void WrapLineConsoleWidth40Is39Last()
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                //        1         2         3         4
                //   5    0    5    0    5    0    5    0
                "virtual console so that we can check it");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(1));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("virtual console so that we can check it"));
        }

        [Test]
        public void WrapLineConsoleWidth40Is40LastWithSpaces()
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                //        1         2         3         4
                //   5    0    5    0    5    0    5    0
                "virtual console so that we can check " +
                "the            ");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(2));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("virtual console so that we can check"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("the"));
        }

        [Test]
        public void WrapLongLineNoSpaces()
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                //        1         2         3         4
                //   5    0    5    0    5    0    5    0
                "virtual_console_so_that_we_can_check_" +
                "the_precise_behaviour_of_wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(1));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("virtual_console_so_that_we_can_check_the_precise_behaviour_of_wrapping."));
        }

        [Test]
        public void WrapLongLineNoSpacesThenSpace()
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                //        1         2         3         4
                //   5    0    5    0    5    0    5    0
                "virtual_console_so_that_we_can_check_" +
                "the_precise_behaviour_of_wrapping. The " +
                "long line is finished. And provide at " +
                "least two more lines to check wrap.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(3));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("virtual_console_so_that_we_can_check_the_precise_behaviour_of_wrapping."));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("The long line is finished. And provide"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("at least two more lines to check wrap."));
        }

        [Test]
        public void WrapLineConsoleWidth40UpTo39Indent2()
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(2,
                //        1         2         3         4
                //   5    0    5    0    5    0    5    0
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                "wrapped into multiple lines. The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(8));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("  This is a line that should be more"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("  than eighty-characters long, so that"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("  it can be checked if the line is"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("  really being wrapped into multiple"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("  lines. The proper test will need to"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("  be done using a virtual console so"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("  that we can check the precise"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("  behaviour of wrapping."));
        }

        [Test]
        public void WrapLineConsoleWidth40UpTo39Indent2HangRight2()
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(2, 2,
                //        1         2         3         4
                //   5    0    5    0    5    0    5    0
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                "wrapped into multiple lines. The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(9));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("  This is a line that should be more"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("    than eighty-characters long, so"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("    that it can be checked if the line"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("    is really being wrapped into"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("    multiple lines. The proper test"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("    will need to be done using a"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("    virtual console so that we can"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("    check the precise behaviour of"));
            Assert.That(terminal.StdOutLines[8], Is.EqualTo("    wrapping."));
        }

        [Test]
        public void WrapLineConsoleWidth40UpTo39Indent2HangLeft2()
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(2, -2,
                //        1         2         3         4
                //   5    0    5    0    5    0    5    0
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                "wrapped into multiple lines. The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(7));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("  This is a line that should be more"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("than eighty-characters long, so that it"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("can be checked if the line is really"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("being wrapped into multiple lines. The"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("proper test will need to be done using"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("a virtual console so that we can check"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("the precise behaviour of wrapping."));
        }

        [Test]
        public void WrapLineConsoleFormat()
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine("Version of the software - {0}; Copyright {1} by {2}",
                "1.0.0", 2024, "Jason Curl");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(2));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("Version of the software - 1.0.0;"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("Copyright 2024 by Jason Curl"));
        }

        [Test]
        public void WrapLineConsoleNegativeIndentError()
        {
            VirtualTerminal terminal = new();
            Assert.That(() => {
                terminal.StdOut.WrapLine(-1, "This is a line.");
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void WrapLineConsoleNegativeHangError()
        {
            VirtualTerminal terminal = new();
            Assert.That(() => {
                terminal.StdOut.WrapLine(5, -6, "This is a line.");
            }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void WrapLineConsoleSmallWidth()
        {
            VirtualTerminal terminal = new() {
                Width = 10
            };
            terminal.StdOut.WrapLine("This is not a very wide console");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(4));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("not a"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("very wide"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("console"));
        }

        [Test]
        public void WrapLineConsoleSmallWidthIndentClamp()
        {
            VirtualTerminal terminal = new() {
                Width = 10
            };
            // The indent is reset to zero for consoles less than 10 characters wide.
            terminal.StdOut.WrapLine(2, "This is not a very wide console");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(4));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("not a"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("very wide"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("console"));
        }

        [Test]
        public void WrapLineConsoleSmallWidthIndentClampRightHang1()
        {
            VirtualTerminal terminal = new() {
                Width = 10
            };
            // The indent is reset to zero for consoles less than 10 characters wide. The hang is limited to maximum 2.
            terminal.StdOut.WrapLine(2, 1, "This is not a very wide console");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(5));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo(" not a"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo(" very"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo(" wide"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo(" console"));
        }

        [TestCase(2)]
        [TestCase(4)]
        public void WrapLineConsoleSmallWidthIndentClampRightHang2(int hang)
        {
            VirtualTerminal terminal = new() {
                Width = 10
            };
            // The indent is reset to zero for consoles less than 10 characters wide. The hang is limited to maximum 2.
            terminal.StdOut.WrapLine(2, hang, "This is not a very wide console");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(5));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("  not a"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("  very"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("  wide"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("  console"));
        }

        [Test]
        public void WrapLineConsoleSmallWidthIndentClampLeftHang2()
        {
            VirtualTerminal terminal = new() {
                Width = 10
            };
            // The indent is reset to zero for consoles less than 10 characters wide.
            terminal.StdOut.WrapLine(2, -2, "This is not a very wide console");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(4));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("not a"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("very wide"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("console"));
        }

        [Test]
        public void WrapLineConsoleLargeIndent()
        {
            VirtualTerminal terminal = new() {
                Width = 25
            };
            // Even if the indent is 20, we use an indent of 15 so that there are 10 chars to print with.
            terminal.StdOut.WrapLine(20, "This has a very large indent with less than 10 chars on the right");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(8));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("               This has"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("               a very"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("               large"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("               indent"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("               with less"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("               than 10"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("               chars on"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("               the right"));
        }

        [Test]
        public void WrapLineConsoleLargeIndentLeftHang4()
        {
            VirtualTerminal terminal = new() {
                Width = 25
            };
            // Even if the indent is 20, we use an indent of 15 so that there are 10 chars to print with.
            terminal.StdOut.WrapLine(20, -4, "This has a very large indent with less than 10 chars on the right");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(6));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("               This has"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("           a very large"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("           indent with"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("           less than 10"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("           chars on the"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("           right"));
        }

        [TestCase(2)]
        [TestCase(4)]
        public void WrapLineConsoleLargeIndentRightHang2(int hang)
        {
            VirtualTerminal terminal = new() {
                Width = 25
            };
            // Even if the indent is 20, we use an indent of 15 so that there are 10 chars to print with. There is a max
            // indent of 2 if the width is less than 10 (so a hang is min width of 8).
            terminal.StdOut.WrapLine(20, hang, "This has a very large indent with less than 10 chars on the right");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(10));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("               This has"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("                 a very"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("                 large"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("                 indent"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("                 with"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("                 less"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("                 than 10"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("                 chars"));
            Assert.That(terminal.StdOutLines[8], Is.EqualTo("                 on the"));
            Assert.That(terminal.StdOutLines[9], Is.EqualTo("                 right"));
        }

        [Test]
        public void WrapLineConsoleLargeIndentLeftHang20()
        {
            VirtualTerminal terminal = new() {
                Width = 25
            };
            // Even if the indent is 20, we use an indent of 15 so that there are 10 chars to print with. The left hang
            // is large, so we clamp to the far left margin. The hang is relative to the index.
            terminal.StdOut.WrapLine(20, -20, "This has a very large indent with less than 10 chars on the right");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(4));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("               This has"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("a very large indent with"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("less than 10 chars on"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("the right"));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLine(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                $"wrapped into multiple lines.{sep}The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(8));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line that should be more than"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("eighty-characters long, so that it can"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("be checked if the line is really being"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("wrapped into multiple lines."));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("The proper test will need to be done"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("using a virtual console so that we can"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("check the precise behaviour of"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineMultiple(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                $"wrapped into multiple lines.{sep}{sep}The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(9));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line that should be more than"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("eighty-characters long, so that it can"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("be checked if the line is really being"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("wrapped into multiple lines."));
            Assert.That(terminal.StdOutLines[4], Is.Empty);
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("The proper test will need to be done"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("using a virtual console so that we can"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("check the precise behaviour of"));
            Assert.That(terminal.StdOutLines[8], Is.EqualTo("wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineIndent(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(2,
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                $"wrapped into multiple lines.{sep}The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(9));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("  This is a line that should be more"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("  than eighty-characters long, so that"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("  it can be checked if the line is"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("  really being wrapped into multiple"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("  lines."));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("  The proper test will need to be done"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("  using a virtual console so that we"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("  can check the precise behaviour of"));
            Assert.That(terminal.StdOutLines[8], Is.EqualTo("  wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineIndentMultiple(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(2,
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                $"wrapped into multiple lines.{sep}{sep}The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(10));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("  This is a line that should be more"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("  than eighty-characters long, so that"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("  it can be checked if the line is"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("  really being wrapped into multiple"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("  lines."));
            Assert.That(terminal.StdOutLines[5], Is.Empty);
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("  The proper test will need to be done"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("  using a virtual console so that we"));
            Assert.That(terminal.StdOutLines[8], Is.EqualTo("  can check the precise behaviour of"));
            Assert.That(terminal.StdOutLines[9], Is.EqualTo("  wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineIndentLeftHang(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(2, -2,
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                $"wrapped into multiple lines.{sep}The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(8));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("  This is a line that should be more"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("than eighty-characters long, so that it"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("can be checked if the line is really"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("being wrapped into multiple lines."));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("  The proper test will need to be done"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("using a virtual console so that we can"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("check the precise behaviour of"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineIndentLeftHangMultiple(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(2, -2,
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                $"wrapped into multiple lines.{sep}{sep}The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(9));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("  This is a line that should be more"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("than eighty-characters long, so that it"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("can be checked if the line is really"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("being wrapped into multiple lines."));
            Assert.That(terminal.StdOutLines[4], Is.Empty);
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("  The proper test will need to be done"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("using a virtual console so that we can"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("check the precise behaviour of"));
            Assert.That(terminal.StdOutLines[8], Is.EqualTo("wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineIndentRightHang(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(2, 2,
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                $"wrapped into multiple lines.{sep}The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(9));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("  This is a line that should be more"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("    than eighty-characters long, so"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("    that it can be checked if the line"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("    is really being wrapped into"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("    multiple lines."));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("  The proper test will need to be done"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("    using a virtual console so that we"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("    can check the precise behaviour of"));
            Assert.That(terminal.StdOutLines[8], Is.EqualTo("    wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineIndentRightHangMultiple(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(2, 2,
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                $"wrapped into multiple lines.{sep}{sep}The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(10));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("  This is a line that should be more"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("    than eighty-characters long, so"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("    that it can be checked if the line"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("    is really being wrapped into"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("    multiple lines."));
            Assert.That(terminal.StdOutLines[5], Is.Empty);
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("  The proper test will need to be done"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("    using a virtual console so that we"));
            Assert.That(terminal.StdOutLines[8], Is.EqualTo("    can check the precise behaviour of"));
            Assert.That(terminal.StdOutLines[9], Is.EqualTo("    wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineAutoIndent(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                $"wrapped into multiple lines.{sep} The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(8));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line that should be more than"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("eighty-characters long, so that it can"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("be checked if the line is really being"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("wrapped into multiple lines."));
            // If we have a space in the original, it means here that we use the indent.
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("The proper test will need to be done"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("using a virtual console so that we can"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("check the precise behaviour of"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineMultipleAutoIndent(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                $"wrapped into multiple lines.{sep}{sep} The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(9));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line that should be more than"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("eighty-characters long, so that it can"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("be checked if the line is really being"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("wrapped into multiple lines."));
            Assert.That(terminal.StdOutLines[4], Is.Empty);
            // If we have a space in the original, it means here that we use the indent.
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("The proper test will need to be done"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("using a virtual console so that we can"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("check the precise behaviour of"));
            Assert.That(terminal.StdOutLines[8], Is.EqualTo("wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineIndentRightHangAutoIndent(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(2, 2,
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                $"wrapped into multiple lines.{sep} The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(9));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("  This is a line that should be more"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("    than eighty-characters long, so"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("    that it can be checked if the line"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("    is really being wrapped into"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("    multiple lines."));
            // If we have a space in the original, it means here that we use the indent.
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("    The proper test will need to be"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("    done using a virtual console so"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("    that we can check the precise"));
            Assert.That(terminal.StdOutLines[8], Is.EqualTo("    behaviour of wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineIndentRightHangMultipleAutoIndent(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(2, 2,
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                $"wrapped into multiple lines.{sep}{sep} The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(10));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("  This is a line that should be more"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("    than eighty-characters long, so"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("    that it can be checked if the line"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("    is really being wrapped into"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("    multiple lines."));
            Assert.That(terminal.StdOutLines[5], Is.Empty);
            // If we have a space in the original, it means here that we use the indent.
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("    The proper test will need to be"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("    done using a virtual console so"));
            Assert.That(terminal.StdOutLines[8], Is.EqualTo("    that we can check the precise"));
            Assert.That(terminal.StdOutLines[9], Is.EqualTo("    behaviour of wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineIndentLeftHangAutoIndent(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(2, -2,
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                $"wrapped into multiple lines.{sep} The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(8));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("  This is a line that should be more"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("than eighty-characters long, so that it"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("can be checked if the line is really"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("being wrapped into multiple lines."));
            // Note, even if we have a space in the original, it means here that we don't indent when "hang" is not
            // zero.
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("The proper test will need to be done"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("using a virtual console so that we can"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("check the precise behaviour of"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineIndentLeftHangMultipleAutoIndent(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(2, -2,
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                $"wrapped into multiple lines.{sep}{sep} The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(9));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("  This is a line that should be more"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("than eighty-characters long, so that it"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("can be checked if the line is really"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("being wrapped into multiple lines."));
            Assert.That(terminal.StdOutLines[4], Is.Empty);
            // Note, even if we have a space in the original, it means here that we don't indent when "hang" is not
            // zero.
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("The proper test will need to be done"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("using a virtual console so that we can"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("check the precise behaviour of"));
            Assert.That(terminal.StdOutLines[8], Is.EqualTo("wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineStart(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                $"{sep}This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                "wrapped into multiple lines. The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(8));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.Empty);
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("This is a line that should be more than"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("eighty-characters long, so that it can"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("be checked if the line is really being"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("wrapped into multiple lines. The proper"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("test will need to be done using a"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("virtual console so that we can check"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("the precise behaviour of wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineMultipleStart(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                $"{sep}{sep}This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                "wrapped into multiple lines. The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                "the precise behaviour of wrapping.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(9));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.Empty);
            Assert.That(terminal.StdOutLines[1], Is.Empty);
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("This is a line that should be more than"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("eighty-characters long, so that it can"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("be checked if the line is really being"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("wrapped into multiple lines. The proper"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("test will need to be done using a"));
            Assert.That(terminal.StdOutLines[7], Is.EqualTo("virtual console so that we can check"));
            Assert.That(terminal.StdOutLines[8], Is.EqualTo("the precise behaviour of wrapping."));
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineEnd(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                "wrapped into multiple lines. The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                $"the precise behaviour of wrapping.{sep}");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(8));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line that should be more than"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("eighty-characters long, so that it can"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("be checked if the line is really being"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("wrapped into multiple lines. The proper"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("test will need to be done using a"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("virtual console so that we can check"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("the precise behaviour of wrapping."));
            Assert.That(terminal.StdOutLines[7], Is.Empty);
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineMultipleEnd(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                "wrapped into multiple lines. The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                $"the precise behaviour of wrapping.{sep}{sep}");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(9));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line that should be more than"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("eighty-characters long, so that it can"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("be checked if the line is really being"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("wrapped into multiple lines. The proper"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("test will need to be done using a"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("virtual console so that we can check"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("the precise behaviour of wrapping."));
            Assert.That(terminal.StdOutLines[7], Is.Empty);
            Assert.That(terminal.StdOutLines[8], Is.Empty);
        }

        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n")]
        public void WrapLineWithNewLineMultipleEndBoundary(string sep)
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine(
                "This is a line that should be more than " +
                "eighty-characters long, so that it can " +
                "be checked if the line is really being " +
                "wrapped into multiple lines. The proper " +
                "test will need to be done using a " +
                "virtual console so that we can check " +
                $"the precise behaviour of cccc wrapping.{sep}{sep}");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(9));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line that should be more than"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("eighty-characters long, so that it can"));
            Assert.That(terminal.StdOutLines[2], Is.EqualTo("be checked if the line is really being"));
            Assert.That(terminal.StdOutLines[3], Is.EqualTo("wrapped into multiple lines. The proper"));
            Assert.That(terminal.StdOutLines[4], Is.EqualTo("test will need to be done using a"));
            Assert.That(terminal.StdOutLines[5], Is.EqualTo("virtual console so that we can check"));
            Assert.That(terminal.StdOutLines[6], Is.EqualTo("the precise behaviour of cccc wrapping."));
            Assert.That(terminal.StdOutLines[7], Is.Empty);
            Assert.That(terminal.StdOutLines[8], Is.Empty);
        }

        [Test]
        public void WrapLineWithTab()
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine("This is a line that\thas\ttabs. How should tabs\tlook.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(2));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line that has tabs. How"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("should tabs look."));
        }

        [Test]
        public void WrapLineWithMultipleSpaces()
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WrapLine("This is a line that  has  multiple  spaces  in  it. How should  this  look.");
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(2));
            //                                                        1         2         3         4
            //                                              0    5    0    5    0    5    0    5    0
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a line that has multiple spaces"));
            Assert.That(terminal.StdOutLines[1], Is.EqualTo("in it. How should this look."));
        }

        [Test]
        public void WrapLineEmptyLine()
        {
            VirtualTerminal terminal = new() {
                Width = 40
            };
            terminal.StdOut.WriteLine("This is a test");
            terminal.StdOut.WriteLine();
            Assert.That(terminal.StdOutLines, Has.Count.EqualTo(2));
            Assert.That(terminal.StdOutLines[0], Is.EqualTo("This is a test"));
            Assert.That(terminal.StdOutLines[1], Is.Empty);
        }
    }
}
