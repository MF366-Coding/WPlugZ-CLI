# WPlugZ
> The only CLI you'll ever need for developing WriterClassic plugins.

Need help getting started with making your very own WriterClassic plugin? Then, use **WPluZ**, a powerful **CLI** for helping you manage your own plugins.

## Features
- Creating a new plugin (`new`)
- Bumping and managing versions (`bump`)
- Removing a plugin (`remove`)
- Testing a plugin using WriterClassic (`test`)

## Anti-Features
- Creating a ZIP package
- Find WriterClassic automatically (the path to it must be given)
- Removing a certain version and not the entire plugin when using `remove`

## Flags
Here is a list of the flags for every command.

| Command Group | Short Form Flag | Long Form Flag | Optional? |
------------------------------------------------------------
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
| None | `-h` | `--help` | Yes |
| None | `-w` | `--info` | Yes |

## Things you shouldn't do
- Create a plugin on a dir that already has one
- Bump to previous versions
- Rebump versions
- Use `remove` on dirs that don't have plugins (it actually erases non-plugin files so **be careful!!!!**)
