# WPlugZ
### > Now written in C# (v3.1.0 coming soon).
Not just a rewrite. This version does actually include nee features.

> The only CLI you'll ever need for developing WriterClassic plugins.

[![Latest Version](https://img.shields.io/github/v/tag/MF366-Coding/WPlugZ-CLI?color=brown)](https://github.com/MF366-Coding/WPlugZ-CLI/releases/latest)
[![License](https://img.shields.io/github/license/MF366-Coding/WPlugZ-CLI)](https://raw.githubusercontent.com/MF366-Coding/WPlugZ-CLI/main/LICENSE)
![GitHub top language](https://img.shields.io/github/languages/top/MF366-Coding/WPlugZ-CLI?color=purple)
![GitHub contributors](https://img.shields.io/github/contributors/MF366-Coding/WPlugZ-CLI?color=yellow)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues-pr/MF366-Coding/WPlugZ-CLI?style=flat&color=blue)
![GitHub Repo stars](https://img.shields.io/github/stars/MF366-Coding/WPlugZ-CLI?color=red)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/MF366-Coding/WPlugZ-CLI?style=flat&color=red)

Need help getting started with making your very own WriterClassic plugin? Then, use **WPluZ**, a powerful **CLI** for helping you manage your own plugins.

## Coming Soon
- [ ] Better Documentation
- [ ] Sync Details, Manifest, etc... with one command
- [ ] Add default image with one command (useful if accidentally removed)
- [ ] ...and much more!

## Features
- Creating a new plugin (`new`)
- Bumping and managing versions (`bump`)
- Removing a plugin (`remove`)
- Testing a plugin using WriterClassic (`test`)
- **NEW!** Verify if a MANIFEST file is structured correctly (`verify` or use the **WPlugZ Manifest Verify** script)

## Anti-Features
- Creating a ZIP package
- Find WriterClassic automatically (the path to it must be given)
- Removing a certain version and not the entire plugin when using `remove`

## Flags
Here is a list of the flags for every command.

| Command Group | Short Form Flag | Long Form Flag | Optional? |
--------|------|-----------|----|
| `new` | `-t` | `--title` | No |
| `new` | `-a` | `--author` | Yes |
| `new` | `-d` | `--description` | Yes |
| `new` | `-i` | `--icon` | Yes |
| `new` | `-v` | `--version` | Yes |
| `new` | None | `--package` | Yes |
| `new` | None | `--authorfile` | Yes |
| `new` | None | `--readme` | Yes |
| `new` | None | `--versioning` | Yes |
| `new` | None | `--author` | Yes |
| `test` | `-p` | `--path` | No |
| `test` | `-v` | `--version` | Yes |
| `remove` | None | `--skip` | Yes |
| `bump` | `-c` | `--changes` | Yes |
| `bump` | `-v` | `--version` | Yes |
| `verify` | `-f` | `--file` | No |
| `verify` | None | `--ignore-hints` | Yes |
| None | `-h` | `--help` | Yes |
| None | `-w` | `--info` | Yes |

## Things you shouldn't do
- Create a plugin on a dir that already has one
- Bump to previous versions
- Rebump versions
- Use `remove` on dirs that don't have plugins (it actually erases non-plugin files so **be careful!!!!**)
- Verify files that are not WriterClassic Plugin MANIFESTs (it doesn't do anything to the file but it's gonna show a lot of errors probably)

## About WPlugZ Manifest Verify's errors and warnings
### E1: TypeError
A certain type (e.g.: string) was expected for a parameter but another type (e.g.: bool) was given instead.

### E2: InvalidVersionTag
The version tag doesn’t match at least one of the following criteria:
•    Starts with lowercase ‘v’
•    Has at least 2 characters and less than 5, in total
•    There are only digits after the first character

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
