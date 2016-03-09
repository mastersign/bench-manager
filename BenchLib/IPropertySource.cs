using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public interface IPropertySource
    {
        object GetValue(string name);

        string GetStringValue(string name);

        string[] GetStringListValue(string name);

        bool GetBooleanValue(string name);

        bool CanGetValue(string name);
    }
}
