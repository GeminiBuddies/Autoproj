using CommandLine;

namespace GeminiLab.Autoproj {
    public sealed class CommandlineOptions {
        [Option('p', "path", HelpText = "Working path.")]
        public string Path { get; set; } = null;

        [Option('e', "extension", HelpText = "Template extension.", Default = Def.DefaultTemplateExtension)]
        public string TemplateExtension { get; set; }

        [Option('j', "json-extension", HelpText = "Template json storage extension.", Default = Def.DefaultTemplateStoreExtension)]
        public string TemplateJsonExtension { get; set; }
    }
}
