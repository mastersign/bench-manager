﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Mastersign.Bench
{
    public class AppFacade
    {
        private readonly IConfiguration AppIndex;

        private readonly string AppName;

        public AppFacade(IConfiguration source, string appName)
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
            return AppIndex.GetStringGroupValue(AppName, property);
        }

        private bool BoolValue(string property)
        {
            return AppIndex.GetBooleanGroupValue(AppName, property);
        }

        private int IntValue(string property)
        {
            return AppIndex.GetInt32GroupValue(AppName, property);
        }

        private string[] ListValue(string property)
        {
            var value = Value(property);
            if (value is string) return new[] { (string)value };
            return (value as string[]) ?? new string[0];
        }

        private void UpdateValue(string property, object value)
        {
            AppIndex.SetGroupValue(AppName, property, value);
        }

        public string ID { get { return AppName; } }

        public string Category { get { return AppIndex.GetGroupCategory(AppName); } }

        public string Typ { get { return StringValue(PropertyKeys.AppTyp); } }

        public string Version { get { return StringValue(PropertyKeys.AppVersion); } }

        public string[] Dependencies
        {
            get { return ListValue(PropertyKeys.AppDependencies); }
            set { UpdateValue(PropertyKeys.AppDependencies, value); }
        }

        public void AddDependency(string app)
        {
            Dependencies = AddToSet(Dependencies, app);
        }

        public void RemoveDependency(string app)
        {
            Dependencies = RemoveFromSet(Dependencies, app);
        }

        public bool IsActive { get { return BoolValue(PropertyKeys.AppActivated); } }

        public string Url { get { return StringValue(PropertyKeys.AppUrl); } }

        public IDictionary<string, string> DownloadHeaders
        {
            get { return Value(PropertyKeys.AppDownloadHeaders) as IDictionary<string, string>; }
        }

        public IDictionary<string, string> DownloadCookies
        {
            get { return Value(PropertyKeys.AppDownloadCookies) as IDictionary<string, string>; }
        }

        public string ResourceFileName { get { return StringValue(PropertyKeys.AppResourceName); } }

        public string ResourceArchiveName { get { return StringValue(PropertyKeys.AppArchiveName); } }

        public string ResourceArchivePath { get { return StringValue(PropertyKeys.AppArchivePath); } }

        public string ResourceArchiveTyp { get { return StringValue(PropertyKeys.AppArchiveTyp); } }

        public bool Force
        {
            get { return BoolValue(PropertyKeys.AppForce); }
            set { UpdateValue(PropertyKeys.AppForce, value); }
        }

        public string PackageName { get { return StringValue(PropertyKeys.AppPackageName); } }

        public string Dir { get { return StringValue(PropertyKeys.AppDir); } }

        public string[] Path
        {
            get { return ListValue(PropertyKeys.AppPath); }
            set { UpdateValue(PropertyKeys.AppPath, value); }
        }

        public string Exe { get { return StringValue(PropertyKeys.AppExe); } }

        public bool Register { get { return BoolValue(PropertyKeys.AppRegister); } }

        public IDictionary<string, string> Environment
        {
            get { return Value(PropertyKeys.AppEnvironment) as IDictionary<string, string>; }
        }

        public string[] AdornedExecutables
        {
            get { return ListValue(PropertyKeys.AppAdornedExecutables); }
            set { UpdateValue(PropertyKeys.AppAdornedExecutables, value); }
        }

        public void AddAdornedExecutable(string path)
        {
            AdornedExecutables = AddToSet(AdornedExecutables, path);
        }

        public void RemoveAdornedExecutable(string path)
        {
            AdornedExecutables = RemoveFromSet(AdornedExecutables, path);
        }

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
                switch (Typ)
                {
                    case AppTyps.NodePackage:
                        var npmDir = AppIndex.GetStringGroupValue(AppKeys.Npm, PropertyKeys.AppDir);
                        var npmPackageDir = System.IO.Path.Combine(
                            System.IO.Path.Combine(npmDir, "node_modules"),
                            PackageName);
                        return System.IO.Directory.Exists(npmPackageDir);
                    case AppTyps.Python2Package:
                        var python2Dir = AppIndex.GetStringGroupValue(AppKeys.Python2, PropertyKeys.AppDir);
                        var pip2PackageDir = System.IO.Path.Combine(
                            System.IO.Path.Combine(python2Dir, "lib"),
                            System.IO.Path.Combine("site-packages", PackageName));
                        return System.IO.Directory.Exists(pip2PackageDir);
                    case AppTyps.Python3Package:
                        var python3Dir = AppIndex.GetStringGroupValue(AppKeys.Python3, PropertyKeys.AppDir);
                        var pip3PackageDir = System.IO.Path.Combine(
                            System.IO.Path.Combine(python3Dir, "lib"),
                            System.IO.Path.Combine("site-packages", PackageName));
                        return System.IO.Directory.Exists(pip3PackageDir);
                    default:
                        return System.IO.File.Exists(SetupTestFile);
                }
            }
        }

        public bool IsResourceCached
        {
            get
            {
                switch (Typ)
                {
                    case AppTyps.Default:
                        return ResourceFileName != null
                            ? File.Exists(System.IO.Path.Combine(AppIndex.GetStringValue(PropertyKeys.DownloadDir), ResourceFileName))
                            : ResourceArchiveName != null
                                ? File.Exists(System.IO.Path.Combine(AppIndex.GetStringValue(PropertyKeys.DownloadDir), ResourceArchiveName))
                                : true;
                    default:
                        return true;
                }
            }
        }

        public void Activate()
        {
            AppIndex.SetGroupValue(AppName, PropertyKeys.AppActivated, true);
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
            AppIndex.SetGroupValue(AppName, PropertyKeys.AppActivated, false);
        }

        public void SetupAutoConfiguration()
        {
            SetupAutoDependencies();
            SetupAdornmentForRegistryIsolation();
            SetupAdornmentPath();
        }

        private void SetupAutoDependencies()
        {
            switch (Typ)
            {
                case AppTyps.NodePackage:
                    AddDependency(AppKeys.Npm);
                    break;
                case AppTyps.Python2Package:
                    AddDependency(AppKeys.Python2);
                    break;
                case AppTyps.Python3Package:
                    AddDependency(AppKeys.Python3);
                    break;
            }
        }

        private void SetupAdornmentForRegistryIsolation()
        {
            if (RegistryKeys.Length > 0 && AdornedExecutables.Length == 0)
            {
                AddAdornedExecutable(Exe);
            }
        }

        private void SetupAdornmentPath()
        {
            if (AdornedExecutables.Length > 0)
            {
                var proxyDir = System.IO.Path.Combine(
                    AppIndex.GetStringValue(PropertyKeys.AppAdornmentBaseDir),
                    AppName.ToLowerInvariant());
                Path = AppendToList(Path, proxyDir);
            }
        }

        private static string[] AppendToList(string[] list, string value)
        {
            var result = new string[list.Length + 1];
            Array.Copy(list, result, list.Length);
            result[list.Length] = value;
            return result;
        }

        private static string[] AddToSet(string[] list, string value)
        {
            var result = new List<string>(list);
            if (!result.Contains(value))
            {
                result.Add(value);
                return result.ToArray();
            }
            else
            {
                return list;
            }
        }

        private static string[] RemoveFromSet(string[] list, string value)
        {
            var result = new List<string>(list);
            if (result.Contains(value))
            {
                result.Remove(value);
                return result.ToArray();
            }
            else
            {
                return list;
            }
        }

        public override string ToString()
        {
            return string.Format("App[{0}] {1}", Typ, ID);
        }
    }
}
