using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Xsl.Transformer.Commands;

internal class TransformCommand : Command<TransformCommand.Settings>
{
	public override int Execute(CommandContext context, Settings settings)
	{
		if (settings.Input is null)
		{
			PrintError("Please provide an -i|--input argument.");
			return -1;
		}

		if (!File.Exists(settings.Input))
		{
			PrintError($"Input file {settings.Input} does not exist.");
			return -1;
		}

		settings.Template ??= Environment.GetEnvironmentVariable("XSLT_TEMPLATE_PATH", EnvironmentVariableTarget.Machine);
		if (settings.Template is null)
		{
			PrintError("Please either provide a -t|--template argument or set up a XSLT_TEMPLATE_PATH environment variable.");
			return -1;
		}

		settings.Output ??= AnsiConsole.Ask<string>("Please choose an output file name (default transformed.html): ");
		settings.Output ??= "transformed.html";

		var cancellationRequested = false;
		if (File.Exists(settings.Output))
		{
			var command = AnsiConsole.Ask<string>($"File {settings.Output} already exists. Overwrite? (Y/n): ");
			cancellationRequested = command.Equals("N", StringComparison.InvariantCultureIgnoreCase);

			if (!cancellationRequested)
			{
				File.Delete(settings.Output);
			}
		}

		if (cancellationRequested)
		{
			AnsiConsole.WriteLine("Exiting the transformation...");
			return 0;
		}

		var xsltArgs = new XsltArgumentList();

		using var ms = new FileStream(settings.Output, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		XPathDocument xpd;

		var xml = File.ReadAllText(settings.Input);
		using (var sr = new StringReader(xml))
		{
			xpd = new XPathDocument(sr);
		}

#if DEBUG
		var xslt = new XslCompiledTransform(true);
#else
		var xslt = new XslCompiledTransform(false);
#endif
		xslt.Load(settings.Template);

		xslt.Transform(xpd, xsltArgs, ms);
		ms.Seek(0, SeekOrigin.Begin);
		AnsiConsole.WriteLine($"File {settings.Output} successfully generated.");
		return 0;
	}

	public sealed class Settings : CommandSettings
	{
		[CommandArgument(0, "<input>")]
		[Description("Full path to the file to transform.")]
		public required string Input { get; set; }

		[CommandOption("-o|--output <FILENAME>")]
		[Description("Full path or name of a file to be generated (preferably that doesn't exist; else it will be overwritten).")]
		public string? Output { get; set; }

		[CommandOption("-t|--template <FILENAME>")]
		[Description("Full path to an XSL template file; can be skipped when a default value is set using an environment variable named XSLT_TEMPLATE_PATH.")]
		public string? Template { get; set; }
	}

	private static void PrintError(string error) => AnsiConsole.Write(new Text(error, new Style(Color.Red)));

}
