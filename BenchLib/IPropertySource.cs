using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    interface IPropertySource
    {
        IEnumerable<string> Groups();

        string GetGroupCategory(string group);

        IEnumerable<string> GroupsByCategory(string category);

        IEnumerable<string> PropertyNames(string group = null);

        object GetValue(string name, object def);

        object GetValue(string group, string name, object def);

        bool ContainsGroup(string group);

        bool ContainsValue(string name);

        bool ContainsValue(string group, string name);
    }
}
