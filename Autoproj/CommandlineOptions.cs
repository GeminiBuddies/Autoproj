using System.Collections.Generic;
using CommandLine;

namespace GeminiLab.Autoproj {
    public sealed class CommandlineOptions {
        [Option('p', "path", HelpText = "Specify the working path.", MetaValue = "path")]
        public string Path { get; set; } = null;

        [Option('e', "extension", HelpText = "Specify the template extension.", MetaValue = ".ext", Default = Def.DefaultTemplateExtension)]
        public string TemplateExtension { get; set; }

        [Option('j', "json-extension", HelpText = "Specify the template json storage extension.", Default = Def.DefaultTemplateStoreExtension)]
        public string TemplateJsonExtension { get; set; }

        // todo: detailed log level control
        [Option('q', "quiet", HelpText = "Suppress all messages.", Default = false)]
        public bool Quiet { get; set; }

        [Option('v', "verbose", HelpText = "Display detailed message.", Default = false)]
        public bool Verbose { get; set; }

        [Value(0, HelpText = "Predefined const. key=value.", Hidden = true)]
        public IEnumerable<string> Predefined { get; set; }
    }
}
