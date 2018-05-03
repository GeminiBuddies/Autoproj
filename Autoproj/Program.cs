using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace GeminiLab.Autoproj {
    internal static class Program {
        private static void initEnv(AutoprojEnv rootEnv) {
            rootEnv.TryAddConst("lcommand", "<~~");
            rootEnv.TryAddConst("rcommand", "~~>");
            rootEnv.TryAddConst("lreplace", "<~");
            rootEnv.TryAddConst("rreplace", "~>");

            rootEnv.TryAddConst("autoprojver", typeof(Program).Assembly.GetName().Version.ToString());
            rootEnv.TryAddFunction("now", param => param.Length == 1 ? DateTime.Now.ToString(param[0]) : DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }

        private static void enumerateDir(DirectoryInfo directory, AutoprojEnv env) {
            var thisEnv = new AutoprojEnv(env, directory);

            var rootFile = new FileInfo(Path.Combine(directory.FullName, ".autoproj"));
            if (rootFile.Exists) FileProcessor.ProcessFile(rootFile, thisEnv);

            foreach (var file in directory.EnumerateFiles()) if (file.Name != ".autoproj" && file.Extension == ".autoproj") {
                string outputfullname = file.FullName.Substring(0, file.FullName.Length - ".autoproj".Length);
                var fileEnv = new AutoprojEnv(thisEnv, file);

                var cont = FileProcessor.ProcessFile(file, fileEnv);
                var sw = new StreamWriter(new FileStream(outputfullname, FileMode.Create, FileAccess.Write), new UTF8Encoding(false));
                sw.Write(cont);
                sw.Close();

                fileEnv.EndUse();
            }

            foreach (var dir in directory.EnumerateDirectories()) {
                enumerateDir(dir, thisEnv);
            }

            thisEnv.EndUse();
        }

        public static void Main(string[] args) {
            var rootDir = new DirectoryInfo(Environment.CurrentDirectory);
            var rootEnv = new AutoprojEnv();

            initEnv(rootEnv);
            enumerateDir(rootDir, rootEnv);
        }
    }
}
