using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public interface IValueResolver
    {
        string ResolveValue(string group, string name, string value);
    }
}
