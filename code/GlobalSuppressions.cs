// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0016:Use 'throw' expression", Justification = "Code style allowed but not mandatory")]
[assembly: SuppressMessage("Style", "IDE0034:Simplify 'default' expression", Justification = "Feature not available in C# 7.0, need 7.1 or higher")]
[assembly: SuppressMessage("Style", "IDE0039:Use local function", Justification = "Feature not available in C# 7.0, need 8.0 or higher")]
[assembly: SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Feature not available in C# 7.0, need 8.0 or higher")]
[assembly: SuppressMessage("Style", "IDE0066:Convert switch statement to expression", Justification = "Doesn't necessarily improve code readability")]
[assembly: SuppressMessage("Style", "IDE1005:Delegate invocation can be simplified.", Justification = "Old style is preferred to make it more explicit")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reviews capture most instances, exceptions allowed")]
[assembly: SuppressMessage("Major Code Smell", "S1066:Collapsible \"if\" statements should be merged", Justification = "Doesn't necessarily improve code readability")]
[assembly: SuppressMessage("Major Code Smell", "S1168:Empty arrays and collections should be returned instead of null", Justification = "Null indicates not a collection")]
[assembly: SuppressMessage("Minor Code Smell", "S2344:Enumeration type names should not have \"Flags\" or \"Enum\" suffixes", Justification = "Doesn't necessarily improve code readability")]
[assembly: SuppressMessage("Critical Code Smell", "S2346:Flags enumerations zero-value members should be named \"None\"", Justification = "Doesn't necessarily improve code readability, not applicable for flags")]
[assembly: SuppressMessage("Major Code Smell", "S2933:Fields that are only assigned in the constructor should be \"readonly\"", Justification = "")]
[assembly: SuppressMessage("Critical Code Smell", "S3217:\"Explicit\" conversions of \"foreach\" loops should not be used", Justification = "Avoid unnecessary type cast, behaviour remains the same")]
[assembly: SuppressMessage("Minor Code Smell", "S3626:Jump statements should not be redundant", Justification = "Leaving 'return' may reduce copy/paste errors")]
[assembly: SuppressMessage("Major Code Smell", "S4070:Non-flags enums should not be marked with \"FlagsAttribute\"", Justification = "The [Flags] attribute adds readability where correct")]
[assembly: SuppressMessage("Major Code Smell", "S112:General exceptions should never be thrown", Justification = "ApplicationException should never be seen by user code")]
