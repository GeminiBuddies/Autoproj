using System.Text.RegularExpressions;

namespace GeminiLab.Autoproj.Processors {
    internal static partial class Processor {
        private static readonly Regex Reg = new Regex(@"<~(?<content>.*?)~>");

        public static string ProcessLine(string line, ProcessorEnvironment env, ProcessorConfig options,
            string unitName, int ln) {
            var logger = options.Logger;
            return Reg.Replace(line, match => {
                var fullContent = match.Groups["content"].Value.Trim();
                SplitArguments(fullContent, out var command, out var args);

                if (env.TryEvaluate(out var evalResult, command, args)) {
                    logger.Debug($"'{fullContent}' at {unitName}:line {ln} evaluates to '{evalResult}'");
                    return evalResult;
                }

                logger.Warn($"No known method to evaluate '{fullContent}' at {unitName}:line {ln}, leave it as it is.");
                return fullContent;
            });
        }
    }
}
