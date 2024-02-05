from typing import SupportsFloat
from colorama import Fore
from getpass import getuser
import os, sys, shutil, simple_webbrowser
from argparse import ArgumentParser, Namespace

NEED_INT_REPLACE: int = -1
NEED_REPLACE: str = 'changeme'
LOREM_IPSUM: str = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.'
DEFAULT_MESSAGE: str = 'Updated the plugin'

TITLE = """██╗    ██╗██████╗ ██╗     ██╗   ██╗ ██████╗ ███████╗               ██████╗██╗     ██╗
██║    ██║██╔══██╗██║     ██║   ██║██╔════╝ ╚══███╔╝              ██╔════╝██║     ██║
██║ █╗ ██║██████╔╝██║     ██║   ██║██║  ███╗  ███╔╝     █████╗    ██║     ██║     ██║
██║███╗██║██╔═══╝ ██║     ██║   ██║██║   ██║ ███╔╝      ╚════╝    ██║     ██║     ██║
╚███╔███╔╝██║     ███████╗╚██████╔╝╚██████╔╝███████╗              ╚██████╗███████╗██║
 ╚══╝╚══╝ ╚═╝     ╚══════╝ ╚═════╝  ╚═════╝ ╚══════╝               ╚═════╝╚══════╝╚═╝
                              
"""

TITLE_MSG = "WriterClassic  Copyright (C) 2024  MF366"

def bool_swap(v: bool) -> bool:
    return not v

def title_ascii(_ascii: str, _msg: str):
    ascii_table = _ascii.split("\n")
    cur_map_index = 0
    
    COLORMAP = (Fore.BLUE, Fore.LIGHTBLUE_EX)
    
    for ascii_line in ascii_table:
        print(COLORMAP[cur_map_index], end="")
        print(ascii_line)
        
        cur_map_index = int(bool_swap(cur_map_index))
        
    print(f'{COLORMAP[cur_map_index]}{_msg}{Fore.RESET}\n')

title_ascii(TITLE, TITLE_MSG)

class ClampingError(Exception): ...

def clamp(n: SupportsFloat, _min: SupportsFloat = 0, _max: SupportsFloat = 1000) -> SupportsFloat:
    """
    clamp clamps n between _min and _max

    Args:
        n (SupportsFloat): val
        _min (SupportsFloat, optional): min val. Defaults to 0.
        _max (SupportsFloat, optional): max val. Defaults to 1000.

    Raises:
        ClampingError: if _min is greater than _max

    Returns:
        SupportsFloat: clamped val
    """
    
    if _min > _max:
        raise ClampingError(f"Min value {_min} is greater than max value {_max}")

    if n < _min:
        return _min

    if n > _max:
        return _max

    return n

def is_between(n: SupportsFloat, _min: SupportsFloat = 0, _max: SupportsFloat = 1000) -> bool:
    """
    is_between checks if a number is between or equal to _min and _max

    Args:
        n (SupportsFloat): value to check
        _min (SupportsFloat, optional): min value. Defaults to 0.
        _max (SupportsFloat, optional): max value. Defaults to 1000.

    Returns:
        bool: whether the condition meets or not
    """
    
    if n >= _min and n <= _max:
        return True

    return False

parser = ArgumentParser("WPlugZ", None, "A powerful CLI for helping you develop your own WriterClassic plugins.")
subparsers = parser.add_subparsers(dest="command")

def action_bump(params: Namespace):
    pkg_path = os.path.abspath(os.getcwd())
    v = 1

    print(f"{Fore.CYAN}Got package located at: {Fore.YELLOW}{pkg_path}{Fore.RESET}")

    with open(os.path.join(pkg_path, 'Versions.txt'), "r", encoding="utf-8") as f:
        v = clamp(int(f.read()), 1, 1000)
        f.close()

    if not is_between(params.version, 1, 1000):
        params.version = clamp(v + 1, v, 1000)

    print(f"{Fore.CYAN}Got new version number: {Fore.YELLOW}{params.version}{Fore.RESET}")

    a = shutil.copytree(os.path.join(pkg_path, f'v{v}'), os.path.join(pkg_path, f'v{params.version}'))

    print(f"{Fore.CYAN}Copied version {v}'s dir: {Fore.YELLOW}{a}{Fore.RESET}")

    if os.path.exists(os.path.join(pkg_path, 'VERSIONING.md')):
        with open(os.path.join(pkg_path, 'VERSIONING.md'), mode='a', encoding="utf-8") as f:
            f.seek(0)
            f.write(f"\n{params.version}: {params.changes}")
            f.close()

        print(f"{Fore.CYAN}Added the changes to the VERSIONING.md: {Fore.YELLOW}{params.changes}{Fore.RESET}")

    if v < params.version:
        with open(os.path.join(pkg_path, 'Versions.txt'), "w", encoding="utf-8") as f:
            f.write(str(params.version))
            f.close()

        print(f"{Fore.CYAN}Wrote to Versions.txt: {Fore.YELLOW}Old: {v} | New: {params.version}{Fore.RESET}")

def action_del(params: Namespace) -> bool:
    """
    Returns:
        bool: whether the operation was sucessful or not
    """
    
    if not params.skip:
        print(f"{Fore.RED}Are you sure you want to remove this plugin permanently from your device?{Fore.RESET} [y/N]", end=" ")
        k = input().lower().strip()

        if k == '':
            k = 'n'

        if k[0] != 'y':
            print(f"\n{Fore.GREEN}Nothing was removed.{Fore.RESET}")
            return False

    for root, dirs, files in os.walk(os.path.abspath(os.getcwd()), topdown=False):
        for file in files:
            file_path = os.path.join(root, file)
            os.remove(file_path)
        for _dir in dirs:
            dir_path = os.path.join(root, _dir)
            os.rmdir(dir_path)

    print(f"{Fore.RED}The plugin has been removed permanently.{Fore.RESET}")
    return True

def file_extension(s: str, return_as_lower: bool = True) -> str:
    """
    file_extension gets the file extension of a path in the form of a string

    This function doesn't check if the path exists or if it's a file or any of that.

    Args:
        s (str): the path
        return_as_lower (bool, optional): whether the extension should be returned as lower() or not. Defaults to True.

    Returns:
        str: the file extension
    """
    
    a = s.split('.')
    b = a[-1]

    if return_as_lower:
        b = b.lower()

    return b

def path_check(__s: str):
    if not os.path.exists(__s):
        return False
    
    if not os.path.isfile(__s):
        return False
    
    if not __s.lower().endswith('.py') and not __s.lower().endswith('.exe'):
        return False
    
    return True

def action_test(params: Namespace) -> bool:
    """
    Returns:
        bool: whether the operation was sucessful or not
    """
    
    pkg_path = os.path.abspath(os.getcwd())

    if not path_check(params.path):
        print(f"{Fore.RED}Doesn't exist, is not a file or doesn't match any of the possible extensions (*.py, *.exe).{Fore.RESET}")
        return False

    if not is_between(params.version, 1, 1000):
        with open(os.path.join(pkg_path, 'Versions.txt'), "r", encoding="utf-8") as f:
            params.version = clamp(int(f.read()), 1, 1000)
            f.close()

    print(f"{Fore.CYAN}Got the test version: {Fore.YELLOW}{params.version}{Fore.RESET}")

    writerclassic_path: tuple[str] = (params.path, file_extension(params.path))
    writerclassic_dir = os.path.dirname(writerclassic_path[0])
    plugins_dir = os.path.join(writerclassic_dir, 'plugins')

    names: list[str] = [f"plugin_{i}" for i in range(1, 1001)]
    d = ''

    for name in names:
        _path = os.path.join(plugins_dir, name)
        d = name

        if os.path.exists(_path):
            continue

        break

    a = shutil.copytree(os.path.join(pkg_path, f'v{params.version}'), _path)
    
    print(f"{Fore.CYAN}Ready to launch WriterClassic, plugin setup: {Fore.YELLOW}{a}{Fore.RESET}")
    
    pycmd = f'python3 {writerclassic_path[0]}'
    
    if sys.platform == 'win32':
        if writerclassic_path[1] == 'py':
            pycmd = f'python {writerclassic_path[0]}'
        
        else:
            pycmd = writerclassic_path[0]
    
    print(f"{Fore.MAGENTA}Launching WriterClassic. Once it has launched, run plugin number {Fore.RED}{d.split('_')[1]}{Fore.MAGENTA}.{Fore.RESET}\nYou can choose to remove it or not.{Fore.RESET}")
    
    if pycmd.startswith('python'):
        print(f"\n{Fore.LIGHTGREEN_EX}Output:{Fore.RESET}")
    
    os.system(pycmd)
    
    return True

def action_new(params: Namespace):
    pkg_path = os.path.abspath(params.package) if os.path.exists(params.package) and os.path.isdir(params.package) else os.path.abspath(os.getcwd())

    os.mkdir(os.path.join(pkg_path, f'v{int(clamp(params.version, 1, 1000))}'))

    version_path = os.path.join(pkg_path, f'v{int(clamp(params.version, 1, 1000))}')

    print(f"{Fore.CYAN}Created the version dir at: {Fore.YELLOW}{version_path}{Fore.RESET}")

    metadata = f"""{params.title}
{params.author}
{params.description}"""

    initial_code = """from typing import Any

def start(_globals: dict[str, Any]):
    print('Hello World')
    return _globals
"""

    README_TEXT = f'''# {params.title}
{params.description}

Made with <3 by {params.author}

## Additional Information
**This `README` file is completely optional, just like `VERSIONING.md` and `AUTHOR.md`.**
'''

    print(f"{Fore.CYAN}Generated metadata, placeholder code and initial README contents.{Fore.RESET}")

    with open(os.path.join(version_path, 'Details.txt'), 'w', encoding='utf-8') as f:
        f.write(metadata)
        f.close()

    print(f"{Fore.CYAN}Wrote the metadata.{Fore.RESET}")

    if not os.path.exists(params.icon):
        params.icon = os.path.join(os.path.dirname(__file__), 'files', 'WriterPlugin.png')

    a = shutil.copy2(os.path.abspath(params.icon), os.path.join(version_path, 'WriterPlugin.png'))

    print(f"{Fore.CYAN}Applied the icon: {Fore.YELLOW}{a}{Fore.RESET}")

    with open(os.path.join(version_path, f'{params.title}.py'), 'w', encoding='utf-8') as f:
        f.write(initial_code)
        f.close()

    print(f"{Fore.CYAN}Created the Python file with placeholder code.{Fore.RESET}")

    with open(os.path.join(pkg_path, 'Versions.txt'), 'w', encoding='utf-8') as f:
        f.write(str(clamp(params.version, 1, 1000)))
        f.close()

    print(f"{Fore.CYAN}Wrote the version data.{Fore.RESET}")

    if params.readme:
        with open(os.path.join(pkg_path, 'README.md'), 'w', encoding='utf-8') as f:
            f.write(README_TEXT)
            f.close()

        print(f"{Fore.CYAN}Created the optional README.md file.{Fore.RESET}")

    if params.versioning:
        with open(os.path.join(pkg_path, 'VERSIONING.md'), 'w', encoding='utf-8') as f:
            f.write(f"{str(clamp(params.version, 1, 1000))}: Initial Release")
            f.close()

        print(f"{Fore.CYAN}Created a VERSIONING.md file.{Fore.RESET}")

    if params.authorfile:
        with open(os.path.join(pkg_path, 'AUTHOR.md'), 'w', encoding='utf-8') as f:
            f.write(f"Plugin by: {params.author}")
            f.close()

        print(f"{Fore.CYAN}Generated an author file.{Fore.RESET}")

make_group = subparsers.add_parser('new', help='Start your plugin porjects with ease.')

make_group.add_argument("-t", "--title", type=str, help="The title/name of the plugin. This argument is required.", required=True)
make_group.add_argument("-a", "--author", type=str, help="The name of the author behind the creation of this plugin. Defaults to the current user's name.", default=getuser())
make_group.add_argument("-d", "--description", type=str, help="A simple description of what the plugin does. Defaults to a Lorem Ipsum string.", default=LOREM_IPSUM)
make_group.add_argument("-i", "--icon", type=str, help="The path to the icon of the plugin in the form of a PNG. Defaults to the WriterClassic Plugins Logo.", default=os.path.join(os.path.dirname(__file__), 'files', 'WriterPlugin.png'))
make_group.add_argument("--package", '--pkg', type=str, help="The package directory for the plugin. Defaults to the working directory.", default=os.path.abspath(os.getcwd()))
make_group.add_argument("-v", "--version", type=int, help="The version of the plugin. If this is a new plugin, you can ignore this flag. If the plugin already had a previous version, then you can use this flag. This argument is optional, defaults to 1. It will be clamped between 1 and 1000.", default=1)
make_group.add_argument("--authorfile", action='store_true', help="This argument indicates that an AUTHOR.md file should be generated. Such file is optional, like this argument.", default=False)
make_group.add_argument("--versioning", action='store_true', help="This argument indicates that a VERSIONING.md file containing changelogs should be generated. Such file is optional, like this argument.", default=False)
make_group.add_argument("--readme", action='store_true', help="This argument indicates that a README.md file containing general info about the plugin should be generated. Such file is optional, like this argument.", default=False)

test_group = subparsers.add_parser('test', help='Is your plugin working? Test its behavior with WriterClassic.')

test_group.add_argument("-p", "--path", type=str, help="The path to WriterClassic. If you use the Python version, you'll be able to see the console output. This argument is required.", required=True)
test_group.add_argument("-v", '--version', type=int, help="The version to test. Defaults to the one in the Versions.txt file.", default=NEED_INT_REPLACE)

delete_group = subparsers.add_parser('remove', help='Remove your plugin safely and quickly.')

delete_group.add_argument("--skip", action='store_true', help="Skip confirmation.", default=False)

bump_group = subparsers.add_parser('bump', help='Bump your plugin to the next version.')

bump_group.add_argument("-c", '--changes', type=str, help="The description of the changes that will be made.", default=DEFAULT_MESSAGE)
bump_group.add_argument('-v', '--version', type=int, help="The version to bump to. Defaults to the next after Versions.txt.", default=NEED_INT_REPLACE)

parser.add_argument("--info", "-w", action='store_true', help="Need a hand?", default=False)

data = parser.parse_args()

if data.info:
    simple_webbrowser.Website('https://github.com/MF366-Coding/WriterClassic/wiki/Plugin-API-(v10.1.1-)')
    sys.exit()

match data.command:
    case 'new':
        action_new(data)

    case 'bump':
        action_bump(data)

    case 'remove':
        action_del(data)

    case 'test':
        action_test(data)

print(f"\n\n{Fore.YELLOW}Hit ENTER to leave...{Fore.RESET}")
input()
sys.exit()
