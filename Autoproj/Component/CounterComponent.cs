using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GeminiLab.Autoproj.Evaluator;
using GeminiLab.Autoproj.Handler;
using GeminiLab.Autoproj.Processor;

namespace GeminiLab.Autoproj.Component {
    // this component contains:
    // - counter
    // - static counter
    public class CounterComponent : IExpressionEvaluator, ICommandHandler {
        private const string Category = "counter";

        public bool TryEvaluate(out string result, ProcessorEnvironment env, string command, params string[] param) {
            if (env.TryFindStorageRecursively<int>(Category, command, out var item) && item.Exists) {
                // item.Exists in condition seems to be redundant, but just keep it here
                result = item.Value.ToString();
                item.Value += 1;
                return true;
            }

            result = null;
            return false;
        }

        public void Handle(string command, string[] param, ProcessorEnvironment env, TextWriter output) {
            if (command == "counter" && param.Length >= 1) {
                var key = param[0];
                var value = param.Length >= 2 ? int.TryParse(param[1], out var result) ? result : 0 : 0;

                env.OpenStorage<int>(Category, key).Value = value;
            }

            if (command == "static_counter" && param.Length >= 1) {
                var key = param[0];
                var value = param.Length >= 2 ? int.TryParse(param[1], out var result) ? result : 0 : 0;

                var item = env.OpenStorage<int>(Category, key);
                if (!item.Exists) item.Value = value;
            }
        }
    }
}
