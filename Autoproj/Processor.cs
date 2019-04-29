using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GeminiLab.Core2;
using GeminiLab.Core2.Logger;

namespace GeminiLab.Autoproj {
    internal static class Processor {
        public static void ProcessDirectory(DirectoryInfo directory, AutoprojEnv parentEnv, CommandlineOptions options) {
            Program.Logger.Info($"entering directory '{directory.FullName}'...");

            var thisEnv = AutoprojEnv.GetDirectoryEnv(parentEnv, directory, options);
            thisEnv.Begin();

            var rootFile = new FileInfo(Path.Combine(directory.FullName, options.TemplateExtension));
            if (rootFile.Exists) ProcessFile(rootFile, thisEnv, null);

            foreach (var file in directory.EnumerateFiles()) {
                if (file.Extension == options.TemplateExtension && file.Name != options.TemplateExtension) {
                    string filePath = file.FullName;
                    string outputPath = filePath.Substring(0, filePath.Length - options.TemplateExtension.Length);
                    // string storagePath = outputPath + options.TemplateJsonExtension;

                    var fileEnv = AutoprojEnv.GetFileEnv(thisEnv, file, options);
                    fileEnv.Begin();
                    ProcessFile(file, fileEnv, outputPath);
                    fileEnv.End();
                }
            }

            foreach (var dir in directory.EnumerateDirectories()) {
                ProcessDirectory(dir, thisEnv, options);
            }

            thisEnv.End();

            Program.Logger.Info($"leaving directory '{directory.FullName}'...");
        }

        private static readonly Regex Reg = new Regex(@"<~(?<content>[^<~>]*)~>");

        public static void ProcessFile(FileInfo file, AutoprojEnv env, string outputfile) {
            Program.Logger.Info($"processing file '{_currentFilename = file.FullName}'...");

            var sr = new StreamReader(file.OpenRead(), Encoding.UTF8);
            var sw = outputfile != null ? new StreamWriter(new FileStream(outputfile, FileMode.Create, FileAccess.Write), new UTF8Encoding(false)) : null;

            ProcessText(env, sr, sw);

            sw?.Close();
            sr.Close();
        }

        private static string _currentFilename;
        private static long _ifln, _ofln;

        public static void ProcessText(AutoprojEnv env, TextReader reader, TextWriter writer) {
            _ifln = 0; _ofln = 0;

            // as we know what we are doing...
            // ReSharper disable AccessToModifiedClosure
            env.TryAddFunction("ifln", any => $"{_ifln}");
            env.TryAddFunction("ofln", any => $"{_ofln}");
            // ReSharper restore AccessToModifiedClosure

            string l;
            while ((l = reader.ReadLine()) != null) {
                ++_ifln;

                if (l.Length > 6 && l.Substring(0, 3) == "<~~" && l.Substring(l.Length - 3, 3) == "~~>") {
                    var command = l.Substring(3, l.Length - 6).Trim();

                    try {
                        handleCommand(command, env);
                    } catch (Exception ex) {
                        Program.Logger.Error($"failed to handle command '{command}' at {_currentFilename}:line {_ifln}, {ex.GetType().FullName}: {ex.Message}.");
                        Program.Logger.Debug($"stack trace is {ex.StackTrace}.");
                    }
                } else {
                    ++_ofln;

                    string result;
                    try {
                        result = Reg.Replace(l, match => matchEvaluator(match, env));
                    } catch (Exception ex) {
                        Program.Logger.Error($"failed to translate line '{l}' at {_currentFilename}:line {_ifln}, {ex.GetType().FullName}: {ex.Message}, this line is ignored.");
                        Program.Logger.Debug($"stack trace is {ex.StackTrace}.");
                        --_ofln; continue;
                    }

                    writer?.WriteLine(result);
                }
            }
        }

        private static void handleCommand(string command, AutoprojEnv env) {
            var parameters = command.Split().RemoveEmpty().ToArray();

            if (parameters[0] == "counter") {
                if (parameters.Length < 2) return;

                string name = parameters[1];
                long initv;

                if (parameters.Length == 2)
                    initv = 0;
                else if (parameters.Length > 3 || !long.TryParse(parameters[2], out initv))
                    return;

                env.TryAddCounter(name, initv);
            } else if (parameters[0] == "static_counter") {
                if (parameters.Length < 2) return;

                string name = parameters[1];
                long initv;

                if (parameters.Length == 2)
                    initv = 0;
                else if (parameters.Length > 3 || !long.TryParse(parameters[2], out initv))
                    return;

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
                env.TryAssign(name, result);
            } else if (parameters[0] == "raise") {
                throw new Exception(parameters.Skip(1).JoinBy(" "));
            } else {
                Program.Logger.Warn($"unknown command '{command}' at {_currentFilename}:line {_ifln}, continue anyway.");
                return;
            }

            Program.Logger.Debug($"command '{command}' successfully handled.");
        }

        private static string matchEvaluator(Match match, AutoprojEnv env) {
            var total = match.Value;
            var parameters = match.Groups["content"].Value.Trim().Split().RemoveEmpty().ToArray();

            if (parameters.Length == 0) return "";

            if (env.TryConvert(parameters[0], parameters.Skip(1).ToArray(), out string result)) {
                Program.Logger.Debug($"'{total}' at {_currentFilename}:line {_ifln} evaluates to '{result}'");
                return result;
            }

            Program.Logger.Warn($"no method to evaluate '{total}' at {_currentFilename}:line {_ifln}, leave it as it is.");
            return total;
        }
    }
}
