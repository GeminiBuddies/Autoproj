using System.IO;
using GeminiLab.Autoproj.Processors;

namespace GeminiLab.Autoproj.Evaluators {
    public class FileInfoEvaluator : IExpressionEvaluator {
        public bool TryEvaluate(out string result, ProcessorEnvironment env, string command, params string[] param) {
            switch (command) {
            case "if":
            case "ifname":
                result = _ifname;
                return true;
            case "ifpath":
                result = _ifpath;
                return true;
            case "of":
            case "ofname":
                result = _ofname;
                return true;
            case "ofpath":
                result = _ofpath;
                return true;
            default:
                result = null;
                return false;
            }
        }

        private readonly string _ifpath, _ifname;
        private readonly string _ofpath, _ofname;
        public FileInfoEvaluator(FileInfo input, FileInfo output) {
            _ifpath = input.FullName;
            _ifname = input.Name;
            _ofpath = output.FullName;
            _ofname = output.Name;
        }
    }
}
