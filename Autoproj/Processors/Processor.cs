using System;
using System.Linq;

namespace GeminiLab.Autoproj.Processors {
    internal static partial class Processor {
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
    }
}
