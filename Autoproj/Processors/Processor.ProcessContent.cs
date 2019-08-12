using System;
using System.IO;

namespace GeminiLab.Autoproj.Processors {
    internal static partial class Processor {
        public static void ProcessContent(TextReader reader, TextWriter writer, ProcessorEnvironment env,
            ProcessorConfig options, string unitName) {
            var logger = options.Logger;

            int line = 0;
            string l;
            while ((l = reader.ReadLine()) != null) {
                ++line;

                string trim = l.Trim();
                if (trim.Length > 5 && trim.StartsWith("<~!") && trim.EndsWith("~>")) {
                    var content = l.Substring(3, l.Length - 5).Trim();
                    SplitArguments(content, out var command, out var args);

                    try {
                        if (options.TryGetCommandHandler(command, out var handler)) {
                            handler.Handle(command, args, env, writer);
                        } else {
                            logger.Warn(
                                $"Unknown command '{command}' at {unitName}:line {line}, this line is ignored.");
                        }
                    } catch (Exception ex) {
                        logger.Error(
                            $"Failed to handle command '{command}' at {unitName}:line {line}, {ex.GetType().FullName}: {ex.Message}.");
                        logger.Debug($"Stacktrace is {ex.StackTrace}.");
                    }
                } else if (trim.Length > 5 && trim.StartsWith("<~#") && trim.EndsWith("~>")) {
                    logger.Trace($"Comment '{l.Substring(3, l.Length - 5)}' at {unitName}:line {line}.");
                    continue;
                } else {
                    string result;
                    try {
                        result = ProcessLine(l, env, options, unitName, line);
                    } catch (Exception ex) {
                        logger.Error(
                            $"Failed to evaluate line '{l}' at {unitName}:line {line}, {ex.GetType().FullName}: {ex.Message}, this line is ignored.");
                        logger.Debug($"Stack trace is {ex.StackTrace}.");
                        continue;
                    }

                    writer.WriteLine(result);
                }
            }
        }
    }
}
