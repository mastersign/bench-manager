using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Mastersign.Bench
{
    public class PathResolver : IGroupedValueResolver
    {
        private static readonly Regex DefaultNamePattern = new Regex("(?:File|Dir)$");

        public string BasePath { get; set; }

        public Regex NamePattern { get; set; }

        public GroupBasePathSource GroupBasePathSource { get; set; }

        public PathResolver()
            : this(Environment.CurrentDirectory, null)
        {
        }

        public PathResolver(string basePath, GroupBasePathSource groupBasePathSource)
        {
            NamePattern = DefaultNamePattern;
            BasePath = basePath;
            GroupBasePathSource = groupBasePathSource;
        }

        public object ResolveGroupValue(string group, string name, object value)
        {
            if (value == null) return null;
            if (value is string && (NamePattern == null || NamePattern.IsMatch(name)))
            {
                var path = (string)value;
                if (!Path.IsPathRooted(path))
                {
                    if (!string.IsNullOrEmpty(group) && GroupBasePathSource != null)
                    {
                        value = Path.Combine(GroupBasePathSource(group), path);
                    }
                    else
                    {
                        value = Path.Combine(BasePath, path);
                    }
                }
            }
            return value;
        }
    }

    public delegate string GroupBasePathSource(string group);
}
