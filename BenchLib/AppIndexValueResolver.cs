using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Mastersign.Bench
{
    public class AppIndexValueResolver : IGroupedValueResolver
    {
        private static readonly char[] KeyValueSeparator = new char[] { ':' };
        private static readonly char[] DetailSeparator = new char[] { '=' };

        public IGroupedPropertySource AppIndex { get; set; }

        public AppIndexValueResolver()
        {
        }

        public AppIndexValueResolver(IGroupedPropertySource appIndex)
            : this()
        {
            AppIndex = appIndex;
        }

        public object ResolveGroupValue(string group, string name, object value)
        {
            switch (name)
            {
                case PropertyKeys.AppDownloadHeaders:
                    return ParseKeyValuePairs(value);
                case PropertyKeys.AppDownloadCookies:
                    return ParseCookies(value);
                case PropertyKeys.AppEnvironment:
                    return ParseKeyValuePairs(value);
                default:
                    return value;
            }
        }

        private Cookie[] ParseCookies(object value)
        {
            if (value is string)
            {
                var cookie = ParseCookie((string)value);
                return cookie != null ? new[] { cookie } : new Cookie[0];
            }
            if (value is string[])
            {
                var list = (string[])value;
                var result = new List<Cookie>();
                for (int i = 0; i < list.Length; i++)
                {
                    var cookie = ParseCookie(list[i]);
                    if (cookie != null)
                    {
                        result.Add(cookie);
                    }
                }
                return result.ToArray();
            }
            return new Cookie[0];
        }

        private Cookie ParseCookie(string cookie)
        {
            var kvp = ParseKeyValuePair(cookie);
            if (!string.IsNullOrEmpty(kvp.Key) && !string.IsNullOrEmpty(kvp.Value))
            {
                var c = new Cookie();
                c.Domain = kvp.Key;
                var detail = kvp.Value.Split(DetailSeparator, 2);
                c.Name = detail[0];
                if (detail.Length > 1)
                {
                    c.Value = detail[1];
                }
                return c;
            }
            return null;
        }

        private Dictionary<string, string> ParseKeyValuePairs(object value)
        {
            var d = new Dictionary<string, string>();
            if (value is string)
            {
                var kvp = ParseKeyValuePair((string)value);
                if (!string.IsNullOrEmpty(kvp.Key))
                {
                    d.Add(kvp.Key, kvp.Value);
                }
            }
            if (value is string[])
            {
                foreach (var v in (string[])value)
                {
                    var kvp = ParseKeyValuePair(v);
                    if (!string.IsNullOrEmpty(kvp.Key))
                    {
                        d[kvp.Key] = kvp.Value;
                    }

                }
            }
            return d;
        }

        private KeyValuePair<string, string> ParseKeyValuePair(string header)
        {
            if (header != null && header.Contains(":"))
            {
                var p = header.Split(KeyValueSeparator, 2);
                return new KeyValuePair<string, string>(p[0].Trim(), p[1].Trim());
            }
            else
            {
                return new KeyValuePair<string, string>();
            }
        }
    }
}
