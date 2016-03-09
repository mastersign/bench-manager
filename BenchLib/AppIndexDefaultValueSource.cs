using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public class AppIndexDefaultValueSource : IGroupedPropertySource
    {
        public IGroupedPropertySource AppIndex { get; set; }

        public AppIndexDefaultValueSource()
        {
        }

        public AppIndexDefaultValueSource(IGroupedPropertySource appIndex)
        {
            AppIndex = appIndex;
        }

        public object GetGroupValue(string group, string name)
        {
            throw new NotImplementedException();
        }

        public bool CanGetGroupValue(string group, string name)
        {
            return false;
        }
    }
}
