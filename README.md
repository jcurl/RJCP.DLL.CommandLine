# RJCP.Core.CommandLine <!-- omit in toc -->

This library parses command line options, and abstracts wrapping text to the
console.

- [1. Parsing Command Line](#1-parsing-command-line)
  - [1.1. Simple Usage Walkthrough](#11-simple-usage-walkthrough)
  - [1.2. Defining the Options Style](#12-defining-the-options-style)
    - [1.2.1. Windows Style](#121-windows-style)
    - [1.2.2. Unix Style](#122-unix-style)
  - [1.3. Defining the Options](#13-defining-the-options)
    - [1.3.1. Decorating the Fields and Properties](#131-decorating-the-fields-and-properties)
    - [1.3.2. Default Options](#132-default-options)
    - [1.3.3. Remaining Arguments](#133-remaining-arguments)
  - [1.4. Exceptions while Parsing](#14-exceptions-while-parsing)
  - [1.5. Extending with `IOptions`](#15-extending-with-ioptions)
- [2. Converting Windows Command Line](#2-converting-windows-command-line)
- [3. Terminal Formatting](#3-terminal-formatting)
  - [3.1. Sample Usage](#31-sample-usage)
    - [3.1.1. Wrapping Lines (simply)](#311-wrapping-lines-simply)
    - [3.1.2. Indenting](#312-indenting)
    - [3.1.3. Indented Paragraphs](#313-indented-paragraphs)
    - [3.1.4. Hanging Indented Paragraphs](#314-hanging-indented-paragraphs)
    - [3.1.5. New Lines with Wrapping Lines](#315-new-lines-with-wrapping-lines)
  - [3.2. Setting Colours](#32-setting-colours)
  - [3.3. Test Cases with Dependency Injection](#33-test-cases-with-dependency-injection)
  - [3.4. Limitations](#34-limitations)
    - [3.4.1. No Escape Sequences](#341-no-escape-sequences)
    - [3.4.2. Fixed Width on Linux with Mono](#342-fixed-width-on-linux-with-mono)
    - [3.4.3. Mixing Write and WriteLine with WrapLine](#343-mixing-write-and-writeline-with-wrapline)

## 1. Parsing Command Line

The Command Line library can parse the command line, putting the results in a
user supplied class. It uses properties decorated with attributes to describe
the name of the command line options, and the type to determine the valid inputs
for the command line option. Through reflection, it writes to the class.

It was first written at a time when almost no command line option parsing
existed in .NET Framework, and the desire to use .NET's reflection for making
parsing simpler.

### 1.1. Simple Usage Walkthrough

Define a class that contains the command line options (this is an example from
another of my projects):

```csharp
namespace RJCP.VsSolutionSort.CmdLine {
    using System.Collections.Generic;
    using RJCP.Core.CommandLine;

    internal class SolutionOptions {
        [Option('v', "version")]
        public bool Version { get; private set; }

        [Option('?', "help")]
        public bool Help { get; private set; }

        [Option('R', "recurse")]
        public bool Recurse { get; private set; }

        [Option('d', "dryrun")]
        public bool DryRun { get; private set; }

        private int m_Jobs = 0;

        [Option('j', "jobs")]
        public int Jobs
        {
            get { return m_Jobs; }
            private set
            {
                if (value < 1)
                    throw new OptionException("Number of jobs must be 1 or more.");
                if (value > 255)
                    throw new OptionException("Maximum number of jobs is 255.");
                m_Jobs = value;
            }
        }

        [OptionArguments]
        public readonly List<string> Arguments = new();
    }
}
```

This class shows that:

- We can assign options (short option characters and long option names) to a
  single property, if it is either public or private.
- The backing store may be a private field, and still apply an option to a
  public field.
- All remaining arguments at the end, which don't belong to options, are in the
  `Arguments` list.

To parse the options, create an instance of the custom options class (it doesn't
need to derive from any other base class), and parse it.

```csharp
internal async static Task<int> Main(string[] args) {
    CmdLine.SolutionOptions options = new();
    try {
        Options.Parse(options, args, OptionsStyle.Unix);
    } catch (OptionException ex) {
        CmdLine.Terminal.WriteLine($"ERROR: {ex.Message}");
        CmdLine.Terminal.WriteLine();
        CmdLine.Help.PrintSimpleHelp();
        return 1;
    }

    if (options.Version) {
        CmdLine.Version.PrintVersion();
        return 0;
    }
```

### 1.2. Defining the Options Style

Options can be parsed as one of two styles:

- Unix format; or
- Windows format.

This is defined when calling `Options.Parse(options args, OptionsStyle)`. If the
`OptionsStyle` parameter is omitted, the default for the current Operating
System is used.

| Environment     | Style                  |
| --------------- | ---------------------- |
| Linux           | `OptionsStyle.Unix`    |
| MSys on Windows | `OptionsStyle.Unix`    |
| Windows         | `OptionsStyle.Windows` |

The MSys environment is detected through the environment variables. This
simplifies considerably calling .NET binaries from a MSYS shell (e.g. Git for
Windows) which can otherwise lead to hard to understand errors or wrong
interpretation of the command line.

All options must be presented at the start of the command line string.

#### 1.2.1. Windows Style

The Windows style for options is intended for use on Windows command line (DOS,
Console or PowerShell) and has the format:

| Argument                | Style                              |
| ----------------------- | ---------------------------------- |
| Long with parameter     | `/option:value` or `/option value` |
| Long with no parameter  | `/option`                          |
| Short with parameter    | `/o:value` or `/o value`           |
| Short with no parameter | `/o`                               |

Each option must be presented individually, and at the beginning of the command
line string. All general arguments must be at the end.

#### 1.2.2. Unix Style

| Argument                | Style                                |
| ----------------------- | ------------------------------------ |
| Long with parameter     | `--option=value` or `--option value` |
| Long with no parameter  | `--option`                           |
| Short with parameter    | `-o=value` or `-o value`             |
| Short with no parameter | `-o`                                 |

The parsing is simple. All options must be at the start of the command line
string. Any general arguments that are not part of arguments must occur at the
end.

Patches welcome to improve compatibility.

### 1.3. Defining the Options

#### 1.3.1. Decorating the Fields and Properties

A class must be provided. It doesn't need to derive from anything. The
`Options.Parse()` method will enumerate through all fields with attributes to
build up the command line list that is required.

Each property or field which is an option shall be decorated with the
`OptionsAttribute`:

- One can write to a property, and check the value; or
- One can write to a field, and check the values later.

For complex option checking, it might be required to capture all the arguments,
and then check the results with custom code, rather than in the property itself.
E.g. some properties might not be allowed simultaneously, or the interpretation
of another property is dependent on some other runtime factors.

An option can be given a short name, a long name, or both.

- `[Option('s')]`; or
- `[Option("short")]`; or
- `[Option('s', "short")]`.

If the option must be provided, append `true` as the last argument:

- `[Option('s', required: true)]`; or
- `[Option("short", required: true)]`; or
- `[Option('s', "short", required: true)]`.

Short option names must be a letter (`a-z` or `A-Z`), a digit (`0-9`), or a
symbol `!`, `?`, `#`.

Long option names may begin only with a letter (`a-z` or `A-Z`). Options may have
letters, digits (`0-9`), a dash `-`, dot `.` or an underscore `_`.

The short options and long options must be unique for all options that are being
parsed.

#### 1.3.2. Default Options

Each field may have a default option. The `[OptionDefault("string")]` should be
applied to a property or field. The argument to the attribute is a string (not
the native type of the field or property) and is converted while parsing.

#### 1.3.3. Remaining Arguments

A property of type `List<string>` can be assigned the attribute
`[OptionArguments]`, where all arguments that are not parsed as options are
parsed.

For the current version, options must occur before the default arguments.

### 1.4. Exceptions while Parsing

All exceptions derive from `OptionException`.

- `OptionException`
  - User provided a list but quotes are not formatted correctly, or missing
    quotes.
  - User provided an unexpected value to an option.
- `OptionUnknownException`
  - User provided an option on the command line that is unknown.
- `OptionAssignedException`
  - The option was provided more than once on the command line.
- `OptionFormatException`
  - There was a general exception parsing the value for an option, either by
    converting the type, or by the property raising an exception.
- `OptionMissingException`
  - A mandatory option was not provided on the command line.

Exceptions raised that are not caused by the user:

- `ArgumentException`
  - A property has no setter. To fix, ensure the property has a `set` method.
  - A field is readonly. To fix, ensure the field is not `readonly` by removing
    that keyword.
  - The `OptionArgumentsAttribute` is used more than once. To fix only use once.
  - The `OptionArgumentsAttribute` must be assigned to a collection property. To
    fix, assign to a list property of strings.
  - The `OptionArgumentsAttribute` is assigned to a collection of non-strings.
    To fix, assign to a list property of strings.
  - The short option from `OptionAttribute` is used on more than one field or
    property. Must be fixed in the program. Note, single character long options
    are included in this check, as for example, Windows has the same character
    `/` for short and long options.
  - The long option from `OptionAttribute` is used on more than one field or
    property. Must be fixed in the program. Note, a single character long
    options must be unique also when checked against short options, as for
    example, Windows has the same character `/` for short and long options.
  - Types must be simple types (primitive types, `bool` or `string` types),
    which apply also to collections.

### 1.5. Extending with `IOptions`

If the class for parsing options implements the interface `IOptions`, methods on
that class will be called during parsing for certain scenarios (instead of
assigning events and getting callbacks).

The method `IOptions.InvalidOption` is called on:

- `OptionUnknownException`
- `OptionMissingArgumentException`
- `OptionFormatException`

If there is a general `OptionException`, then `IOptions.Usage` is called.

After all the options have been parsed, the method `IOptions.Check` is called,
that allows for checking the consistency of the options. Then the options class
can raise an exception.

The guidelines towards providing an `IOptions` interface is to allow all code
related to option handling and checks, as well as help, to be in a single class.

## 2. Converting Windows Command Line

When windows starts a process, it does not preprocess any of the text given on
the command line. This is contrary to Linux where the shell typically splits the
command line into multiple arguments and parses that to the application.

Under Windows, normally the implementation of the runtime library (e.g. MSVCRT) does this.

The static methods in `CommandLine.Parse.Windows` allow to split a full string,
or to join a complete string. This is useful when using Windows API
[`CreateProcess`](https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-createprocessw).

It emulates as much as possible the behaviour in
[`CommandLineToArgv`](https://learn.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-commandlinetoargvw)
and was tested on Windows 10 and Windows 11.

The implementation however does not depend on any Windows API, and as such, may
not match the behaviour of future Windows versions.

## 3. Terminal Formatting

Often with command line programs comes the requirement to provide feedback on
the terminal. This library provides a very simple implementation for formatting
information on the console.

There is the object `Terminal` that can write information to the console that
can wrap on the console width, for both `StdErr` and `StdOut`.

### 3.1. Sample Usage

#### 3.1.1. Wrapping Lines (simply)

As a simple usage of the functionality is given: The line being wrapped is a
single string. Often these strings are obtained from a string resource and is
not available directly in code. It might have translations.

```csharp
ConsoleTerminal terminal = new ConsoleTerminal();
terminal.StdOut.WrapLine(
  "This is a line that should be more " +
  "than eighty-characters long, so that it can " +
  "be checked if the line is really being " +
  "wrapped into multiple lines. The proper " +
  "test will need to be done using a virtual " +
  "console so that we can check the " +
  "precise behaviour of wrapping.");
```

The output will be:

```text
         1    1    2    2    3    3    4    4    5    5    6    6    7    7    8
    5    0    5    0    5    0    5    0    5    0    5    0    5    0    5    0
----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+
This is a line that should be more than eighty-characters long, so that it can
be checked if the line is really being wrapped into multiple lines. The proper
test will need to be done using a virtual console so that we can check the
precise behaviour of wrapping.
```

When lines wrap, they do so one character before the width of the console. So if
the console has a width of 80 characters, it will print up to 79 characters and
then start on the next line.

#### 3.1.2. Indenting

It is possible to format the strings so there is an indent.

```csharp
terminal.StdOut.WrapLine(indent: 4,
  "This is a line that should be more " +
  "than eighty-characters long, so that it can " +
  "be checked if the line is really being " +
  "wrapped into multiple lines. The proper " +
  "test will need to be done using a virtual " +
  "console so that we can check the " +
  "precise behaviour of wrapping.");
```

The output will be:

```text
         1    1    2    2    3    3    4    4    5    5    6    6    7    7    8
    5    0    5    0    5    0    5    0    5    0    5    0    5    0    5    0
----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+
    This is a line that should be more than eighty-characters long, so that it
    can be checked if the line is really being wrapped into multiple lines. The
    proper test will need to be done using a virtual console so that we can
    check the precise behaviour of wrapping.
```

#### 3.1.3. Indented Paragraphs

It is possible to format the strings so that the first line is indented. This is
using a hanging indent, but the offset is negative.

```csharp
terminal.StdOut.WrapLine(indent: 2, hang: -2,
  "This is a line that should be more " +
  "than eighty-characters long, so that it can " +
  "be checked if the line is really being " +
  "wrapped into multiple lines. The proper " +
  "test will need to be done using a virtual " +
  "console so that we can check the " +
  "precise behaviour of wrapping.");
```

The output will be:

```text
         1    1    2    2    3    3    4    4    5    5    6    6    7    7    8
    5    0    5    0    5    0    5    0    5    0    5    0    5    0    5    0
----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+
  This is a line that should be more than eighty-characters long, so that it
can be checked if the line is really being wrapped into multiple lines. The
proper test will need to be done using a virtual console so that we can check
the precise behaviour of wrapping.
```

#### 3.1.4. Hanging Indented Paragraphs

When printing help information, you might want to print bullet point
information. To do this, hanging indented paragraphs are useful. A hanging
indent is given by a positive value for the hang parameter:

```csharp
terminal.StdOut.WrapLine(indent: 2, hang: 2,
  "* If the return value is 1, this indicates " +
  "there was a problem processing the file. Check "+
  "that no other process is using the file and " +
  "try again.");
```

The output will be:

```text
         1    1    2    2    3    3    4    4    5    5    6    6    7    7    8
    5    0    5    0    5    0    5    0    5    0    5    0    5    0    5    0
----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+
* If the return value is 1, this indicates there was a problem processing the
  file. Check that no other process is using the file and try again.
```

You'll notice on the output, the second line is indented.

#### 3.1.5. New Lines with Wrapping Lines

You can insert new lines in the string for starting a new line. The indent and
hanging values are reset and applied to the new line again. If you want to start
the line with the hanging indent, then simply insert a space as the first
character.

This allows to have a resource file with multiple paragraphs for an option, that
makes it much easier for translators concentrate on a single resource entry.

```csharp
terminal.StdOut.WrapLine(indent: 0, hang: 2,
  "* If the return value is 1, this indicates " +
  "there was a problem processing the file.\n\n" +
  // Note the space here to use the hanging indent on the new line
  " Check that no other process is using the file " +
  "and try again.\n\n" +
  // This will start at the indent on the far left
  "* If the return value is 255, there was a " +
  "bug in the program. Please report it to the " +
  "author at:\n\n" +
  " author@mycompany.com");
```

The output will be:

```text
         1    1    2    2    3    3    4    4    5    5    6    6    7    7    8
    5    0    5    0    5    0    5    0    5    0    5    0    5    0    5    0
----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+----+
* If the return value is 1, this indicates there was a problem processing the
  file.

  Check that no other process is using the file and try again.

* If the return value is 255, there was a bug in the program. Please report it
  to the author at

  author@mycompany.com
```

### 3.2. Setting Colours

Colours can be set on a single line. Use the functions

```csharp
ConsoleTerminal terminal = new ConsoleTerminal();
terminal.ForegroundColor = ConsoleColor.White;
terminal.BackgroundColor = ConsoleColor.Blue;
```

### 3.3. Test Cases with Dependency Injection

There is an implementation of `VirtualTerminal` that is intended to simulate a
terminal for test cases. At the end of the test case, the user can check the
content of the terminal.

For design information, see [DESIGN_Termina.md](./docs/DESIGN_Terminal.md).

### 3.4. Limitations

#### 3.4.1. No Escape Sequences

The implementation is simple, it doesn't support escape sequences when wrapping.
Nor are there any tests.

#### 3.4.2. Fixed Width on Linux with Mono

On .NET 4.0 on Linux, it may not be possible to detect if redirection is
occurring on the console. The library depends on undefined behaviour that the
console width is zero in redirection. To get full functionality, it's
recommended to use .NET Framework 4.6.2 or .NET Core instead.

#### 3.4.3. Mixing Write and WriteLine with WrapLine

The two sets of APIs are not intended to be used with each other. The
`WrapLine()` function is assumed to always start at the beginning of the line.
Use `Write()` and `WriteLine()` for more control over writing on the console.
The latter two are calling the `Console.Write()` and `Console.WriteLine()`
directly and are provided to allow testing with a `VirtualTerminal`.
