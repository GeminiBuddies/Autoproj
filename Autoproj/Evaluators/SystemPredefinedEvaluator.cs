using System;
using System.Collections.Generic;
using GeminiLab.Autoproj.Processors;

namespace GeminiLab.Autoproj.Evaluators {
    public class SystemPredefinedEvaluator : IExpressionEvaluator {
        public bool TryEvaluate(out string result, ProcessorEnvironment env, string command, params string[] param) {
            switch (command) {
            case "autoprojver":
                result = Metadata.VersionString;
                return true;
            case "now":
                result = DateTime.Now.ToString(param.Length == 0 ? "yyyy/MM/dd HH:mm:ss" : param[0]);
                return true;
            default:
                result = null;
                return false;
            }
        }
    }
}
