using System.IO;
using GeminiLab.Autoproj.Evaluators;
using GeminiLab.Autoproj.Handlers;
using GeminiLab.Autoproj.Processors;

namespace GeminiLab.Autoproj.Components {
    // this component contains:
    // - counter
    // - static counter
    public class CounterComponent : IExpressionEvaluator, ICommandHandler {
        private const string Category = "counter";

        public bool TryEvaluate(out string result, ProcessorEnvironment env, string command, params string[] param) {
            if (env.StorageTryFindRecursively<int>(Category, command, out var item)) {
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

                var item = env.StorageOpenOrCreate(Category, key, value, false);
                item.Value = value;
            }

            if (command == "static_counter" && param.Length >= 1) {
                var key = param[0];
                var value = param.Length >= 2 ? int.TryParse(param[1], out var result) ? result : 0 : 0;

                var item = env.StorageOpenOrCreate(Category, key, value, true);
            }
        }
    }
}
