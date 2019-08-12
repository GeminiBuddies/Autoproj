using System.Collections.Generic;
using GeminiLab.Autoproj.Handlers;
using GeminiLab.Core2.Logger;

namespace GeminiLab.Autoproj.Processors {
    // replace it with a Record class as soon as C# 8.0 is available
    public class ProcessorConfig {
        public string TemplateSuffix { get; }
        public int TemplateSuffixLength { get; }
        public string StorageSuffix { get; }
        public Logger Logger { get; }
        public ProcessorEnvironment RootEnv { get; }

        public ProcessorConfig(string templateSuffix, string storageSuffix, Logger logger, ProcessorEnvironment rootEnv) {
            TemplateSuffix = templateSuffix;
            TemplateSuffixLength = templateSuffix.Length;
            StorageSuffix = storageSuffix;
            Logger = logger;
            RootEnv = rootEnv;
        }

        protected IDictionary<string, IBlockHandler> blockHandlers = new Dictionary<string, IBlockHandler>();
        protected IDictionary<string, ICommandHandler> commandHandlers = new Dictionary<string, ICommandHandler>();

        public virtual void AddBlockHandler(IBlockHandler blockHandler, params string[] commands) {
            foreach (var command in commands) {
                blockHandlers[command] = blockHandler;
            }
        }

        public virtual void AddCommandHandler(ICommandHandler commandHandler, params string[] commands) {
            foreach (var command in commands) {
                commandHandlers[command] = commandHandler;
            }
        }

        public virtual bool TryGetBlockHandler(string command, out IBlockHandler handler) {
            return blockHandlers.TryGetValue(command, out handler);
        }

        public virtual bool TryGetCommandHandler(string command, out ICommandHandler handler) {
            return commandHandlers.TryGetValue(command, out handler);
        }
    }
}
