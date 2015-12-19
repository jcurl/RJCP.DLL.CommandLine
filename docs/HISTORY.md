# RJCP.CommandLine

This library parses command line options using reflection and setting the values
in a class.

## History

The code base is a consolidation from my other repositories that have been used
in other of my projects (starting with Subversion, then later Git). This repository
maintains source history, with the following changes:

* The solution and project files aren't original. The original project started
  with Visual Studio 2010, and now can be used with Visual Studio 2019.
* Namespaces have been rewritten to be `RJCP.Core`, as the other projects were
  similar code, but different namespaces while I developed the code for my
  personal use and used it in side projects where I work.
* The directory structure is different than the original, so it is easier to
  track history.
* NUnit 2.x is imported as a NuGet package.

The original project started with .NET 4.0. It continues to this time to still
target .NET 4.0.

The history is maintained to make it easier to visualize changes over time, and
to be able to use tools like "blame" to identify when issues first occurred. The
commit history tracks that from the original repositories. The migration was
done manually, but this code base is relatively small.
