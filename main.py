from typing import Any, SupportsFloat
from colorama import Fore
from getpass import getuser
import os, sys, shutil, simple_webbrowser
from argparse import ArgumentParser, ArgumentError, Namespace

NEED_INT_REPLACE: int = -1
NEED_REPLACE: str = 'changeme'
LOREM_IPSUM: str = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.'
DEFAULT_MESSAGE: str = 'Updated the plugin'

class ClampingError(Exception): ...

def clamp(n: SupportsFloat, _min: SupportsFloat = 0, _max: SupportsFloat = 1000) -> SupportsFloat:
    if _min > _max:
        raise ClampingError(f"Min value {_min} is greater than max value {_max}")
    
    if n < _min:
        return _min
    
    if n > _max:
        return _max
    
    return n

parser = ArgumentParser("WPlugZ", None, "A powerful CLI for helping you develop your own WriterClassic plugins.")
subparsers = parser.add_subparsers(dest="command")

def action_new(params: Namespace):
    if params.package == NEED_REPLACE:
        params.package = params.title
        
    os.mkdir(params.package)
    
    pkg_path = os.path.abspath(params.package)
    
    os.mkdir(os.path.join(pkg_path, f'v{int(clamp(params.version, 1, 1000))}'))
    
    version_path = os.path.join(pkg_path, f'v{int(clamp(params.version, 1, 1000))}')
    
    metadata = f"""{params.title}
{params.author}
{params.description}"""

    initial_code = """from typing import Any
    
def start(_globals: dict[str, Any]):
    print('Hello World')
    return _globals
"""
    
    with open(os.path.join(version_path, 'Details.txt'), 'w', encoding='utf-8') as f:
        f.write(metadata)
        f.close()
        
    if not os.path.exists(params.icon):
        params.icon = os.path.join(os.path.dirname(__file__), 'files', 'WriterPlugin.png')
        
    shutil.copy2(os.path.abspath(params.icon), version_path)
    
    with open(os.path.join(version_path, f'{params.title}.py'), 'w', encoding='utf-8') as f:
        f.write(initial_code)
        f.close()
        
    with open(os.path.join(pkg_path, 'Versions.txt'), 'w', encoding='utf-8') as f:
        f.write(str(clamp(params.version, 1, 1000)))
        f.close()

    if params.versioning:
        with open(os.path.join(pkg_path, 'VERSIONING.md'), 'w', encoding='utf-8') as f:
            f.close()
            
    if params.authorfile:
        with open(os.path.join(pkg_path, 'AUTHOR.md'), 'w', encoding='utf-8') as f:
            f.write(f"Plugin by: {params.author}")
            f.close()

parser.add_argument("-t", "--title", type=str, help="The title/name of the plugin. This argument is required.", required=True)
parser.add_argument("-a", "--author", type=str, help="The name of the author behind the creation of this plugin. Defaults to the current user's name.", default=getuser())
parser.add_argument("-d", "--description", type=str, help="A simple description of what the plugin does. Defaults to a Lorem Ipsum string.", default=LOREM_IPSUM)
parser.add_argument("-i", "--icon", type=str, help="The path to the icon of the plugin in the form of a PNG. Defaults to the WriterClassic Plugins Logo.", default=os.path.join(os.path.dirname(__file__), 'files', 'WriterPlugin.png'))
parser.add_argument("--package", type=str, help="The package name for the plugin. Defaults to the plugin name.", default=NEED_REPLACE)
parser.add_argument("-v", "--version", type=int, help="The version of the plugin. If this is a new plugin, you can ignore this flag. If the plugin already had a previous version, then you can use this flag. This argument is optional, defaults to 1. It will be clamped between 1 and 1000.", default=1)
parser.add_argument("--authorfile", action='store_true', help="This argument indicates that an AUTHOR.md file should be generated. Such file is optional, like this argument.", default=False)
parser.add_argument("--versioning", action='store_true', help="This argument indicates that a VERSIONING.md file containing changelogs should be generated. Such file is optional, like this argument.", default=False)

parser.add_argument("-I", "-H", "--info", action='store_true', help="Need a hand?", default=False)

_command = parser.parse_args()

if _command.info:
    simple_webbrowser.Website('https://github.com/MF366-Coding/WriterClassic/wiki/Plugin-API-(v10.1.1-)')
    sys.exit()

action_new(_command)
