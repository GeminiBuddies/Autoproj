using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GeminiLab.Autoproj.Processor;
using GeminiLab.Core2.Logger;

namespace GeminiLab.Autoproj.Evaluator {
    public class DirectoryInfoEvaluator : IExpressionEvaluator {
        public bool TryEvaluate(out string result, ProcessorEnvironment env, string command, params string[] param) {
            switch (command) {
            case "cwd":
            case "cwdpath":
                result = _name;
                return true;
            case "cwdname":
                result = _path;
                return true;
            default:
                result = null;
                return false;
            }
        }

        // private readonly DirectoryInfo _info;
        private readonly string _path;
        private readonly string _name;
        public DirectoryInfoEvaluator(DirectoryInfo info) {
            // _info = info;
            _path = info.FullName;
            _name = info.Name;
        }
    }
}
