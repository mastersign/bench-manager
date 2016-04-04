﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Mastersign.Bench
{
    public static class BenchTasks
    {
        public static BenchConfiguration PrepareConfiguration(
            string benchRootDir,
            SetupStore setupStore,
            IUserInterface ui)
        {
            var cfg = new BenchConfiguration(benchRootDir, false, true);

            var customConfigDir = cfg.GetStringValue(PropertyKeys.CustomConfigDir);
            AsureDir(customConfigDir);

            var customConfigFile = cfg.GetStringValue(PropertyKeys.CustomConfigFile);
            var customConfigFileExists = File.Exists(customConfigFile);

            if (!customConfigFileExists)
            {
                var customConfigTemplateFile = cfg.GetStringValue(PropertyKeys.CustomConfigTemplateFile);
                File.Copy(customConfigTemplateFile, customConfigFile, false);

                setupStore.UserInfo = ui.ReadUserInfo("Please enter the name and email of your developer identity.");
                setupStore.ProxyInfo = BenchProxyInfo.SystemDefault;

                var updates = new Dictionary<string, string>();
                setupStore.UserInfo.Transfer(updates);
                setupStore.ProxyInfo.Transfer(updates);
                MarkdownHelper.UpdateFile(customConfigFile, updates);

                ui.EditTextFile(customConfigFile,
                    "Adapt the custom configuration to your preferences.");

                cfg = new BenchConfiguration(benchRootDir, false, true);
            }
            else
            {
                setupStore.UserInfo = new BenchUserInfo(
                    cfg.GetStringValue(PropertyKeys.UserName),
                    cfg.GetStringValue(PropertyKeys.UserEmail));
            }

            var homeDir = cfg.GetStringValue(PropertyKeys.HomeDir);
            AsureDir(homeDir);
            AsureDir(Path.Combine(homeDir, "Desktop"));
            AsureDir(Path.Combine(homeDir, "Documents"));
            AsureDir(cfg.GetStringValue(PropertyKeys.AppDataDir));
            AsureDir(cfg.GetStringValue(PropertyKeys.LocalAppDataDir));

            var privateKeyFile = Path.Combine(homeDir,
                Path.Combine(".ssh", "id_rsa"));
            if (!File.Exists(privateKeyFile))
            {
                setupStore.SshPrivateKeyPassword = ui.ReadPassword(
                    "Enter the Password for the private key of the RSA key pair.");
            }

            AsureDir(cfg.GetStringValue(PropertyKeys.TempDir));
            AsureDir(cfg.GetStringValue(PropertyKeys.DownloadDir));
            AsureDir(cfg.GetStringValue(PropertyKeys.LibDir));

            var customAppIndexFile = cfg.GetStringValue(PropertyKeys.CustomAppIndexFile);
            if (!File.Exists(customAppIndexFile))
            {
                var customAppIndexTemplateFile = cfg.GetStringValue(PropertyKeys.CustomAppIndexTemplateFile);
                File.Copy(customAppIndexTemplateFile, customAppIndexFile, false);
            }

            var activationFile = cfg.GetStringValue(PropertyKeys.AppActivationFile);
            if (!File.Exists(activationFile))
            {
                var activationTemplateFile = cfg.GetStringValue(PropertyKeys.AppActivationTemplateFile);
                File.Copy(activationTemplateFile, activationFile, false);
                ui.EditTextFile(activationFile, "Activate some of the included apps.");
            }
            var deactivationFile = cfg.GetStringValue(PropertyKeys.AppDeactivationFile);
            if (!File.Exists(deactivationFile))
            {
                var deactivationTemplateFile = cfg.GetStringValue(PropertyKeys.AppDeactivationTemplateFile);
                File.Copy(deactivationTemplateFile, deactivationFile, false);
            }

            return new BenchConfiguration(benchRootDir, true, true);
        }

        public static Downloader InitializeDownloader(BenchConfiguration config)
        {
            var parallelDownloads = config.GetInt32Value(PropertyKeys.ParallelDownloads, 1);
            var downloadAttempts = config.GetInt32Value(PropertyKeys.DownloadAttempts, 1);
            var useProxy = config.GetBooleanValue(PropertyKeys.UseProxy);
            var httpProxy = config.GetStringValue(PropertyKeys.HttpProxy);
            var httpsProxy = config.GetStringValue(PropertyKeys.HttpsProxy);
            var proxyBypass = config.GetStringListValue(PropertyKeys.ProxyBypass);
            var downloader = new Downloader(parallelDownloads);
            downloader.DownloadAttempts = downloadAttempts;
            if (useProxy)
            {
                downloader.HttpProxy = new WebProxy(httpProxy, true, proxyBypass);
                downloader.HttpsProxy = new WebProxy(httpsProxy, true, proxyBypass);
            }
            return downloader;
        }

        private static IUrlResolver EclipseDownloadLinkResolver = new HtmlLinkUrlResolver(
            new UrlPattern(
                new Regex(@"^www\.eclipse\.org$"),
                new Regex(@"^/downloads/download\.php"),
                new Dictionary<string, Regex> { { "file", null } }),
            new UrlPattern(
                null,
                new Regex(@"download\.php$"),
                new Dictionary<string, Regex> {
                    { "file", null },
                    { "mirror_id", new Regex(@"^\d+$") }
                }));

        private static IUrlResolver EclipseMirrorResolver = new SurroundedHtmlLinkUrlResolver(
            new UrlPattern(
                new Regex(@"^www\.eclipse\.org$"),
                new Regex(@"^/downloads/download\.php"),
                new Dictionary<string, Regex>
                {
                    {"file", null },
                    {"mirror_id", new Regex(@"^\d+$") }
                }),
                new Regex(@"\<span\s[^\>]*class=""direct-link""[^\>]*\>(.*?)\</span\>"));

        public static void DownloadAppResources(BenchConfiguration config, Downloader downloader, AppTaskCallback cb, IEnumerable<AppFacade> apps)
        {
            var targetDir = config.GetStringValue(PropertyKeys.DownloadDir);
            AsureDir(targetDir);

            var tasks = new List<DownloadTask>();

            EventHandler finishedHandler = null;
            finishedHandler = (EventHandler)((o, e) =>
            {
                downloader.WorkFinished -= finishedHandler;
                var errors = new List<AppTaskError>();
                foreach (var t in tasks)
                {
                    if (!t.Success) errors.Add(new AppTaskError(t.Id, t.ErrorMessage));
                }
                cb(errors.Count == 0, errors);
            });
            downloader.WorkFinished += finishedHandler;

            downloader.UrlResolver.Clear();
            downloader.UrlResolver.Add(EclipseMirrorResolver);
            downloader.UrlResolver.Add(EclipseDownloadLinkResolver);

            foreach (var app in apps)
            {
                if (app.Typ != AppTyps.Default) continue;

                var targetFile = app.ResourceFileName ?? app.ResourceArchiveName;
                if (targetFile == null)
                {
                    Debug.WriteLine("Skipped app " + app.ID + " because of missing resource name.");
                    continue;
                }
                var targetPath = Path.Combine(targetDir, targetFile);
                if (File.Exists(targetPath))
                {
                    Debug.WriteLine("Skipped app " + app.ID + " because resource already exists.");
                    continue;
                }

                var task = new DownloadTask(app.ID, new Uri(app.Url), targetPath);
                task.Headers = app.DownloadHeaders;
                task.Cookies = app.DownloadCookies;
                tasks.Add(task);

                downloader.Enqueue(task);
            }

            if (tasks.Count == 0)
            {
                downloader.WorkFinished -= finishedHandler;
                cb(true, null);
            }
        }

        public static void DownloadAppResources(BenchConfiguration config, Downloader downloader, AppTaskCallback cb)
        {
            DownloadAppResources(config, downloader, cb, config.Apps.ActiveApps);
        }

        public static void DownloadAppResources(BenchConfiguration config, Downloader downloader, AppTaskCallback cb, string appId)
        {
            DownloadAppResources(config, downloader, cb, new[] { config.Apps[appId] });
        }

        public static void DeleteAppResources(BenchConfiguration config, IEnumerable<AppFacade> apps)
        {
            var downloadDir = config.GetStringValue(PropertyKeys.DownloadDir);

            foreach (var app in apps)
            {
                if (app.Typ != AppTyps.Default) continue;

                var resourceFile = app.ResourceFileName ?? app.ResourceArchiveName;
                if (resourceFile == null)
                {
                    Debug.WriteLine("Skipped app " + app.ID + " because of missing resource name.");
                    continue;
                }
                var resourcePath = Path.Combine(downloadDir, resourceFile);
                if (File.Exists(resourcePath))
                {
                    File.Delete(resourcePath);
                }
            }
        }

        public static void DeleteAppResources(BenchConfiguration config)
        {
            DeleteAppResources(config, config.Apps);
        }

        public static void DeleteAppResources(BenchConfiguration config, string appId)
        {
            DeleteAppResources(config, new[] { config.Apps[appId] });
        }

        public static Process LaunchApp(BenchConfiguration config, BenchEnvironment env, string appId, string[] args)
        {
            var app = config.Apps[appId];
            var exe = app.LauncherExecutable;

            if (string.IsNullOrEmpty(exe))
            {
                throw new ArgumentException("The launcher executable is not set.");
            }
            return StartProcess(env, config.GetStringValue(PropertyKeys.HomeDir),
                exe, ProcessArguments(app.LauncherArguments, args));
        }

        public static Process StartProcess(BenchEnvironment env, string cwd, string exe, string[] args)
        {
            return StartProcess(env, cwd, exe, FormatArguments(args));
        }

        public static Process StartProcess(BenchEnvironment env, string cwd, string exe, string arguments)
        {
            var p = new Process();
            if (!File.Exists(exe))
            {
                throw new FileNotFoundException("The executable could not be found.", exe);
            }

            var si = new ProcessStartInfo(exe, arguments);
            si.UseShellExecute = false;
            si.WorkingDirectory = cwd;
            env.Load(si.EnvironmentVariables);
            p.StartInfo = si;
            p.Start();
            return p;
        }

        private static string ProcessArguments(string[] launcherArguments, string[] args)
        {
            var result = new List<string>();
            for (int i = 0; i < launcherArguments.Length; i++)
            {
                var arg = SubstituteArgument(launcherArguments[i], args);
                if (arg == "%*")
                {
                    result.AddRange(args);
                }
                else
                {
                    result.Add(arg);
                }
            }
            return FormatArguments(result.ToArray());
        }

        private static string FormatArguments(params string[] args)
        {
            var list = new string[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                list[i] = EscapeArgument(args[i]);
            }
            return string.Join(" ", list);
        }

        private static string SubstituteArgument(string arg, string[] args)
        {
            arg = Environment.ExpandEnvironmentVariables(arg);
            for (int i = 0; i < 10; i++)
            {
                var v = args.Length > i ? args[i] : "";
                arg = arg.Replace("%" + i, v);
            }
            return arg;
        }

        private static string EscapeArgument(string arg)
        {
            var s = Regex.Replace(arg.Trim('"'), @"(\\*)" + "\"", @"$1$1\" + "\"");
            s = "\"" + Regex.Replace(s, @"(\\+)$", @"$1$1") + "\"";
            return s;
        }

        private static void AsureDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Debug.WriteLine("Creating directory: " + path);
                Directory.CreateDirectory(path);
            }
        }
    }

}
