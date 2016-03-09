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
        private const string DownloadHeadersKey = "DownloadHeaders";
        private const string DownloadCookiesKey = "DownloadCookies";
        private const string AppDirKey = "Dir";
        private const string AppTypKey = "Typ";
        private const string NpmPackageTyp = "node-package";
        private const string Python2PackageTyp = "python2-package";
        private const string Python3PackageTyp = "python3-package";
        private const string MetaAppTyp = "meta";
        private const string NpmAppId = "Node";
        private const string Python2AppId = "Python2";
        private const string Python3AppId = "Python3";

        public IGroupedPropertySource AppIndex { get; set; }

        public AppIndexValueResolver()
        {
        }

        public AppIndexValueResolver(IGroupedPropertySource appIndex)
        {
            AppIndex = appIndex;
        }

        public object ResolveGroupValue(string group, string name, object value)
        {
            switch (name)
            {
                case DownloadHeadersKey:
                    return ParseKeyValuePairs(value);
                case DownloadCookiesKey:
                    return ParseCookies(value);
                case AppDirKey:
                    return GetAppDir(group, value);
                default:
                    return value;
            }
        }

        private string GetAppDir(string app, object value)
        {
            if (AppIndex == null) throw new InvalidOperationException("The resolver has no reference to the application index.");
            var typ = AppIndex.GetGroupValue(app, AppTypKey) as string;
            switch (typ)
            {
                case NpmPackageTyp:
                    return AppIndex.GetGroupValue(NpmAppId, AppDirKey) as string;
                case Python2PackageTyp:
                    return AppIndex.GetGroupValue(Python2AppId, AppDirKey) as string;
                case Python3PackageTyp:
                    return AppIndex.GetGroupValue(Python3AppId, AppDirKey) as string;
                default:
                    return value as string;
            }
        }

        private Cookie[] ParseCookies(object value)
        {
            if (value is string)
            {
                return new[] { ParseCookie((string)value) };
            }
            if (value is string[])
            {
                var list = (string[])value;
                var result = new Cookie[list.Length];
                for (int i = 0; i < list.Length; i++)
                {
                    result[i] = ParseCookie(list[i]);
                }
                return result;
            }
            return new Cookie[0];
        }

        private Cookie ParseCookie(string cookie)
        {
            var kvp = ParseKeyValuePair(cookie);
            var c = new Cookie();
            c.Domain = kvp.Key;
            var detail = kvp.Value.Split(DetailSeparator, 2);
            c.Name = detail[0];
            c.Value = detail[1];
            return c;
        }

        private Dictionary<string, string> ParseKeyValuePairs(object value)
        {
            var d = new Dictionary<string, string>();
            if (value is string)
            {
                var kvp = ParseKeyValuePair((string)value);
                d.Add(kvp.Key, kvp.Value);
            }
            if (value is string[])
            {
                foreach (var v in (string[])value)
                {
                    var kvp = ParseKeyValuePair(v);
                    d[kvp.Key] = kvp.Value;
                }
            }
            return d;
        }

        private KeyValuePair<string, string> ParseKeyValuePair(string header)
        {
            var p = header.Split(KeyValueSeparator, 2);
            return new KeyValuePair<string, string>(p[0].Trim(), p[1].Trim());
        }
    }
}
