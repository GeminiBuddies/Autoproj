using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeminiLab.Autoproj.IO {
    public class TextReaderLineSource :  ILineSource {
        private readonly TextReader _source;

        public TextReaderLineSource(TextReader source) {
            _source = source;
        }

        public string GetLine() {
            return _source.ReadLine();
        }
    }
}
