using McMaster.Extensions.CommandLineUtils;
using MusicLibraryChecker.Commands;
using System;
using System.Text;

namespace MusicLibraryChecker
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var app = new CommandLineApplication()
            {
                Name = "MusicLibraryChecker",
                Description = "Check audio library files",
            };

            app.HelpOption("-h|--help", inherited: true);

            app.Command("auralic", cfg =>
            {
                cfg.Description = "Check music library for Auralic Aries devices reading and using best practice";

                var path = cfg.Option("-p|--path", "File paths", CommandOptionType.MultipleValue);
				var recursive = cfg.Option("-r|--recursive", "Recursive directory search", CommandOptionType.NoValue);
				var fix = cfg.Option("-f|--fix", "Fix issues", CommandOptionType.NoValue);

                cfg.OnExecute(() => AuralicCommand.Execute(path.Values, recursive.HasValue(), fix.HasValue()));
            });

			app.Command("list", cfg =>
			{
				cfg.Description = "List music library";

				var path = cfg.Option("-p|--path", "File paths", CommandOptionType.MultipleValue);
				var recursive = cfg.Option("-r|--recursive", "Recursive directory search", CommandOptionType.NoValue);

				cfg.OnExecute(() => ListCommand.Execute(path.Values, recursive.HasValue()));
			});

			app.OnExecute(() =>
            {
                Console.WriteLine("Specify command");
                app.ShowHelp();
                return 1;
            });

            return app.Execute(args);
        }
    }
}
