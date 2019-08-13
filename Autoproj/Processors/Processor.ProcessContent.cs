using System;
using System.Collections.Generic;
using System.IO;
using GeminiLab.Autoproj.Handlers;
using GeminiLab.Autoproj.IO;

namespace GeminiLab.Autoproj.Processors {
    internal static partial class Processor {
        public static void ProcessContent(TextReader reader, TextWriter writer, ProcessorEnvironment env, ProcessorConfig options, string unitName) {
            ProcessLines(new TextReaderLineSource(reader), new TextWriterLineAcceptor(writer), env, options, unitName);
        }
    }
}
