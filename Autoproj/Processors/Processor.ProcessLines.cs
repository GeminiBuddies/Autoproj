using System;
using System.Collections.Generic;
using GeminiLab.Autoproj.Handlers;
using GeminiLab.Autoproj.IO;
using GeminiLab.Core2.Logger;

namespace GeminiLab.Autoproj.Processors {
    internal static partial class Processor {
        public class LinesProcessorRootBlock : IBlock {
            private readonly string _unitName;
            private readonly ProcessorEnvironment _env;
            private readonly ProcessorConfig _options;
            private readonly ILineAcceptor _output;
            private readonly Logger _logger;

            public int LineNo { private get; set; }

            public LinesProcessorRootBlock(ILineAcceptor output, ProcessorEnvironment env, ProcessorConfig options, string unitName) {
                _unitName = unitName;
                _env = env;
                _options = options;
                _output = output;

                _logger = options.Logger;
            }

            public void AcceptLine(string line) {
                string trim = line.Trim();
                // process comments, again, in case other block handlers emit comments
                if (trim.Length > 5 && trim.StartsWith("<~#") && trim.EndsWith("~>")) {
                    _logger.Trace($"Comment '{line}' at {_unitName}:line {line}.");
                } else if (trim.Length > 5 && trim.StartsWith("<~!") && trim.EndsWith("~>")) {
                    var content = trim.Substring(3, trim.Length - 5).Trim();
                    SplitArguments(content, out var command, out var args);

                    try {
                        if (_options.TryGetCommandHandler(command, out var handler)) {
                            handler.Handle(command, args, _env, _options, _output);
                        } else {
                            _logger.Warn($"Unknown command '{command}' at {_unitName}:line {LineNo}, this line is ignored.");
                        }
                    } catch (Exception ex) {
                        _logger.Error($"Failed to handle command '{command}' at {_unitName}:line {LineNo}, {ex.GetType().FullName}: {ex.Message}.");
                        _logger.Debug($"Stacktrace is {ex.StackTrace}.");
                    }
                } else {
                    string result;
                    try {
                        result = EvaluateLine(line, _env, _options, _unitName, LineNo);
                    } catch (Exception ex) {
                        _logger.Error($"Failed to evaluate line '{line}' at {_unitName}:line {LineNo}, {ex.GetType().FullName}: {ex.Message}, this line is ignored.");
                        _logger.Debug($"Stack trace is {ex.StackTrace}.");
                        return;
                    }

                    _output.AcceptLine(result);
                }
            }

            public BlockHandlerAcceptResult Accept(string command, string[] param) {
                // always reject
                return BlockHandlerAcceptResult.Reject;
            }

            public void ForceEnd() {
                // nothing to do here
            }
        }

        public static void ProcessLines(ILineSource input, ILineAcceptor output, ProcessorEnvironment env, ProcessorConfig options, string unitName) {
            var logger = options.Logger;

            var blocks = new Stack<IBlock>();
            var rootBlock = new LinesProcessorRootBlock(output, env, options, unitName);
            blocks.Push(rootBlock);

            int line = 0;
            string l;
            while ((l = input.GetLine()) != null) {
                rootBlock.LineNo = ++line;

                string trim = l.Trim();
                if (trim.Length > 5 && trim.StartsWith("<~#") && trim.EndsWith("~>")) {
                    logger.Trace($"Comment '{l}' at {unitName}:line {line}.");
                } else if (trim.Length > 5 && trim.StartsWith("<~~") && trim.EndsWith("~>")) {
                    SplitArguments(trim.Substring(3, trim.Length - 5), out var command, out var args);
                    switch (blocks.Peek().Accept(command, args)) {
                    case BlockHandlerAcceptResult.Accept:
                        continue;
                    case BlockHandlerAcceptResult.End:
                        blocks.Pop();
                        continue;
                    case BlockHandlerAcceptResult.Reject:
                    default:
                        break;
                    }

                    if (options.TryGetBlockHandler(command, out var handler)) {
                        switch (handler.BeginBlock(command, args, env, options, blocks.Peek(), out var block)) {
                        case BlockHandlerAcceptResult.Accept:
                            blocks.Push(block);
                            break;
                        case BlockHandlerAcceptResult.End:
                            break;
                        case BlockHandlerAcceptResult.Reject:
                        default:
                            logger.Warn($"Block handler rejected to start block '{command}' at {unitName}:line {line}, this line is ignored.");
                            break;
                        }
                    } else {
                        logger.Warn($"Unknown block '{command}' at {unitName}:line {line}, this line is ignored.");
                    }
                } else {
                    blocks.Peek().AcceptLine(l);
                }
            }

            while (blocks.Count > 0) blocks.Pop().ForceEnd();
        }
    }
}
