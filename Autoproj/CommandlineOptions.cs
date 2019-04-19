using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;
using CommandLine.Text;

namespace GeminiLab.Autoproj {
    internal class CommandlineOptions {
        [Option('p', "path", HelpText = "Working path.")]
        public string Path { get; set; } = null;

        [Option('e', "extension", HelpText = "Template extension.", Default = Def.DefaultTemplateExtension)]
        public string TemplateExtension { get; set; }

        [Option('j', "json-extension", HelpText = "Template json storage extension.", Default = Def.DefaultTemplateJsonExtension)]
        public string TemplateJsonExtension { get; set; }
    }
}
