using System;
using System.IO;

using CommandLine;
using CommandLine.Text;

using GeminiLab.Core2.Logger;
using GeminiLab.Core2.Logger.Appenders;

namespace GeminiLab.Autoproj {
    internal class Program {
        internal static LoggerContext LoggerContext;
        internal static Logger Logger;

        private static void logOptions(CommandlineOptions opt) {
            Logger.Debug("parameters:");
            Logger.Debug($"  path: {opt.Path}");
            Logger.Debug($"  verbose: {opt.Verbose}, quiet: {opt.Quiet}");
            Logger.Debug($"  template extension: {opt.TemplateExtension}");
            Logger.Debug($"  template json extension: {opt.TemplateJsonExtension}");
        }

        private static void optMain(CommandlineOptions opt) {
            LoggerContext = new LoggerContext();
            LoggerContext.AddAppender("console", new ColorfulConsoleAppender());
            LoggerContext.AddCategory("default");

            if (opt.Verbose) {
                LoggerContext.Connect("default", "console");
            } else {
                LoggerContext.Connect("default", "console",
                    opt.Quiet ? Filters.DenyFilter : Filters.Threshold(Logger.LevelWarn));
            }

            Logger = LoggerContext.GetLogger("default");

            Logger.Debug("logger initialized.");
            logOptions(opt);

            Processor.ProcessDirectory(new DirectoryInfo(opt.Path ?? "."), AutoprojEnv.GetRootEnv(), opt);
        }

        private static string getHelpString() {
            var help = new HelpText {
                Heading = new HeadingInfo(Def.ProgramName, Def.VersionString),
                Copyright = new CopyrightInfo(Def.Author, 2018, DateTime.Now.Year),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true,
                MaximumDisplayWidth = 120
            };
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine(Def.OpenSourceInfo);
            help.AddOptions(_res);
            help.AddPostOptionsLine("");

            return help;
        }

        private static ParserResult<CommandlineOptions> _res;

        public static void Main(string[] args) {
            var parser = new Parser(settings => {
                settings.HelpWriter = null;
                settings.EnableDashDash = true;
            });

            _res = parser.ParseArguments<CommandlineOptions>(args);
            _res.WithParsed(optMain).WithNotParsed(errs => {
                foreach (var e in errs) {
                    if (e is HelpRequestedError) {
                        Console.WriteLine(getHelpString());
                    } else if (e is VersionRequestedError) {
                        Console.WriteLine(Def.VersionString);
                    } else {
                        Console.WriteLine($"Error#{e.Tag}");
                        Environment.Exit(1);
                    }
                }
            });
        }
    }
}
