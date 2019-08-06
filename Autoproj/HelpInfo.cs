namespace GeminiLab.Autoproj {
    internal static class HelpInfo {
        public static readonly string HelpTextHeader =
            $"{Metadata.ProgramName} ver. {Metadata.VersionCodeName}({Metadata.VersionString})\n" +
            $"{Metadata.CopyrightInfo}\n" +
            $"{Metadata.OpenSourceInfo}\n";

        public static readonly string HelpTextBody = $@"
Usage: autoproj [OPTIONS] [-- key1=value1 [key2=value2 ...]]

OPTIONS:
    -p path     --path path
        Specify the working directory. The default is current working directory.
                                                                                                    .
    -s suffix   --suffix suffix
        Specify the suffix of template files. The default is ""{StaticConfig.DefaultTemplateSuffix}"".
                                                                                                    .
    -t suffix   --storage suffix
        Specify the suffix of template local storage files. The default is ""{StaticConfig.DefaultStorageSuffix}"".
                                                                                                    .
    -l level    --log-level level
        Disable all logs with log level below the level specified.
        Possible log levels: all(show all logs), trace, debug, warn, error, fatal, off(be quiet).
        The default is warn.
                                                                                                    .
    -q          --quiet
        Be quiet. Equivalent to -l off.
                                                                                                    .
    -c          --chatty
        Be chatty. Equivalent to -l all.
                                                                                                    .
    -f format   --timestamp format
        Set timestamp format of logs.
                                                                                                    .
    -h          --help
        Print this help text and exit.
                                                                                                    .
    NOTE that
    - If there are multiple -p/-s/-t/-l/-f options, ONLY the last ones of each category work.
    - Option -q overrides option -c, while option -l overrides both.
    - Option -h has highest priority.
                                                                                                    .
key1=value1 [key2=value2 ...]:
    Predefined constants.
                                                                                                    .";

        public static readonly string HelpText = HelpTextHeader + HelpTextBody;
    }
}