using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public interface IPropertyCollection : IPropertySource
    {
        IEnumerable<string> PropertyNames();

        object GetValue(string name, object def);

        string GetStringValue(string name, string def);

        string[] GetStringListValue(string name, string[] def);

        bool GetBooleanValue(string name, bool def);

        bool ContainsValue(string name);
    }
}
