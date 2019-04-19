using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

using CommandLine;
using CommandLine.Text;

namespace GeminiLab.Autoproj {
    internal class Program { 
        private void optMain(CommandlineOptions opt) {
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
            help.AddPreOptionsLine(" ");
            help.AddPreOptionsLine(Def.OpenSourceInfo);
            help.AddOptions(res);
            help.AddPostOptionsLine("");

            return help;
        }

        private static ParserResult<CommandlineOptions> res;

        public static void Main(string[] args) {
            var program = new Program();

            var parser = new Parser(settings => {
                settings.HelpWriter = null;
                settings.EnableDashDash = true;
            });

            res = parser.ParseArguments<CommandlineOptions>(args);
            res.WithParsed(program.optMain).WithNotParsed(errs => {
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
