using Spectre.Console.Cli;
using Xsl.Transformer.Commands;

var app = new CommandApp();
app.Configure(config =>
{
	config.SetApplicationName("xslt");
	config.ValidateExamples();

	config.AddCommand<TransformCommand>("transform")
		.WithDescription(@"Transforms a given data file (usually in .XML format, e.g an IIS failed request log file) into a human or browser readable file (e.g HTML, XLS, etc.).

You can control the template by either using one of the available flags or by setting an environment variable in your system.
Just set the environment variable XSLT_TEMPLATE_PATH on machine level to be the full path to the template file.")
		.WithExample("transform", "data.xml", "-o", "transformed.html")
		.WithExample("transform", "data.xml", "-t", "template.xslt", "-o", "transformed.html")
		.WithExample("transform", "data.xml", "--template", "template.xslt", "--output", "transformed.html");
});

return app.Run(args);