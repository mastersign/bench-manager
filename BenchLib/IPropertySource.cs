using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public interface IPropertySource
    {
        IEnumerable<string> PropertyNames();

        object GetValue(string name, object def);

        string GetStringValue(string name, string def = "");

        bool GetBooleanValue(string name, bool def = false);

        string[] GetStringListValue(string name, string[] def = null);

        bool ContainsValue(string name);
    }
}
