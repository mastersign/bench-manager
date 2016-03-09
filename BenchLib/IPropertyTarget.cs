using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public interface IPropertyTarget
    {
        void SetGroupCategory(string group, string category);

        void SetValue(string name, object value);

        void SetValue(string group, string name, object value);
    }
}
