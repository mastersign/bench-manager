using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            this.AddResolver(new GroupedVariableResolver(this));
            this.AddResolver(new VariableResolver(this));
            this.AddResolver(new PathResolver(benchRootDir));

            var parser = new MarkdownPropertyParser
            {
                Target = this,
                GroupBeginCue = new Regex("^[\\*\\+-]\\s+ID:\\s*`(?<group>\\S+?)`$"),
                GroupEndCue = new Regex("^\\s*$"),
            };
            
            var configFile = Path.Combine(benchRootDir, ConfigFile);
            Debug.WriteLine("Looking for default configuration: " + configFile);
            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException("The default configuration for Bench was not found.", configFile);
            }
            using (var configStream = File.OpenRead(configFile)) 
            {
                Debug.WriteLine("Reading default configuration ...");
                parser.Parse(configStream);
            }

            var customConfigFile = GetStringValue(CustomConfigFileKey);
            Debug.WriteLine("Looking for custom config file: " + customConfigFile);
            if (File.Exists(customConfigFile))
            {
                using (var customConfigStream = File.OpenRead(customConfigFile))
                {
                    Debug.WriteLine("Reading custom configuration ...");
                    parser.Parse(customConfigStream);
                }
            }

            var appIndexFile = GetStringValue(AppIndexFileKey);
            Debug.WriteLine("Looking for application index: " + appIndexFile);
            if (!File.Exists(appIndexFile))
            {
                throw new FileNotFoundException("The default app index for Bench was not found.", appIndexFile);
            }
            using (var appIndexStream = File.OpenRead(appIndexFile))
            {
                Debug.WriteLine("Reading default application index ...");
                parser.Parse(appIndexStream);
            }

            var customAppIndexFile = GetStringValue(CustomAppIndexFileKey);
            Debug.WriteLine("Looking for custom application index: " + customConfigFile);
            if (File.Exists(customAppIndexFile))
            {
                using (var customAppIndexStream = File.OpenRead(customAppIndexFile))
                {
                    Debug.WriteLine("Reading custom application index ...");
                    parser.Parse(customAppIndexStream);
                }
            }

            this.GroupedDefaultValueSource = new AppIndexDefaultValueSource(this);
        }
    }
}
