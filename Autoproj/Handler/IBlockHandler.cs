using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GeminiLab.Autoproj.Processor;

namespace GeminiLab.Autoproj.Handler {
    public enum BlockHandlerAcceptResult {
        Accept,
        Reject,
        End
    }

    public interface IBlockHandler {
        void Begin(string command, string[] param, ProcessorEnvironment env, TextWriter output);
        BlockHandlerAcceptResult Accept(string command, string[] param);
        void ForceEnd();
    }
}
