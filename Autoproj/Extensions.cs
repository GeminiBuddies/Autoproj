using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeminiLab.Autoproj {
    internal static class Extensions {
        public static IEnumerable<string> GetLines(this StreamReader reader) {
            string buff;
            while ((buff = reader.ReadLine()) != null) yield return buff;
        }

        public static IEnumerable<string> RemoveEmpty(this IEnumerable<string> src) {
            return src.Where(s => !string.IsNullOrEmpty(s));
        }
    }
}
