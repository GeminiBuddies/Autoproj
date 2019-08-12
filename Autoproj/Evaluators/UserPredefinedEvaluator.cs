using System.Collections.Generic;

namespace GeminiLab.Autoproj.Evaluators {
    public class UserPredefinedEvaluator : DictionaryEvaluator {
        private static Dictionary<string, string> parsePredefined(params string[] predefined) {
            var dict = new Dictionary<string, string>();

            foreach (var s in predefined) {
                int index = s.IndexOf('=');
                if (index == 0) index = s.IndexOf('=', 1);
                if (index < 0) dict[s] = "";
                if (index > 0) dict[s.Substring(0, index)] = s.Substring(index + 1);
            }

            return dict;
        }

        public UserPredefinedEvaluator(params string[] predefined) : base(parsePredefined(predefined)) { }
    }
}