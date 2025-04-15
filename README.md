# WPlugZ-CLI
> Now written in C# (v3.1.0+).
Not just a rewrite. This version does actually include new features.

> The only CLI you'll ever need for developing WriterClassic plugins.

[![Latest Version](https://img.shields.io/github/v/tag/MF366-Coding/WPlugZ-CLI?color=brown)](https://github.com/MF366-Coding/WPlugZ-CLI/releases/latest)
[![License](https://img.shields.io/github/license/MF366-Coding/WPlugZ-CLI)](https://raw.githubusercontent.com/MF366-Coding/WPlugZ-CLI/main/LICENSE)
![GitHub top language](https://img.shields.io/github/languages/top/MF366-Coding/WPlugZ-CLI?color=purple)
![GitHub contributors](https://img.shields.io/github/contributors/MF366-Coding/WPlugZ-CLI?color=yellow)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues-pr/MF366-Coding/WPlugZ-CLI?style=flat&color=blue)
![GitHub Repo stars](https://img.shields.io/github/stars/MF366-Coding/WPlugZ-CLI?color=red)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/MF366-Coding/WPlugZ-CLI?style=flat&color=red)

Need help getting started with making your very own WriterClassic plugin? Then, use **WPluZ**, a powerful **CLI** for helping you manage your own plugins.

## Features
- Creating a new plugin (`new` / `create`)
- Bumping the plugin to a new version (`bump` / `bmp`)
- Removing a plugin or a specific version of one (`remove` / `delete` / `rm` / `del`)
- Testing a plugin using WriterClassic (`test`)
- Verify if a MANIFEST file is structured correctly (`verify` / `manifest`)
- Creating a ZIP package (`pack`)

## Anti-Features
- Find WriterClassic automatically (the path to it must be given)
- Updating the MANIFEST when removing a version with `remove`

## Things you shouldn't do
- Create a plugin on a dir that already has one
- Use `remove` on dirs that don't have plugins (it actually erases non-plugin files so **be careful!!!!**)
- Verify files that are not WriterClassic Plugin MANIFESTs (it doesn't do anything to the file but it's gonna show a lot of errors probably)

## How to build your own binaries
**Command:** `dotnet publish -c Release -r {your-runtime-id} -f {your-framework-of-choice} WPlugZ.csproj`

**Your Runtime ID:** Can be `linux-x64` or `win-x64`

**Your framework of choice:** Can be `net6.0`, `net7.0` or `net8.0`

### Custom options
You can add compilation options, using the `-p` flag.

**Command:** `dotnet publish -c Release -r {your-runtime-id} -f {your-framework-of-choice} -p:{option1}={value1};{option2}={value2} WPlugZ.csproj`

**Available options:** `BuildDocs` (`true` = Build WPlugZ internal documentation, *anything else = Don't build docs = Default value); `OptimizeExecutable` (`true` = Generate an optimized executable = Default Value - valid only if in `Release` mode, *anything else = Don't generate optimized executables)

**Creating an unoptimized executable (it's optimized by default):** `dotnet publish -c Release -r {your-runtime-id} -f {your-framework-of-choice} -p:OptimizeExecutable=false WPlugZ.csproj`
**Creating an unoptimized executable with internal documentation (the latter is disabled by default):** `dotnet publish -c Release -r {your-runtime-id} -f {your-framework-of-choice} -p:OptimizeExecutable=false;BuildDocs=true WPlugZ.csproj`

## About WPlugZ Manifest Verify's errors and warnings
Below you can find information an all of the WPlugZ Manifest Verify's errors et al.

They haven't ben changed much since `v3.0.0` but some are slightly stricter.

### E1: TypeError
A certain type (e.g.: string) was expected for a parameter but another type (e.g.: bool) was given instead.

### E2: InvalidVersionTag
The version tag doesn’t match the Regex pattern `^v(1000|[1-9][0-9]{0,2})$`.

### E3: ForbiddenName
A forbidden character such as “*” or “?” was used in the name parameter.

### E4: EmptyVersionObject
The version doesn’t have any parameters at all.

### E5: MandatoryParameterMissing
One of “pyfile”/”zipfile” must be defined but neither are.

### W1: UncompatibleByDefault
Used a WriterClassic version in the “uncompatible” parameter that is uncompatible by default.

### W2: RedundantUseOfParameter
Other parameters that are not “uncompatible” are being used alongside “zipfile”, which takes priority.

### W3: LongStringAsParameter
The description or the name are too long (each one has its own character limitation).

### W4: NameShouldBeManifest
The filename is not “manifest.json”.

### R1: PlaceholderTextReminder
A reminder you’re using placeholder text (which could be ‘###’, ‘…’ or ‘.’).

### H2: UsedExcludeInsteadOfUncompatible
**This is an hint, it can be ignored by doing “—ignore-hints”.**

An “exclude” parameter exists but the MANIFEST’s creator probably meant “uncompatible”.

### H3: UnnecessaryParameter
**This is an hint, it can be ignored by doing “—ignore-hints”.**

A parameter that isn’t used by WriterClassic and is not “exclude” either was given and can be safely removed.

### H4: UsingZipfileParameter
**This is an hint, it can be ignored by doing “—ignore-hints”.**

'zipfile' was used despite its use being disencouraged.
