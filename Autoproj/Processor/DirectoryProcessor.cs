using System.IO;
using GeminiLab.Autoproj.Evaluator;

namespace GeminiLab.Autoproj.Processor {
    public static class DirectoryProcessor {
        public static void Process(DirectoryInfo directory, ProcessorEnvironment parentEnv, ProcessorConfig options) {
            var logger = options.Logger;
            logger.Info($"Entering directory '{directory.FullName}'...");

            if (!directory.Exists) {
                logger.Warn($"Directory '{directory.FullName}' doesn't exist, maybe a bad option or an internal error.");
                return;
            }

            var thisEnv = new ProcessorStoredEnvironment(parentEnv, Path.Join(directory.FullName, options.StorageSuffix));
            thisEnv.AddEvaluator(new DirectoryInfoEvaluator(directory));
            thisEnv.Begin();

            var rootFile = new FileInfo(Path.Combine(directory.FullName, options.TemplateSuffix));
            if (rootFile.Exists) FileProcessor.ProcessFile(rootFile, null, thisEnv, options);

            foreach (var file in directory.EnumerateFiles()) {
                var filename = file.Name;
                if (filename.Length > options.TemplateSuffixLength && filename.EndsWith(options.TemplateSuffix)) {
                    string filePath = file.FullName;
                    string outputPath = filePath.Substring(0, filePath.Length - options.TemplateSuffixLength);
                    var output = new FileInfo(outputPath);
                    string storagePath = outputPath + options.StorageSuffix;

                    var fileEnv = new ProcessorStoredEnvironment(thisEnv, storagePath);
                    fileEnv.AddEvaluator(new FileInfoEvaluator(file, output));
                    fileEnv.Begin();
                    FileProcessor.ProcessFile(file, output, fileEnv, options);
                    fileEnv.End();
                }
            }

            foreach (var dir in directory.EnumerateDirectories()) {
                Process(dir, thisEnv, options);
            }

            thisEnv.End();

            logger.Info($"Leaving directory '{directory.FullName}'...");
        }
    }
}