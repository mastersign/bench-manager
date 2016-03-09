﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mastersign.Bench
{
    public class PropertyCollection : IPropertyTarget, IPropertySource
    {
        private static readonly Regex DefaultVariablePattern = new Regex("\\$(?<name>[\\S]+?)\\$");
        private static readonly Regex DefaultGroupVariablePattern = new Regex("\\$(?<group>[\\S]+?):(?<name>[\\S]+?)\\$");

        private List<string> groupNames = new List<string>(); // ordered list for group names
        private Dictionary<string, string> groupCategories = new Dictionary<string, string>();
        private Dictionary<string, List<string>> groupKeys = new Dictionary<string, List<string>>(); // ordered lists for property names
        private Dictionary<string, Dictionary<string, object>> groups = new Dictionary<string, Dictionary<string, object>>();

        /// <remarks>
        /// The regular expression needs a named group with name <c>name</c>'.
        /// </remarks>
        public Regex VariablePattern { get; set; }

        /// <remarks>
        /// The regular expression needs two named groups with names <c>group</c> and <c>name</c>.
        /// </remarks>
        public Regex GroupVariablePattern { get; set; }

        public PropertyCollection()
        {
            VariablePattern = DefaultVariablePattern;
            GroupVariablePattern = DefaultGroupVariablePattern;
        }

        public void Clear()
        {
            groupNames.Clear();
            groupCategories.Clear();
            groupKeys.Clear();
            groups.Clear();
        }

        private static bool IsValueSupported(object value)
        {
            return value == null
                || value is string
                || value is string[]
                || value is bool;
        }

        public void SetGroupCategory(string group, string category)
        {
            group = group ?? string.Empty;
            groupCategories[group] = category;
        }

        public string GetGroupCategory(string group)
        {
            string category;
            return groupCategories.TryGetValue(group, out category) ? category : null;
        }

        public bool ContainsGroup(string group)
        {
            group = group ?? string.Empty;
            return groups.ContainsKey(group);
        }

        public bool ContainsValue(string name) { return ContainsValue(null, name); }

        public bool ContainsValue(string group, string name)
        {
            group = group ?? string.Empty;
            Dictionary<string, object> g;
            return groups.TryGetValue(group, out g) && g.ContainsKey(name);
        }

        public void SetValue(string name, string value) { SetValue(null, name, value); }

        public void SetValue(string name, bool value) { SetValue(null, name, value); }

        public void SetValue(string name, string[] value) { SetValue(null, name, value); }

        public void SetValue(string name, object value) { SetValue(null, name, value); }

        public void SetValue(string group, string name, string value) { InternalSetValue(group, name, value); }

        public void SetValue(string group, string name, bool value) { InternalSetValue(group, name, value); }

        public void SetValue(string group, string name, string[] value) { InternalSetValue(group, name, value); }

        public void SetValue(string group, string name, object value)
        {
            if (!IsValueSupported(value)) throw new ArgumentException();
            InternalSetValue(group, name, value);
        }

        private void InternalSetValue(string groupName, string propertyName, object value)
        {
            groupName = groupName ?? string.Empty;
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentOutOfRangeException("propertyName", "The property name must not be null or empty.");
            }
            List<string> keys;
            Dictionary<string, object> group;
            if (groups.ContainsKey(groupName))
            {
                keys = groupKeys[groupName];
                group = groups[groupName];
            }
            else
            {
                groupNames.Add(groupName);
                keys = new List<string>();
                groupKeys.Add(groupName, keys);
                group = new Dictionary<string, object>();
                groups.Add(groupName, group);
            }
            if (group.ContainsKey(propertyName))
            {
                group[propertyName] = value;
            }
            else
            {
                keys.Add(propertyName);
                group.Add(propertyName, value);
            }
        }

        private object InternalGetValue(string groupName, string propertyName, out bool found, object def = null)
        {
            groupName = groupName ?? string.Empty;
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentOutOfRangeException("propertyName", "The property name must not be null or empty.");
            }
            Dictionary<string, object> group;
            if (groups.TryGetValue(groupName, out group))
            {
                object value;
                if (group.TryGetValue(propertyName, out value))
                {
                    if (value != null)
                    {
                        found = true;
                        return value;
                    }
                }
            }
            found = false;
            return def;
        }

        public object GetRawValue(string name, object def = null) { return GetRawValue(null, name, def); }

        public object GetRawValue(string group, string name, object def = null)
        {
            bool found;
            return InternalGetValue(group, name, out found, def);
        }

        public object GetValue(string name, object def = null) { return GetValue(null, name, def); }

        public object GetValue(string group, string name, object def = null)
        {
            var value = GetRawValue(group, name, def);
            if (value is string) value = ResolveValue((string)value);
            if (value is string[]) value = ResolveListValue((string[])value);
            return value;
        }

        public string GetStringValue(string name, string def = "") { return GetStringValue(null, name, def); }

        public string GetStringValue(string group, string name, string def = "")
        {
            bool found;
            var value = InternalGetValue(group, name, out found, def) as string;
            return found ? ResolveValue(value) : value;
        }

        public string[] GetStringListValue(string name, string[] def = null) { return GetStringListValue(null, name, def); }

        public string[] GetStringListValue(string group, string name, string[] def = null)
        {
            bool found;
            var value = InternalGetValue(group, name, out found, def);
            if (value is string) value = new string[] { (string)value };
            return found ? ResolveListValue(value as string[]) : def ?? new string[0];
        }

        public bool GetBooleanValue(string name, bool def = false) { return GetBooleanValue(null, name, def); }

        public bool GetBooleanValue(string group, string name, bool def = false)
        {
            bool found;
            var value = InternalGetValue(group, name, out found, def);
            return value is bool ? (bool)value : def;
        }

        private string ResolveValue(string value)
        {
            if (value == null) return null;
            if (GroupVariablePattern != null)
            {
                value = GroupVariablePattern.Replace(value, m =>
                {
                    var group = m.Groups["group"].Value;
                    var name = m.Groups["name"].Value;
                    return GetStringValue(group, name, string.Format("#{0}:{1}#", group, name));
                });
            }
            if (VariablePattern != null)
            {
                value = VariablePattern.Replace(value, m =>
                {
                    var name = m.Groups["name"].Value;
                    return GetStringValue(name, string.Format("#{0}#", name));
                });
            }
            return value;
        }

        private string[] ResolveListValue(string[] value)
        {
            if (value == null) return null;
            var result = new string[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                result[i] = ResolveValue(value[i]);
            }
            return result;
        }

        public IEnumerable<string> Groups()
        {
            foreach (var gk in groupNames)
            {
                if (gk.Length > 0) yield return gk;
            }
        }

        public IEnumerable<string> GroupsByCategory(string category)
        {
            foreach (var gk in groupNames)
            {
                if (gk != string.Empty && GetGroupCategory(gk) == category)
                {
                    yield return gk;
                }
            }
        }

        public IEnumerable<string> PropertyNames(string group = null)
        {
            group = group ?? string.Empty;
            List<string> keys;
            return groupKeys.TryGetValue(group, out keys) ? keys : (IEnumerable<string>)new string[0];
        }

        private string FormatValue(object val)
        {
            if (val is string)
            {
                val = ResolveValue((string)val);
                return string.Format("'{0}'", val);
            }
            if (val is string[])
            {
                var strings = ResolveListValue((string[])val);
                for (int i = 0; i < strings.Length; i++)
                {
                    strings[i] = string.Format("'{0}'", strings[i]);
                }
                return string.Join(", ", strings);
            }
            if (val is bool)
            {
                return (bool)val ? "True" : "False";
            }
            return "UNKNOWN";
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var gk in groupNames)
            {
                if (gk.Length > 0)
                {
                    var c = GetGroupCategory(gk);
                    if (c != null)
                    {
                        sb.AppendLine(string.Format("# Group '{0}' [{1}]", gk, c));
                    }
                    else
                    {
                        sb.AppendLine(string.Format("# Group '{0}'", gk));
                    }
                }
                var group = groups[gk];
                foreach (var k in groupKeys[gk])
                {
                    if (gk.Length > 0) sb.Append("  ");
                    sb.AppendLine(string.Format("- {0} = {1}", k, FormatValue(group[k])));
                }
            }
            return sb.ToString();
        }
    }
}
