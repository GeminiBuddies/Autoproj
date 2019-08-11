using System;
using System.Reflection;

namespace GeminiLab.Autoproj {
    internal static class Metadata {
        public static readonly string ProgramName = "Autoproj";
        public static readonly string Author = "Benjamin P.M. Lovegood (a.k.a. aarkegz) @ Gemini Laboratory";
        public static readonly string RepositoryAddress = "https://github.com/GeminiLab/Autoproj";
        public static readonly string OpenSourceInfo = $"Open source under BSD 3-Clause License.";
        public static readonly string ProjectSiteInfo = $"See {RepositoryAddress} for more information.";

        public static readonly string VersionString = ((Func<Assembly, string>)(ass => {
            var informationalVersion = ass.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (informationalVersion != null) return informationalVersion.InformationalVersion;

            var fileVersion = ass.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (fileVersion != null) return fileVersion.Version;

            return ass.GetName().Version.ToString();
        }))(typeof(StaticConfig).Assembly);

        public static readonly string VersionCodeName = "amai";

        public static readonly string CopyrightInfo = $"Copyright (C) 2018 - {DateTime.Now.Year} {Author}";
    }
}