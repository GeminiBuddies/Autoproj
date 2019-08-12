using System;
using System.IO;

using GeminiLab.Core2.GetOpt;
using GeminiLab.Core2.Logger;
using GeminiLab.Core2.Logger.Appenders;
using GeminiLab.Core2.Logger.Layouts;

using GeminiLab.Autoproj.Components;
using GeminiLab.Autoproj.Evaluators;
using GeminiLab.Autoproj.Processors;

namespace GeminiLab.Autoproj {
    static class EntryPoint {
        public static int Main(string[] args) {
            var opt = new OptGetter() { EnableDashDash = true };

            opt.AddOption('p', OptionType.Parameterized, "path");
            opt.AddOption('s', OptionType.Parameterized, "suffix");
            opt.AddOption('t', OptionType.Parameterized, "storage");
            opt.AddOption('l', OptionType.Parameterized, "log-level");
            opt.AddOption('q', OptionType.Switch, "quiet");
            opt.AddOption('c', OptionType.Switch, "chatty");
            opt.AddOption('f', OptionType.Parameterized, "timestamp");
            opt.AddOption('h', OptionType.Switch, "help");

            opt.BeginParse(args);

            GetOptError err;
            bool quiet = false, chatty = false;
            string path = Environment.CurrentDirectory;
            string suffix = StaticConfig.DefaultTemplateSuffix, storage = StaticConfig.DefaultStorageSuffix;
            string timestamp = StaticConfig.DefaultTimestampFormat;
            int logLevel = -1;
            string[] predefined = null;
            while ((err = opt.GetOpt(out var result)) != GetOptError.EndOfArguments) {
                if (result.Option == 'h') {
                    Console.WriteLine(HelpInfo.HelpText);
                    return 0;
                }

                if (result.Option == 'q') {
                    quiet = true;
                } else if (result.Option == 'c') {
                    chatty = true;
                } else if (result.Option == 'f') {
                    timestamp = result.Parameter ?? timestamp;
                } else if (result.Option == 'l') {
                    if (result.Parameter == "all") logLevel = Logger.LevelAll;
                    if (result.Parameter == "trace") logLevel = Logger.LevelTrace;
                    if (result.Parameter == "debug") logLevel = Logger.LevelDebug;
                    if (result.Parameter == "warn") logLevel = Logger.LevelWarn;
                    if (result.Parameter == "error") logLevel = Logger.LevelError;
                    if (result.Parameter == "fatal") logLevel = Logger.LevelFatal;
                    if (result.Parameter == "off") logLevel = Logger.LevelOff;
                } else if (result.Option == 'p') {
                    path = result.Parameter ?? path;
                } else if (result.Option == 's') {
                    suffix = result.Parameter ?? suffix;
                } else if (result.Option == 't') {
                    storage = result.Parameter ?? storage;
                } else {
                    if (result.Type == GetOptResultType.Values) {
                        predefined = result.Parameters;
                    }
                }
            }

            if (logLevel == -1) {
                logLevel = quiet ? Logger.LevelOff :
                    chatty ? Logger.LevelAll : Logger.LevelWarn;
            }

            using (var loggerContext = new LoggerContext()) {
                var layout = new ColoredConsoleLayout(timestamp);
                loggerContext.AddAppender("console", new ColoredConsoleAppender(layout));
                loggerContext.AddCategory("autoproj");
                loggerContext.Connect("autoproj", "console", Filters.Threshold(logLevel));
                var logger = loggerContext.GetLogger("autoproj");
                logger.Info("Logger initialized.");

                var varComp = new VariableComponent();
                var counterComp = new CounterComponent();

                var rootEnv = new ProcessorEnvironment(null);
                var processorConfig = new ProcessorConfig(suffix, storage, logger, rootEnv);

                rootEnv.AddEvaluator(new UserPredefinedEvaluator(predefined ?? Array.Empty<string>()));
                rootEnv.AddEvaluator(new AutoprojPredefinedEvaluator());
                rootEnv.AddEvaluator(varComp);
                rootEnv.AddEvaluator(counterComp);

                processorConfig.AddCommandHandler(varComp, "def");
                processorConfig.AddCommandHandler(counterComp, "counter", "static_counter");

                Processor.ProcessDirectory(new DirectoryInfo(path), processorConfig.RootEnv, processorConfig);

                logger.Info("All jobs done. closing logger...");
            }

            return 0;
        }
    }
}
