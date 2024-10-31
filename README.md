<p align="center">
  <a title="NuGet download" target="_blank" href="https://www.nuget.org/packages/Xsl.Transformer"><img src="https://img.shields.io/nuget/dt/Xsl.Transformer"></a>
  <a title="NuGet download" target="_blank" href="https://www.nuget.org/packages/Xsl.Transformer"><img alt="NuGet" src="https://img.shields.io/nuget/v/Xsl.Transformer"></a>
  <img alt="GitHub last commit" src="https://img.shields.io/github/last-commit/armanossiloko/xsl-transformer">
  <img alt="GitHub repo size" src="https://img.shields.io/github/repo-size/armanossiloko/xsl-transformer">
  <a title="MIT License" target="_blank" href="https://licenses.nuget.org/MIT"><img src="https://img.shields.io/github/license/armanossiloko/xsl-transformer"></a>
</p>

# XSL transformer

Since IE is being retired, it is getting a bit harder and more annoying to view IIS' failed request logs (which I have the deep pleasure of having to view very often), I decided to help myself by creating a simple tool that will transform my failed request logs (or any other transformation data, usually in XML format) into a proper readable file format via an XSL template (in my case, IIS' failed request logs in XML to browser readable HTML so I can easily view them without Internet Explorer).

# Installation:

### By building it on your own:
1. Open command line in the Xsl.Transformer directory
2. Run `dotnet pack`
3. Install the tool using `dotnet tool install --global --add-source ./nupkg Xsl.Transformer`

### Via NuGet:
```
dotnet tool install --global Xsl.Transformer
```

In order to remove, use:
```
dotnet tool uninstall --global Xsl.Transformer
```

## Usage:

```
DESCRIPTION:
Transforms a given data file (usually in .XML format, e.g an IIS failed request log file) into a human or browser
readable file (e.g HTML, XLS, etc.).

You can control the template by either using one of the available flags or by setting an environment variable in your system.
Just set the environment variable XSLT_TEMPLATE_PATH on machine level to be the full path to the template file.

USAGE:
    xslt transform <input> [OPTIONS]

EXAMPLES:
    xslt transform data.xml -o transformed.html
    xslt transform data.xml -t template.xslt -o transformed.html
    xslt transform data.xml --template template.xslt --output transformed.html

ARGUMENTS:
    <input>    Full path to the file to transform

OPTIONS:
    -h, --help                   Prints help information
    -o, --output <FILENAME>      Full path or name of a file to be generated (preferably that doesn't exist; else it
                                 will be overwritten)
    -t, --template <FILENAME>    Full path to an XSL template file; can be skipped when a default value is set using an
                                 environment variable named XSLT_TEMPLATE_PATH
```

Windows users, aside from that, can additionally define a template path (I'll use IIS' template for transforming failed request logs into HTML as an example):

- Save the freb.xsl (rename it to what you want if needed) somewhere on your PC
- Preferably set up an environment variable (`XSLT_TEMPLATE_PATH`) to the value of that location (e.g `C:\Windows\System32\freb.xsl`).
- Install the tool by either building it on your own or from NuGet.org (see above).

## Credits

This project utilizes the following open-source libraries, please star them on Github and contribute to them, if able to:
- [Spectre.Console](https://github.com/spectreconsole/spectre.console): A .NET library that makes it easier to create beautiful console applications.


## Contributing

Got suggestions, found a bug or would like to contribute? If so, feel free to create issues or pull requests. Please do make sure to try to write clear and concise commit messages.

## Support
If you find this project helpful, you can support me on [PayPal.me](https://paypal.me/armanossiloko).

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
