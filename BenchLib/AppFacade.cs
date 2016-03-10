using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Mastersign.Bench
{
    public class AppFacade
    {
        private readonly IGroupedPropertyCollection AppIndex;

        private readonly string AppName;

        public AppFacade(IGroupedPropertyCollection source, string appName)
        {
            AppIndex = source;
            AppName = appName;
        }

        private object Value(string property)
        {
            return AppIndex.GetGroupValue(AppName, property);
        }

        private string StringValue(string property)
        {
            return Value(property) as string;
        }

        private bool BoolValue(string property)
        {
            return (bool)(Value(property) ?? false);
        }

        private string[] ListValue(string property)
        {
            var value = Value(property);
            if (value is string) return new[] { (string)value };
            return (value as string[]) ?? new string[0];
        }

        public string ID { get { return AppName; } }

        public string Category { get { return AppIndex.GetGroupCategory(AppName); } }

        public string Typ { get { return StringValue(PropertyKeys.AppTyp); } }

        public string Version { get { return StringValue(PropertyKeys.AppVersion); } }

        public string[] Dependencies { get { return ListValue(PropertyKeys.AppDependencies); } }

        public bool IsActive { get { return BoolValue(PropertyKeys.AppActivated); } }

        public string Url { get { return StringValue(PropertyKeys.AppUrl); } }

        public IDictionary<string, string> DownloadHeaders
        {
            get { return Value(PropertyKeys.AppDownloadHeaders) as Dictionary<string, string>; }
        }

        public Cookie[] DownloadCookies
        {
            get { return Value(PropertyKeys.AppDownloadCookies) as Cookie[]; }
        }

        public string ResourceName { get { return StringValue(PropertyKeys.AppResourceName); } }

        public string ResourceArchiveName { get { return StringValue(PropertyKeys.AppArchiveName); } }

        public string ResourceArchivePath { get { return StringValue(PropertyKeys.AppArchivePath); } }

        public bool Force { get { return BoolValue(PropertyKeys.AppForce); } }

        public string PackageName { get { return StringValue(PropertyKeys.AppPackageName); } }

        public string Dir { get { return StringValue(PropertyKeys.AppDir); } }

        public string[] Path { get { return ListValue(PropertyKeys.AppPath); } }

        public string Exe { get { return StringValue(PropertyKeys.AppExe); } }

        public bool Register { get { return BoolValue(PropertyKeys.AppRegister); } }

        public IDictionary<string, string> Environment
        {
            get { return Value(PropertyKeys.AppEnvironment) as Dictionary<string, string>; }
        }

        public string[] AdornedExecutables { get { return ListValue(PropertyKeys.AppAdornedExecutables); } }

        public string[] RegistryKeys { get { return ListValue(PropertyKeys.AppRegistryKeys); } }

        public string Launcher { get { return StringValue(PropertyKeys.AppLauncher); } }

        public string LauncherExecutable { get { return StringValue(PropertyKeys.AppLauncherExecutable); } }

        public string[] LauncherArguments { get { return ListValue(PropertyKeys.AppLauncherArguments); } }

        public string LauncherIcon { get { return StringValue(PropertyKeys.AppLauncherIcon); } }

        public string SetupTestFile { get { return StringValue(PropertyKeys.AppSetupTestFile); } }

        public bool IsInstalled
        {
            get
            {
                switch(Typ)
                {
                    case AppTyps.NodePackage:
                        var npmDir = AppIndex.GetGroupValue(AppKeys.Npm, PropertyKeys.AppDir) as string;
                        var npmPackageDir = System.IO.Path.Combine(
                            System.IO.Path.Combine(npmDir, "node_modules"),
                            PackageName);
                        return System.IO.Directory.Exists(npmPackageDir);
                    case AppTyps.Python2Package:
                        var python2Dir = AppIndex.GetGroupValue(AppKeys.Python2, PropertyKeys.AppDir) as string;
                        var pip2PackageDir = System.IO.Path.Combine(
                            System.IO.Path.Combine(python2Dir, "lib"),
                            System.IO.Path.Combine("site-packages", PackageName));
                        return System.IO.Directory.Exists(pip2PackageDir);
                    case AppTyps.Python3Package:
                        var python3Dir = AppIndex.GetGroupValue(AppKeys.Python3, PropertyKeys.AppDir) as string;
                        var pip3PackageDir = System.IO.Path.Combine(
                            System.IO.Path.Combine(python3Dir, "lib"),
                            System.IO.Path.Combine("site-packages", PackageName));
                        return System.IO.Directory.Exists(pip3PackageDir);
                    default:
                        return System.IO.File.Exists(SetupTestFile);
                }
            }
        }

        public void Activate()
        {
            AppIndex.SetValue(AppName, PropertyKeys.AppActivated, true);
            foreach (var depName in Dependencies)
            {
                var depApp = new AppFacade(AppIndex, depName);
                if (!depApp.IsActive)
                {
                    depApp.Activate();
                }
            }
        }

        public void Deactivate()
        {
            AppIndex.SetValue(AppName, PropertyKeys.AppActivated, false);
        }
    }
}
