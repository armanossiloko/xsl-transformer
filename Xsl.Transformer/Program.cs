using System.Xml.XPath;
using System.Xml.Xsl;

void Transform(string xml, string templatePath, string outputPath = @"freb.html")
{
    var xsltArgs = new XsltArgumentList();

    using var ms = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    XPathDocument xpd;

    using (var sr = new StringReader(xml))
    {
        xpd = new XPathDocument(sr);
    }

#if DEBUG
    var xslt = new XslCompiledTransform(true);
#else
    var xslt = new XslCompiledTransform(false);
#endif
    xslt.Load(templatePath);

    xslt.Transform(xpd, xsltArgs, ms);
    ms.Seek(0, SeekOrigin.Begin);
}

void PrintTutorial()
{
    Console.WriteLine(@"Transforms a given IIS failed request log file (usually in .XML format) into a browser readable HTML.

Usage: xslt <input-file> [OPTION]
  -o, --output                  full direct path to a file (preferably that doesn't exist; else it will be overwritten)
  -t, --template                full direct path to an XSL template file

Example case:
    xslt freb.xml --output ""C:\parsed.html""");
}

void PrintError(string error)
{
    var @default = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(error);
    Console.ForegroundColor = @default;
}

try
{
    var arguments = args.AsQueryable();

    if (!arguments.Any())
    {
        PrintTutorial();
        return;
    }

    // First item should always be the file or help command
    var inputXml = arguments.FirstOrDefault();
    if (string.IsNullOrWhiteSpace(inputXml))
    {
        PrintError("Provide an input file");
        return;
    }

    if (inputXml.Equals("-h", StringComparison.OrdinalIgnoreCase)
        || inputXml.Equals("--help", StringComparison.OrdinalIgnoreCase)
        || inputXml.Equals("/?", StringComparison.OrdinalIgnoreCase)
        )
    {
        PrintTutorial();
        return;
    }

    if (!File.Exists(inputXml))
    {
        PrintError($"File {inputXml} doesn't exist");
        return;
    }

    // The rest are flags with their values
    var flags = new Dictionary<string, string>();
    if (arguments.Any(arg => arg.StartsWith("-")))
    {
        // Retrieve flags with their values, store in a dictionary
        var flagsWithValues = arguments.Skip(1).ToList();
        for (int i = 0; i < flagsWithValues.Count - 1; i += 2)
        {
            try
            {
                flags[flagsWithValues[i]] = flagsWithValues[i + 1];
            }
            catch
            {
                PrintError("Invalid call, see usage below");
                PrintTutorial();
                return;
            }
        }
    }
    else
    {
        PrintError("Invalid call, see usage below");
        PrintTutorial();
        return;
    }

    var path = Environment.GetEnvironmentVariable("XSLT_TEMPLATE_PATH", EnvironmentVariableTarget.Machine);
    if (string.IsNullOrWhiteSpace(path) && !flags.ContainsKey("-t") && !flags.ContainsKey("--template"))
    {
        PrintError("Please set up an XSLT_TEMPLATE_PATH environment variable");
        return;
    }

    FileInfo? pathInfo = null;
    FileInfo? exportPath = null;
    foreach (var item in flags)
    {
        switch (item.Key)
        {
            case "-o":
            case "--output":
                {
                    exportPath = new FileInfo(item.Value);
                }
                break;
            case "-t":
            case "--template":
                {
                    pathInfo = new FileInfo(item.Value);
                }
                break;
            // Add more cases as needed
            default:
                break;
        }
    }

    if (pathInfo == null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            PrintError("Please set up an XSLT_TEMPLATE_PATH environment variable");
            return;
        }

        pathInfo = new FileInfo(path);
    }

    if (exportPath == null)
    {
        PrintError(@"Provide an output path (e.g C:\Destination\freb.html)");
        return;
    }

    var xml = File.ReadAllText(inputXml);
    Transform(xml, pathInfo.FullName, exportPath.FullName);
}
catch (Exception ex)
{
    PrintError(ex.Message);
}
// xslt source.xml