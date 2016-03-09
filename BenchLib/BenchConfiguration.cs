using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Mastersign.Bench
{
    public class BenchConfiguration : ResolvingPropertyCollection
    {
        private const string ConfigFile = @"res\config.md";

        private const string CustomConfigFileKey = "CustomConfigFile";
        private const string AppIndexFileKey = "AppIndexFile";
        private const string CustomAppIndexFileKey = "CustomAppIndexFile";

        public BenchConfiguration(string benchRootDir)
        {
            this.AddResolver(new PathResolver(benchRootDir));
            this.AddResolver(new GroupedVariableResolver(this));
            this.AddResolver(new VariableResolver(this));

            var parser = new MarkdownPropertyParser
            {
                Target = this,
                GroupBeginCue = new Regex("^[\\*\\+-]\\s+ID:\\s*`(?<group>\\S+?)`$"),
                GroupEndCue = new Regex("^\\s*$"),
            };
            
            var configFile = Path.Combine(benchRootDir, ConfigFile);
            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException("The default configuration for Bench was not found.", configFile);
            }
            using (var configStream = File.OpenRead(configFile)) 
            {
                parser.Parse(configStream);
            }

            var customConfigFile = GetStringValue(CustomConfigFileKey);
            if (File.Exists(customConfigFile))
            {
                using (var customConfigStream = File.OpenRead(customConfigFile))
                {
                    parser.Parse(customConfigStream);
                }
            }

            var appIndexFile = GetStringValue(AppIndexFileKey);
            if (!File.Exists(appIndexFile))
            {
                throw new FileNotFoundException("The default app index for Bench was not found.", appIndexFile);
            }
            using (var appIndexStream = File.OpenRead(appIndexFile))
            {
                parser.Parse(appIndexStream);
            }

            var customAppIndexFile = GetStringValue(CustomAppIndexFileKey);
            if (File.Exists(customAppIndexFile))
            {
                using (var customAppIndexStream = File.OpenRead(customAppIndexFile))
                {
                    parser.Parse(customAppIndexStream);
                }
            }

            this.GroupedDefaultValueSource = new AppIndexDefaultValueSource(this);
        }
    }
}
