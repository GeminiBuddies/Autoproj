using System;
using System.Globalization;
using System.IO;

namespace GeminiLab.Autoproj {
    public partial class AutoprojEnv {
        public static AutoprojEnv GetRootEnv() {
            var rootEnv = new AutoprojEnv();

            rootEnv.TryAddConst("lcmd", "<~~");
            rootEnv.TryAddConst("rcmd", "~~>");
            rootEnv.TryAddConst("lrpl", "<~");
            rootEnv.TryAddConst("rrpl", "~>");

            rootEnv.TryAddConst("autoprojver", Def.VersionString);
            rootEnv.TryAddFunction("now", param => param.Length == 1 ? DateTime.Now.ToString(param[0]) : DateTime.Now.ToString(CultureInfo.InvariantCulture));

            return rootEnv;
        }


        public static AutoprojEnv GetDirectoryEnv(AutoprojEnv parent, DirectoryInfo directory, CommandlineOptions options) {
            var dbPath = Path.Combine(directory.FullName, options.TemplateJsonExtension);
            var rv = new AutoprojEnvStored(parent, dbPath);

            rv.TryAddConst("cwdname", directory.Name);
            rv.TryAddConst("cwdpath", directory.FullName);

            return rv;
        }

        public static AutoprojEnv GetFileEnv(AutoprojEnv parent, FileInfo file, CommandlineOptions options) {
            var outputFullname = file.FullName.Substring(0, file.FullName.Length - options.TemplateExtension.Length);
            var dbPath = outputFullname + options.TemplateJsonExtension;
            var rv = new AutoprojEnvStored(parent, dbPath);
            
            rv.TryAddConst("ifname", file.Name);
            rv.TryAddConst("ifpath", file.FullName);
            rv.TryAddConst("ofname", file.Name.Substring(0, file.Name.Length - options.TemplateExtension.Length));
            rv.TryAddConst("ofpath", outputFullname);

            return rv;
        }
    }
}
