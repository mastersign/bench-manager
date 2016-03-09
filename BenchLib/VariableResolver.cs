using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mastersign.Bench
{
    public class VariableResolver : IValueResolver
    {
        private static readonly Regex DefaultVariablePattern = new Regex("\\$(?<name>.+?)\\$");

        /// <remarks>
        /// The regular expression needs a named group with name <c>name</c>'.
        /// </remarks>
        public Regex VariablePattern { get; set; }

        public IPropertySource ValueSource { get; set; }

        public VariableResolver()
        {
            VariablePattern = DefaultVariablePattern;
        }

        public VariableResolver(IPropertySource valueSource)
        {
            ValueSource = valueSource;
        }

        public string ResolveValue(string group, string name, string value)
        {
            if (value == null) return null;
            if (ValueSource != null && VariablePattern != null)
            {
                value = VariablePattern.Replace(value, m =>
                {
                    var n = m.Groups["name"].Value;
                    return ValueSource.GetStringValue(n, string.Format("#{0}#", n));
                });
            }
            return value;
        }
    }
}
