using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mastersign.Bench
{
    public class GroupedVariableResolver : IValueResolver
    {
        private static readonly Regex DefaultGroupVariablePattern = new Regex("\\$(?<group>.+?):(?<name>.+?)\\$");

        /// <remarks>
        /// The regular expression needs two named groups with names <c>group</c> and <c>name</c>.
        /// </remarks>
        public Regex GroupVariablePattern { get; set; }

        public IGroupedPropertySource ValueSource { get; set; }

        public GroupedVariableResolver()
        {
            GroupVariablePattern = DefaultGroupVariablePattern;
        }

        public GroupedVariableResolver(IGroupedPropertySource valueSource)
        {
            ValueSource = valueSource;
        }

        public string ResolveValue(string group, string name, string value)
        {
            if (value == null) return null;
            if (ValueSource != null && GroupVariablePattern != null)
            {
                value = GroupVariablePattern.Replace(value, m =>
                {
                    var g = m.Groups["group"].Value;
                    var n = m.Groups["name"].Value;
                    return ValueSource.GetStringValue(g, n, string.Format("#{0}:{1}#", g, n));
                });
            }
            return value;
        }
    }
}
