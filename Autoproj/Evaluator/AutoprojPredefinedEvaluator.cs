using System;
using System.Collections.Generic;

namespace GeminiLab.Autoproj.Evaluator {
    public class AutoprojPredefinedEvaluator : DictionaryEvaluator {
        private static readonly Dictionary<string, string> Predefined = new Dictionary<string, string> {
            ["autoprojver"] = Metadata.VersionString,
        };

        public AutoprojPredefinedEvaluator(params string[] predefined) : base(Predefined) { }
    }
}