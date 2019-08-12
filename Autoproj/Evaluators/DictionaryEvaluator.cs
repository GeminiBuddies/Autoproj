using System.Collections.Generic;
using GeminiLab.Autoproj.Processors;

namespace GeminiLab.Autoproj.Evaluators {
    public class DictionaryEvaluator : IExpressionEvaluator {
        protected IReadOnlyDictionary<string, string> dict;
        public DictionaryEvaluator(IReadOnlyDictionary<string, string> dict) {
            this.dict = dict;
        }

        public bool TryEvaluate(out string result, ProcessorEnvironment env, string command, params string[] param) {
            if (dict.TryGetValue(command, out result)) return true;
            return false;
        }
    }
}
