using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Mastersign.Bench
{
    public class PathResolver : IValueResolver
    {
        private static readonly Regex DefaultNamePattern = new Regex("(?:File|Dir)$");

        public string BasePath { get; set; }

        public Regex NamePattern { get; set; }

        public PathResolver()
            : this(Environment.CurrentDirectory)
        {
        }

        public PathResolver(string basePath)
        {
            NamePattern = DefaultNamePattern;
            BasePath = basePath;
        }

        public string ResolveValue(string group, string name, string value)
        {
            if (value == null) return null;
            if (NamePattern == null || NamePattern.IsMatch(name))
            {
                if (!Path.IsPathRooted(value))
                {
                    value = Path.Combine(BasePath, value);
                }
            }
            return value;
        }
    }
}
