using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GeminiLab.Autoproj.Processors {
    internal static partial class Processor {
        private static char translateEscapeChar(char c) {
            if (c == '"') return '"';
            if (c == '\\') return '\\';
            if (c == '0') return '\0';
            if (c == 'n') return '\n';
            if (c == 'r') return '\r';
            if (c == 't') return '\t';
            if (c == 'b') return '\b';
            if (c == 'v') return '\v';
            if (c == 'a') return '\a';
            if (c == 'f') return '\f';

            return c;
        }

        public static void SplitArguments(string str, out string command, out string[] args) {
            var list = new List<string>();

            int len = str.Length;
            int ptr = 0;

            while (ptr < len) {
                if (char.IsWhiteSpace(str, ptr)) {
                    ++ptr; continue;
                }

                if (str[ptr] == '"') {
                    StringBuilder sb = new StringBuilder();

                    ++ptr;
                    for (;;) {
                        if (ptr == len || str[ptr] == '"') {
                            list.Add(sb.ToString());

                            if (ptr < len) ++ptr;
                            break;
                        }

                        if (str[ptr] == '\\') {
                            ++ptr;
                            sb.Append(ptr < len ? translateEscapeChar(str[ptr]) : '\\');
                        } else {
                            sb.Append(str[ptr]);
                        }

                        ++ptr;
                    }
                } else {
                    int start = ptr;
                    do ++ptr; while (ptr < len && !char.IsWhiteSpace(str, ptr));

                    list.Add(str.Substring(start, ptr - start));
                }
            }

            if (list.Count == 0) {
                command = "";
                args = Array.Empty<string>();
            } else {
                command = list[0];
                args = new string[list.Count - 1];
                list.CopyTo(1, args, 0, list.Count - 1);
            }
        }
    }
}
