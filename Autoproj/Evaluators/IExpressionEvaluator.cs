using GeminiLab.Autoproj.Processors;

namespace GeminiLab.Autoproj.Evaluators {
    public interface IExpressionEvaluator {
        bool TryEvaluate(out string result, ProcessorEnvironment env, string command, params string[] param);
    }
}
