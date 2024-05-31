# pwshgen - Scaffold PowerShell Core scripts
`pwshgen` is a CLI tool that creates a new empty PowerShell Core (pwsh) script from a built-in (or provided) template. Inspired by `dotnet new`. Written in C# and AOT compiled for quick execution.

## Use
Download the executable for your platform from the latest release in the `Releases` section of this repo, then run the following in the directory you want to create a new `.ps1` script in:
```sh
./pwshgen
# after adding to your PATH
pwshgen --file output
```

## Options
### --file
File name used for output. Example: `pwshgen --file NewFile` outputs a file named `NewFile.ps1`. Defaults to the name of the current directory. Appends a random int to the name if the file already exists.

### --template
Template file to use instead of the built-in default. See the `pwsh.psmd` file to see the correct syntax needed. Header names must match those listed and currently you must add every section listed (leave any you don't want in the final output blank). Can use any text file format but I recommend using the `.psmd` extension to avoid confusion with documentation markdown or data file used by your scripts/apps.

Can be set using the `PWSHGEN_TEMPLATE_FILE` environment variable.

### --creator-name
Name of the author to include in the .Notes section of the script help metadata.

Can be set using the `PWSHGEN_CREATOR_NAME` environment variable.

### --param
Name of the output scripts provided parameters.

Can be set using the `PWSHGEN_MAIN_PARAM"` environment variable.
