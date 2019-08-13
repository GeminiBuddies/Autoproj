using System.IO;
using GeminiLab.Autoproj.IO;
using GeminiLab.Autoproj.Processors;

namespace GeminiLab.Autoproj.Handlers {
    public enum BlockHandlerAcceptResult {
        Accept,
        Reject,
        End
    }

    public interface IBlockHandler {
        BlockHandlerAcceptResult BeginBlock(string command, string[] param, ProcessorEnvironment env, ProcessorConfig options, ILineAcceptor output, out IBlock block);
    }

    public interface IBlock : ILineAcceptor {
        BlockHandlerAcceptResult Accept(string command, string[] param);
        void ForceEnd();
    }
}
