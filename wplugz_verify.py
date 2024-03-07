from argparse import ArgumentParser
from colorama import Fore, Style
import json

parser = ArgumentParser("WPlugZ CLI - Manifest Verify", "Verify if your manifest file is good or not.")

parser.add_argument('manifest', type=str, help="The manifest file you want to verify!")

args = parser.parse_args()

problems: int = 0

class _ManifestAttention:
    def __init__(self, code: int, identifier: str, color: str, _type: str) -> None:
        self._code = f"{identifier[0].upper()}{code}"
        self._COLOR = color
        self._type = _type
        
    def raise_method(self, why: str):
        global problems
        
        problems += 1
        
        return f"{self._COLOR}{Style.DIM}{self._code}: {Style.RESET_ALL}{self._type}{Fore.RESET} ({why.rstrip()})"
    

class ManifestError(_ManifestAttention):
    def __init__(self, code: int, _type: str) -> None:
        super().__init__(code, 'E', Fore.RED, _type)
        self.raise_error = self.raise_method
    
    def print_error(self, reason: str):
        print(self.raise_method(reason))
        
    @property
    def error_code(self) -> str:
        return self._code
    
    @property
    def error_type(self) -> str:
        return self._type


class ManifestWarning(_ManifestAttention):
    def __init__(self, code: int, _type: str) -> None:
        super().__init__(code, 'W', Fore.YELLOW, _type)
        self.raise_warning = self.raise_method
    
    def print_error(self, reason: str):
        print(self.raise_method(reason))
        
    @property
    def error_code(self) -> str:
        return self._code
    
    @property
    def error_type(self) -> str:
        return self._type
    

MTypeError = ManifestError(1, 'TypeError')
MInvalidVersionTag = ManifestError(2, 'InvalidVersionTag')
MRedundantUseOfUncompatible = ManifestWarning(1, 'UncompatibleByDefault')
MRedundantParameter = ManifestWarning(2, "RedundantUseOfParameter")
MForbiddenName = ManifestError(3, 'ForbiddenName')
MLongParameter = ManifestWarning(3, 'LongStringAsParameter')
MPlaceholderReminder = ManifestWarning(4, 'PlaceholderTextReminder')
MNotNeeded = ManifestWarning(5, 'UnnecessaryParameter')
MWrongCall = ManifestWarning(6, 'UsedExcludeInsteadOfUncompatible')
MExpectedSomething = ManifestError(4, "EmptyVersionObject")


with open(args.manifest, 'r', encoding='utf-8') as f:
    data: dict[str, dict] = json.load(f)
    
for version_tag, version in data.items():
    print(f'\n{Fore.BLUE} ======== {version_tag} ======== {Fore.RESET}')
    
    if type(version_tag) != str:
        MTypeError.print_error(f'Expected a string but got {type(version_tag)} as version tag')
        
    else:
        if version_tag[0] != 'v' or len(version_tag) < 2:
            MInvalidVersionTag.print_error("Expected a string starting with 'v' and with at least 2 characters but at least one of the conditions failed")
    
    if not version:
        MExpectedSomething.print_error("Empty version")
        continue
    
    for k, v in version.items():
        match k:
            case 'uncompatible':
                if type(v) != list:
                    MTypeError.print_error(f'Expected a list but got {type(v)} as list of uncompatible versions')
                
                else:
                    if not v:
                        MRedundantParameter.print_error("uncompatible was specified but is empty")
                    
                    for i in ('v9.0.0', 'v10.0.0', 'v10.1.0', 'v10.1.1', 'v10.1.2', 'v10.2.0', 'v10.3.0', 'v10.4.0', 'v10.5.0', 'v10.6.0'):
                        if i in v:
                            MRedundantUseOfUncompatible.print_error(f"'{i}' is uncompatible with manifests by default")
                            
            case 'name':
                if type(v) != str:
                    MTypeError.print_error(f'Expected a string but got {type(v)} as the name')
                
                else:
                    for i in '*«»<>$\\/':
                        if i in v:
                            MForbiddenName.print_error(f"Character '{i}' is not allowed in the 'name' parameter")

                    if len(v) > 20:
                        MLongParameter.print_error('The name is too long')
                
                        
            case 'description':
                if type(v) != str:
                    MTypeError.print_error(f'Expected a string but got {type(v)} as the description')
                    
                else:
                    if len(v) > 60:
                        MLongParameter.print_error('The description is too long')
                        
            case 'author':
                if type(v) != str:
                    MTypeError.print_error(f'Expected a string but got {type(v)} as the author parameter')
                    
            case 'imagefile':
                if type(v) != str:
                    MTypeError.print_error(f'Expected a string but got {type(v)} as the image URL')
                    
                else:
                    if v == '###':
                        MPlaceholderReminder.print_error('Use of placeholder text - ###')
                        
            case 'pyfile':
                if type(v) != str:
                    MTypeError.print_error(f'Expected a string but got {type(v)} as the Python file URL')
                    
                else:
                    if v == '###':
                        MPlaceholderReminder.print_error('Use of placeholder text - ###')
                    
            case 'zipfile':
                if type(v) != str:
                    MTypeError.print_error(f'Expected a string but got {type(v)} as the zip file URL')
                        
                if len(version.keys()) > 1:
                    MRedundantParameter.print_error("Other parameters are being used alongside 'zipfile' but it takes priority over all the others")
            
            case 'exclude':
                MWrongCall.print_error("Used 'exclude' but probably meant 'uncompatible'")
                
            case _:
                MNotNeeded.print_error(f"'{k}' isn't used by WriterClassic and can be safely removed")

print(f"\n\n{Fore.GREEN}Done!")

if problems == 0:
    print(f"{Fore.CYAN}No problems found! Great job!")
    
elif problems == 1:
    print(f"{Fore.RED}1 problem found!")

else:
    print(f"{Fore.RED}{problems} problems found!")
    
print(Fore.RESET)
