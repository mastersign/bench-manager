using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Mastersign.Bench
{
    public class PathResolver : IGroupedValueResolver
    {
        public PropertyCriteria Selector { get; set; }

        public BasePathSource BasePathSource { get; set; }

        public PathResolver()
        {
        }

        public PathResolver(PropertyCriteria selector, BasePathSource basePathSource)
            : this()
        {
            Selector = selector;
            BasePathSource = basePathSource;
        }

        public object ResolveGroupValue(string group, string name, object value)
        {
            if (value == null) return null;
            if (value is string && (Selector == null || Selector(group, name)))
            {
                var path = (string)value;
                if (!Path.IsPathRooted(path) && BasePathSource != null)
                {
                    value = Path.Combine(BasePathSource(group, name), path);
                }
            }
            return value;
        }
    }

    public delegate bool PropertyCriteria(string group, string name);
    public delegate string BasePathSource(string group, string name);
}
