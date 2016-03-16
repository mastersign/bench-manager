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
            foreach (var appName in appNames)
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
                foreach (var appName in AppIndex.Groups())
                {
                    var isActive = AppIndex.GetBooleanGroupValue(appName, PropertyKeys.AppActivated);
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
            foreach (var appName in AppIndex.Groups())
            {
                yield return new AppFacade(AppIndex, appName);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string[] EnvironmentPath
        {
            get
            {
                var result = new List<string>();
                foreach (var app in ActiveApps)
                {
                    foreach (var p in app.Path)
                    {
                        if (!result.Contains(p))
                        {
                            result.Add(p);
                        }
                    }
                }
                return result.ToArray();
            }
        }

        public IDictionary<string, string> Environment
        {
            get
            {
                var result = new Dictionary<string, string>();
                var apps = ActiveApps;
                foreach (var app in apps)
                {
                    var e = app.Environment;
                    if (e != null)
                    {
                        foreach (var k in e.Keys)
                        {
                            result[k] = e[k];
                        }
                    }
                }
                return result;
            }
        }
    }
}
