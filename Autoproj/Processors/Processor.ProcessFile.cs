using System.IO;
using System.Text;

namespace GeminiLab.Autoproj.Processors {
    internal static partial class Processor {
        public static void ProcessFile(FileInfo file, FileInfo output, ProcessorEnvironment env,
            ProcessorConfig options) {
            var logger = options.Logger;
            logger.Info($"Processing file '{file.FullName}'...");

            // As StreamReader/Writers dispose inner Streams when disposing...
            using (var sr = new StreamReader(file.OpenRead(), Encoding.UTF8)) {
                using (var sw = output == null ? TextWriter.Null : new StreamWriter(output.OpenWrite())) {
                    sw.NewLine = "\n";

                    Processor.ProcessContent(sr, sw, env, options, file.FullName);
                }
            }
        }
    }
}
