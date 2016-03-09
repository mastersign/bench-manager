using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public class ResolvingPropertyCollection : PropertyCollection
    {
        private readonly List<IValueResolver> resolvers = new List<IValueResolver>();

        public void AddResolver(params IValueResolver[] resolvers)
        {
            this.resolvers.AddRange(resolvers);
        }

        protected override string ResolveValue(string group, string name, string value)
        {
            foreach (var r in resolvers)
            {
                value = r.ResolveValue(group, name, value);
            }
            return value;
        }
    }
}
