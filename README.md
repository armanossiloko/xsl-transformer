# XSL transformer

Since IE is being retired, it is getting a bit harder and more annoying to view IIS' failed request logs (which I have the deep pleasure of having to view very often), I decided to help myself by creating a simple tool that will transform my failed request logs into proper HTML so I can easily view them in another browser.

# Setup:

- Save the freb.xsl (rename it to what you want if needed) somewhere on your PC
- Preferably set up an environment variable (`XSLT_TEMPLATE_PATH`) to the value of that location (e.g `C:\Windows\System32\freb.xsl`).
- Install the tool as per:

1. Open command line in the Xsl.Transformer directory
2. Run `dotnet pack`
3. Install the tool using `dotnet tool install --global --add-source ./nupkg Xsl.Transformer`

When wanting to remove, use `dotnet tool uninstall --global Xsl.Transformer` to uninstall the tool from your system.

## Usage:

```
Transforms a given IIS failed request log file (usually in .XML format) into a browser readable HTML.

Usage: xslt <input-file> [OPTION]
  -o, --output                  full direct path to a file (preferably that doesn't exist; else it will be overwritten)
  -t, --template                full direct path to an XSL template file

Example case:
    xslt freb.xml --output "C:\parsed.html"
```