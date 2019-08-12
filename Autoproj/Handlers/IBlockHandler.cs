using System.IO;
using GeminiLab.Autoproj.Processors;

namespace GeminiLab.Autoproj.Handlers {
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
