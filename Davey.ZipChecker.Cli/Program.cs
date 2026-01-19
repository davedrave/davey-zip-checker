using DaveyZipChecker.Cli.Commands;
using Spectre.Console.Cli;

namespace DaveyZipChecker.Cli
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            Spectre.Console.Cli.CommandApp app = new CommandApp();

            app.Configure(config =>
            {
                config.SetApplicationName("davey-zip-checker");

                config.AddCommand<DiffCommand>("diff")
                      .WithDescription("Compare the contents of a ZIP file with a folder");


#if DEBUG
                // Helpful during development
                config.PropagateExceptions();
                config.ValidateExamples();
#endif
            });

            return app.Run(args);
        }
    }
}

