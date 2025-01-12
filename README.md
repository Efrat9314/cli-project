# CLI Tool for Code Bundling

## Overview
This CLI tool is designed to streamline the process of bundling multiple code files into a single file. This is particularly useful for scenarios like submitting assignments where the instructor needs all code files consolidated into one.

The tool utilizes the `System.CommandLine` library for .NET to parse and execute commands provided via the command line. The library supports options, arguments, and response files for efficient command execution.

---

## Installation

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd <repository-folder>
   ```

2. Ensure you have [.NET SDK](https://dotnet.microsoft.com/) installed on your system.

3. Install dependencies using NuGet:
   ```bash
   dotnet add package System.CommandLine --prerelease
   ```

4. Build the project:
   ```bash
   dotnet build
   ```

---

## Usage

The tool provides two main commands: `bundle` and `create-rsp`. Detailed usage instructions are outlined below.

### Command: `bundle`

Bundles multiple code files from specified directories into a single output file.

#### Syntax:
```bash
fim bundle --source <source-folder> --output <output-file> [--filter <file-extension>]
```

#### Options:
- `--source` (required): The directory containing code files to bundle.
- `--output` (required): The output file path for the bundled content.
- `--filter` (optional): Specify file extensions to include (e.g., `.cs`, `.js`).

#### Example:
```bash
fim bundle --source ./Projects --output ./MergedCode.cs --filter .cs
```
This command bundles all `.cs` files in the `Projects` directory and its subdirectories into a file named `MergedCode.cs`.

---

### Command: `create-rsp`

Generates a response file to simplify repeated execution of commands.

#### Syntax:
```bash
fim create-rsp --commands <command-arguments> --file <response-file>
```

#### Options:
- `--commands` (required): The command arguments to store in the response file.
- `--file` (required): The response file path to create.

#### Example:
```bash
fim create-rsp --commands "bundle --source ./Code --output ./AllCode.cs" --file ./bundle.rsp
```
This command creates a response file named `bundle.rsp` with the specified bundling command.

#### Using Response Files:
You can invoke commands using a response file by prefixing the file path with `@`:
```bash
fim @bundle.rsp
```

---

## Response Files

### Overview
Response files allow storing a sequence of command-line arguments for reuse. They simplify long or repetitive commands and make it easier to maintain consistency.

### Syntax
1. Tokens are separated by spaces.
2. Multi-word tokens should be enclosed in double quotes (`"`).
3. Lines starting with `#` are treated as comments.

### Example Response File:
```
--source ./Code
--output ./AllCode.cs
--filter .cs
```

---

## Notes

- The tool supports both Windows and Unix-based systems.
- Ensure proper file permissions when working with output files.
- Refer to the [official System.CommandLine documentation](https://learn.microsoft.com/en-us/dotnet/standard/commandline/syntax) for further details.
