using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public class ResolvingPropertyCollection : GroupedPropertyCollection
    {
        private readonly List<IGroupedValueResolver> resolvers = new List<IGroupedValueResolver>();

        public void AddResolver(params IGroupedValueResolver[] resolvers)
        {
            this.resolvers.AddRange(resolvers);
        }

        protected override object ResolveGroupValue(string group, string name, object value)
        {
            if (value != null && !(value is string) && value.GetType().IsArray)
            {
                var source = (Array)value;
                var mapped = Array.CreateInstance(source.GetType().GetElementType(), source.Length);
                for (int i = 0; i < source.Length; i++)
                {
                    mapped.SetValue(ResolveGroupValue(group, name, source.GetValue(i)), i);
                }
                return mapped;
            }
            foreach (var r in resolvers)
            {
                value = r.ResolveGroupValue(group, name, value);
            }
            return value;
        }
    }
}
