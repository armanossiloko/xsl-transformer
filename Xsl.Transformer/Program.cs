using System.Xml.XPath;
using System.Xml.Xsl;

void Transform(string xml, string templatePath, string outputPath = "transformed.html")
{
    var cancellationRequested = false;
    if (File.Exists(outputPath))
    {
        Console.Write($"File {outputPath} already exists. Overwrite? (Y/n): ");
        var command = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(command))
        {
            command = string.Empty;
        }

        cancellationRequested = !command.Equals("Y", StringComparison.OrdinalIgnoreCase);
    }

    if (cancellationRequested)
    {
        Console.WriteLine("Exiting the transformation...");
        return;
    }

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
    Console.WriteLine($"File {outputPath} successfully generated.");
}

void PrintTutorial()
{
    Console.WriteLine(@"Transforms a given data file (usually in .XML format, e.g IIS failed request log file) into a human or browser readable file (e.g HTML, XLS, etc.).

Usage: xslt <input-file> [OPTIONS]
  -i, --input
  -t, --template                full direct path to an XSL template file
  -o, --output                  full direct path to a file (preferably that doesn't exist; else it will be overwritten)

You can control the template by either using one of the available flags or by setting an environment variable in your system.
Just set the environment variable XSLT_TEMPLATE_PATH on machine level to be the full path to the template file.

Example case:
    xslt data.xml --output ""C:\transformed.html""
    xslt data.xml --input ""C:\template.xslt"" --output ""C:\transformed.html""
    xslt data.xml -i template.xsl -o transformed.xls
");
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
    if (string.IsNullOrWhiteSpace(inputXml) || inputXml.StartsWith('-'))
    {
        PrintError($"No transformation data provided.");
        PrintTutorial();
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
        PrintError($"File {inputXml} doesn't exist.");
        PrintTutorial();
        return;
    }

    // The rest are flags with their values
    var flags = new Dictionary<string, string>();
    if (!arguments.Any(arg => arg.StartsWith("-")))
    {
        PrintError("Invalid call, see usage below.");
        PrintTutorial();
        return;
    }

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
            PrintError("Invalid call, see usage below.");
            PrintTutorial();
            return;
        }
    }

    var path = Environment.GetEnvironmentVariable("XSLT_TEMPLATE_PATH", EnvironmentVariableTarget.Machine);
    if (string.IsNullOrWhiteSpace(path)
        && !flags.ContainsKey("-t")
        && !flags.ContainsKey("--template")
        && !flags.ContainsKey("-i")
        && !flags.ContainsKey("--input")
        )
    {
        PrintError("Please set up an XSLT_TEMPLATE_PATH environment variable or use one of the flags for defining a template path.");
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
            case "-i":
            case "--input":
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