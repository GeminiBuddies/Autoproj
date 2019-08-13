using System.IO;
using GeminiLab.Autoproj.IO;
using GeminiLab.Autoproj.Processors;

namespace GeminiLab.Autoproj.Handlers {
    public interface ICommandHandler {
        void Handle(string command, string[] param, ProcessorEnvironment env, ProcessorConfig options, ILineAcceptor output);
    }
}
