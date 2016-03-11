using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public class AppIndexFacade : IEnumerable<AppFacade>
    {
        private readonly IConfiguration AppIndex;

        public AppIndexFacade(IConfiguration appIndex)
        {
            AppIndex = appIndex;
        }

        public AppFacade this[string appName]
        {
            get { return Exists(appName) ? new AppFacade(AppIndex, appName) : null; }
        }

        public bool Exists(string appName)
        {
            return AppIndex.ContainsGroup(appName);
        }

        public AppFacade[] ByCategory(string category)
        {
            var appNames = AppIndex.GroupsByCategory(category);
            var result = new List<AppFacade>();
            foreach(var appName in appNames)
            {
                result.Add(new AppFacade(AppIndex, appName));
            }
            return result.ToArray();
        }

        public AppFacade[] ActiveApps
        {
            get
            {
                var result = new List<AppFacade>();
                foreach(var appName in AppIndex.Groups())
                {
                    var isActive = (bool)(AppIndex.GetGroupValue(appName, PropertyKeys.AppActivated) ?? false);
                    if (isActive)
                    {
                        result.Add(new AppFacade(AppIndex, appName));
                    }
                }
                return result.ToArray();
            }
        }

        public IEnumerator<AppFacade> GetEnumerator()
        {
            foreach(var appName in AppIndex.Groups())
            {
                yield return new AppFacade(AppIndex, appName);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
