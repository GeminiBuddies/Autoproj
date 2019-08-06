using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GeminiLab.Autoproj.Processor;

namespace GeminiLab.Autoproj.Handler {
    public interface ICommandHandler {
        void Handle(string command, string[] param, ProcessorEnvironment env, TextWriter output);
    }
}
