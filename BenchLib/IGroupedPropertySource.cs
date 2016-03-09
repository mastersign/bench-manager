using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public interface IGroupedPropertySource
    {
        IEnumerable<string> Groups();

        string GetGroupCategory(string group);

        IEnumerable<string> GroupsByCategory(string category);

        IEnumerable<string> PropertyNames(string group);

        object GetValue(string group, string name, object def);

        string GetStringValue(string group, string name, string def = "");

        bool GetBooleanValue(string group, string name, bool def = false);

        string[] GetStringListValue(string group, string name, string[] def = null);

        bool ContainsGroup(string group);

        bool ContainsValue(string group, string name);
    }
}
