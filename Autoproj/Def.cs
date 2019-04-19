using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GeminiLab.Autoproj {
    internal static class Def {
        public static readonly string ProgramName = "Autoproj";
        public static readonly string Author = "Benjamin P.M. Lovegood (a.k.a. aarkegz) @ Gemini Laboratory";
        public static readonly string RepositoryAddress = "https://github.com/GeminiLab/Autoproj";
        public static readonly string OpenSourceInfo = $"Open source under BSD 3-Clause License. See {RepositoryAddress}.";

        public static readonly string VersionString = ((Func<Assembly, string>)(ass => {
            var informationalVersion = ass.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (informationalVersion != null) return informationalVersion.InformationalVersion;

            var fileVersion = ass.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (fileVersion != null) return fileVersion.Version;

            return ass.GetName().Version.ToString();
        }))(typeof(Def).Assembly);


        public const string DefaultTemplateExtension = ".autoproj";
        public const string DefaultTemplateJsonExtension = ".autoproj.json";
    }
}
