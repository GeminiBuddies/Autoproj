using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GeminiLab.Autoproj.Evaluator;
using GeminiLab.Autoproj.Handler;
using GeminiLab.Autoproj.Processor;

namespace GeminiLab.Autoproj.Component {
    public class CounterComponent : IExpressionEvaluator, ICommandHandler {
        public bool TryEvaluate(out string result, ProcessorEnvironment env, string command, params string[] param) {
            throw new NotImplementedException();
        }

        public void Handle(string command, string[] param, ProcessorEnvironment env, TextWriter output) {
            throw new NotImplementedException();
        }
    }
}
