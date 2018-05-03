using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GeminiLab.Autoproj {
    internal static class FileProcessor {
        public static Regex reg = new Regex(@"<~(?<content>[^<~]*)~>");

        public static string ProcessFile(FileInfo file, AutoprojEnv env) {
            StreamReader sr = null;
            try {
                long line = 0;
                long outputline = 0;

                // as we know what we are doing...
                // ReSharper disable AccessToModifiedClosure
                env.TryAddFunction("line", any => line.ToString());
                env.TryAddFunction("outputline", any => outputline.ToString());
                // ReSharper restore AccessToModifiedClosure

                sr = new StreamReader(file.OpenRead(), Encoding.UTF8);

                var sb = new StringBuilder();

                foreach (var l in sr.GetLines()) {
                    ++line;
                    if (l.Length > 6 && l.Substring(0, 3) == "<~~" && l.Substring(l.Length - 3, 3) == "~~>") {
                        handleCommand(l.Substring(3, l.Length - 6), env);
                    } else {
                        ++outputline;

                        sb.AppendLine(reg.Replace(l, match => matchEvaluator(match, env)));
                    }
                }

                sr.Close();
                
                return sb.ToString();
            } catch (Exception ex) {
                // todo: log here
                return null;
            } finally {
                sr?.Dispose();
            }
        }

        private static void handleCommand(string command, AutoprojEnv env) {
            var parameters = command.Trim().Split().RemoveEmpty().ToArray();

            if (parameters[0] == "counter") {
                if (parameters.Length < 2) return;

                string name = parameters[1];
                ulong initv;

                if (parameters.Length == 2)
                    initv = 0;
                else if (parameters.Length > 3 || !ulong.TryParse(parameters[2], out initv))
                    return;

                env.TryAddCounter(name, initv);
            } else if (parameters[0] == "static_counter") {
                if (parameters.Length < 2) return;

                string name = parameters[1];
                ulong initv;

                if (parameters.Length == 2)
                    initv = 0;
                else if (parameters.Length > 3 || !ulong.TryParse(parameters[2], out initv))
                    return;

                if (env.TryGetTypeLocal(name, out var type) && type == AutoprojEnvItemType.StaticCounter) return;
                env.TryAddStaticCounter(name, initv);
            } else if (parameters[0] == "const") {
                if (parameters.Length != 3) return;

                string name = parameters[1];
                string value = parameters[2];

                env.TryAddConst(name, value);
            } else if (parameters[0] == "assign") {
                if (parameters.Length < 3) return;

                string name = parameters[1];
                string value = parameters[2];
                string[] param = parameters.Skip(3).ToArray();

                if (!env.TryConvert(value, param, out var result)) return;
                env.TrySetVariable(name, result);
            }
        }

        private static string matchEvaluator(Match match, AutoprojEnv env) {
            var total = match.Value;
            var parameters = match.Groups["content"].Value.Trim().Split().RemoveEmpty().ToArray();

            if (parameters.Length == 0) return "";

            if (env.TryConvert(parameters[0], parameters.Skip(1).ToArray(), out string result)) {
                return result;
            }

            return parameters.Length == 1 ? parameters[0] : total;
        }
    }
}
