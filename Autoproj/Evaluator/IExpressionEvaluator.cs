using System;
using System.Collections.Generic;
using System.Text;
using GeminiLab.Autoproj.Processor;

namespace GeminiLab.Autoproj.Evaluator {
    public interface IExpressionEvaluator {
        bool TryEvaluate(out string result, ProcessorEnvironment env, string command, params string[] param);
    }
}
