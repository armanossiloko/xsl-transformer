# XSL Transformer CLI

This little CLI tool transforms XML files using XSLT templates.

## Installation

```
dotnet tool install --global Xsl.Transformer
```

## Usage

```
USAGE:
    xslt [OPTIONS] <COMMAND>

EXAMPLES:
    xslt transform data.xml -o transformed.html
    xslt transform data.xml -t template.xslt -o transformed.html
    xslt transform data.xml --template template.xslt --output transformed.html

OPTIONS:
    -h, --help    Prints help information

COMMANDS:
    transform <input>    Transforms a given data file (usually in .XML format, e.g an IIS failed request log file) into
                         a human or browser readable file (e.g HTML, XLS, etc.).

                         You can control the template by either using one of the available flags or by setting an
                         environment variable in your system.
                         Just set the environment variable XSLT_TEMPLATE_PATH on machine level to be the full path to
                         the template file
```

## Credits

This project utilizes the following open-source libraries, please star them on Github and contribute to them, if able to:
- [Spectre.Console](https://github.com/spectreconsole/spectre.console): A .NET library that makes it easier to create beautiful console applications.

## License

This project is licensed under the MIT License. See the [LICENSE](../LICENSE) file for details.
