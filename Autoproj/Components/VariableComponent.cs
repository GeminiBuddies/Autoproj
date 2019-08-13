using System.IO;
using GeminiLab.Autoproj.Evaluators;
using GeminiLab.Autoproj.Handlers;
using GeminiLab.Autoproj.IO;
using GeminiLab.Autoproj.Processors;

namespace GeminiLab.Autoproj.Components {
    // this component contains:
    // - variable definition
    // - variable evaluation
    public class VariableComponent : IExpressionEvaluator, ICommandHandler {
        private const string Category = "variable";

        public bool TryEvaluate(out string result, ProcessorEnvironment env, string command, params string[] param) {
            if (env.StorageTryFindRecursively<string>(Category, command, out var item)) {
                result = item.Value;
                return true;
            }

            result = null;
            return false;
        }

        public void Handle(string command, string[] param, ProcessorEnvironment env, ProcessorConfig options, ILineAcceptor output) {
            if (command == "def" && param.Length >= 1) {
                var key = param[0];
                var value = param.Length >= 2 ? param[1] : "";

                env.StorageOpenOrCreate<string>(Category, key).Value = Processor.EvaluateLine(value, env, options, $"<def {key}>", 0);
            }
        }
    }
}
