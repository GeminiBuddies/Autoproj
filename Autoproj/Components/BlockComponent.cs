using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeminiLab.Autoproj.Handlers;
using GeminiLab.Autoproj.IO;
using GeminiLab.Autoproj.Processors;

namespace GeminiLab.Autoproj.Components {
    public class BlockComponent : IBlockHandler {
        private const string Category = "block";

        public BlockHandlerAcceptResult BeginBlock(string command, string[] param, ProcessorEnvironment env, ProcessorConfig options, ILineAcceptor output, out IBlock block) {
            if (command == "def_block" && param.Length >= 1) {
                block = new Block(env, param[0]);
                return BlockHandlerAcceptResult.Accept;
            }

            if (command == "ref_block" && param.Length >= 1) {
                if (env.StorageTryFindRecursively<List<string>>(Category, param[0], out var entry)) {
                    entry.Value.ForEach(output.AcceptLine);
                }

                block = null;
                return BlockHandlerAcceptResult.End;
            }

            block = null;
            return BlockHandlerAcceptResult.Reject;
        }
        
        private class Block : IBlock {
            private readonly ProcessorEnvironment _env;
            private readonly string _name;
            private readonly List<string> _lines = new List<string>();
            public Block(ProcessorEnvironment env, string name) {
                _env = env;
                _name = name;
            }

            public BlockHandlerAcceptResult Accept(string command, string[] param) {
                if (command == "end_def_block") {
                    end();
                    return BlockHandlerAcceptResult.End;
                }

                return BlockHandlerAcceptResult.Reject;
            }

            public void AcceptLine(string line) {
                _lines.Add(line);
            }

            private void end() {
                _env.StorageOpenOrCreate<List<string>>(Category, _name).Value = _lines;
            }

            public void ForceEnd() {
                end();
            }
        }
    }
}
