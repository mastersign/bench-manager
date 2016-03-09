using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public interface IGroupedPropertyCollection : IGroupedPropertySource
    {
        IEnumerable<string> Groups();

        string GetGroupCategory(string group);

        IEnumerable<string> GroupsByCategory(string category);

        IEnumerable<string> PropertyNames(string group);

        object GetGroupValue(string group, string name, object def);

        string GetStringGroupValue(string group, string name, string def);

        string[] GetStringListGroupValue(string group, string name, string[] def);

        bool GetBooleanGroupValue(string group, string name, bool def);
        
        bool ContainsGroup(string group);

        bool ContainsGroupValue(string group, string name);
    }
}
