using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeminiLab.Autoproj.IO {
    public class TextWriterLineAcceptor : ILineAcceptor {
        private readonly TextWriter _writer;
        public TextWriterLineAcceptor(TextWriter writer) {
            _writer = writer;
        }

        public void AcceptLine(string line) {
            _writer.WriteLine(line);
        }
    }
}
