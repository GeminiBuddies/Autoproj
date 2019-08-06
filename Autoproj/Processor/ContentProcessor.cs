using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GeminiLab.Autoproj.Processor {
    internal static class ContentProcessor {
        private static readonly Regex Reg = new Regex(@"<~(?<content>.*?)~>");

        public static void SplitArguments(string str, out string command, out string[] args) {
            var p = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (p.Length == 0) {
                command = "";
                args = Array.Empty<string>();
            } else {
                command = p[0];
                args = p.Skip(1).ToArray();
            }
        }

        public static void Process(TextReader reader, TextWriter writer, ProcessorEnvironment env, ProcessorConfig options, string unitName) {
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
                            logger.Warn($"Unknown command '{command}' at {unitName}:line {line}, this line is ignored.");
                        }
                    } catch (Exception ex) {
                        logger.Error($"Failed to handle command '{command}' at {unitName}:line {line}, {ex.GetType().FullName}: {ex.Message}.");
                        logger.Debug($"Stacktrace is {ex.StackTrace}.");
                    }
                } else {
                    string result;
                    try {
                        var ln = line;
                        result = Reg.Replace(l, match => {
                            var fullContent = match.Groups["content"].Value.Trim();
                            SplitArguments(fullContent, out var command, out var args);

                            if (env.TryEvaluate(out var evalResult, command, args)) {
                                logger.Debug($"'{fullContent}' at {unitName}:line {ln} evaluates to '{evalResult}'");
                                return evalResult;
                            } 

                            logger.Warn($"No known method to evaluate '{fullContent}' at {unitName}:line {ln}, leave it as it is.");
                            return fullContent;
                        });
                    } catch (Exception ex) {
                        logger.Error($"Failed to evaluate line '{l}' at {unitName}:line {line}, {ex.GetType().FullName}: {ex.Message}, this line is ignored.");
                        logger.Debug($"Stack trace is {ex.StackTrace}.");
                        continue;
                    }

                    writer.WriteLine(result);
                }
            }
        }
    }
}
