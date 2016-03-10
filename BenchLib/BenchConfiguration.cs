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

        public string BenchRootDir { get; private set; }

        public BenchConfiguration(string benchRootDir)
        {
            BenchRootDir = benchRootDir;
            AddResolver(new GroupedVariableResolver(this));
            AddResolver(new VariableResolver(this));
            AddResolver(new PathResolver(IsPathProperty, GetBaseForPathProperty));

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

            var customConfigFile = GetStringValue(PropertyKeys.CustomConfigFile);
            Debug.WriteLine("Looking for custom config file: " + customConfigFile);
            if (File.Exists(customConfigFile))
            {
                using (var customConfigStream = File.OpenRead(customConfigFile))
                {
                    Debug.WriteLine("Reading custom configuration ...");
                    parser.Parse(customConfigStream);
                }
            }

            var appIndexFile = GetStringValue(PropertyKeys.AppIndexFile);
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

            var customAppIndexFile = GetStringValue(PropertyKeys.CustomAppIndexFile);
            Debug.WriteLine("Looking for custom application index: " + customConfigFile);
            if (File.Exists(customAppIndexFile))
            {
                using (var customAppIndexStream = File.OpenRead(customAppIndexFile))
                {
                    Debug.WriteLine("Reading custom application index ...");
                    parser.Parse(customAppIndexStream);
                }
            }

            AddResolver(new AppIndexValueResolver(this));
            GroupedDefaultValueSource = new AppIndexDefaultValueSource(this);
        }

        private bool IsPathProperty(string app, string property)
        {
            if (string.IsNullOrEmpty(app))
            {
                return property.EndsWith("File")
                    || property.EndsWith("Dir");
            }
            return property == PropertyKeys.AppDir
                || property == PropertyKeys.AppPath
                || property == PropertyKeys.AppExe
                || property == PropertyKeys.AppAdornedExecutables
                || property == PropertyKeys.AppLauncherIcon;
        }

        private string GetBaseForPathProperty(string app, string property)
        {
            if (string.IsNullOrEmpty(app))
            {
                return BenchRootDir;
            }
            else if (property == PropertyKeys.AppDir)
            {
                return GetStringValue(PropertyKeys.LibDir);
            }
            else
            {
                return GetStringGroupValue(app, PropertyKeys.AppDir);
            }
        }
    }
}
