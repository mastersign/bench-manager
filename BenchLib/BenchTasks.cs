using Ionic.Zip;
using System;
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
            FileSystem.AsureDir(customConfigDir);

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
            FileSystem.AsureDir(homeDir);
            FileSystem.AsureDir(Path.Combine(homeDir, "Desktop"));
            FileSystem.AsureDir(Path.Combine(homeDir, "Documents"));
            FileSystem.AsureDir(cfg.GetStringValue(PropertyKeys.AppDataDir));
            FileSystem.AsureDir(cfg.GetStringValue(PropertyKeys.LocalAppDataDir));

            var privateKeyFile = Path.Combine(homeDir,
                Path.Combine(".ssh", "id_rsa"));
            if (!File.Exists(privateKeyFile))
            {
                setupStore.SshPrivateKeyPassword = ui.ReadPassword(
                    "Enter the Password for the private key of the RSA key pair.");
            }

            FileSystem.AsureDir(cfg.GetStringValue(PropertyKeys.TempDir));
            FileSystem.AsureDir(cfg.GetStringValue(PropertyKeys.DownloadDir));
            FileSystem.AsureDir(cfg.GetStringValue(PropertyKeys.LibDir));

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
                //ui.EditTextFile(activationFile, "Activate some of the included apps.");
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
            downloader.UrlResolver.Clear();
            downloader.UrlResolver.Add(EclipseMirrorResolver);
            downloader.UrlResolver.Add(EclipseDownloadLinkResolver);
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


        public static AppTaskError RunCustomScript(BenchConfiguration config, IProcessExecutionHost execHost, string appId, string path, params string[] args)
        {
            var customScriptRunner = Path.Combine(config.GetStringValue(PropertyKeys.BenchScripts), "Run-CustomScript.ps1");
            var exitCode = PowerShell.RunScript(new BenchEnvironment(config), execHost, config.BenchRootDir, customScriptRunner,
                path, PowerShell.FormatArgumentList(args));
            return exitCode != 0
                ? new AppTaskError(appId, string.Format("Executing custom script '{0}' failed.", Path.GetFileName(path)))
                : null;
        }

        private static string CustomScriptDir(BenchConfiguration config)
        {
            return Path.Combine(config.GetStringValue(PropertyKeys.BenchAuto), "apps");
        }

        public static Process StartProcess(BenchEnvironment env, string cwd, string exe, string arguments)
        {
            if (!File.Exists(exe))
            {
                throw new FileNotFoundException("The executable could not be found.", exe);
            }

            var p = new Process();
            var si = new ProcessStartInfo(exe, arguments);
            si.UseShellExecute = false;
            si.WorkingDirectory = cwd;
            env.Load(si.EnvironmentVariables);
            p.StartInfo = si;
            p.Start();
            return p;
        }

        public static Process LaunchApp(BenchConfiguration config, BenchEnvironment env, string appId, string[] args)
        {
            var app = config.Apps[appId];
            var exe = app.LauncherExecutable;
            if (app.IsExecutableAdorned(exe)) exe = app.GetLauncherScriptFile();

            if (string.IsNullOrEmpty(exe))
            {
                throw new ArgumentException("The launcher executable is not set.");
            }
            return StartProcess(env, config.GetStringValue(PropertyKeys.HomeDir),
                exe, CommandLine.SubstituteArgumentList(app.LauncherArguments, args));
        }

        private static string PipExe(BenchConfiguration config, PythonVersion pyVer)
        {
            switch (pyVer)
            {
                case PythonVersion.Python2:
                    return Path.Combine(
                        Path.Combine(config.GetStringValue(PropertyKeys.LibDir), config.Apps[AppKeys.Python2].Dir),
                        @"Scripts\pip2.exe");
                case PythonVersion.Python3:
                    return Path.Combine(
                        Path.Combine(config.GetStringValue(PropertyKeys.LibDir), config.Apps[AppKeys.Python3].Dir),
                        @"Scripts\pip3.exe");
                default:
                    throw new NotSupportedException();
            }
        }

        #region Task Composition

        public static void RunTasks(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb,
            ICollection<AppFacade>[] taskApps, params BenchTask[] tasks)
        {
            if (tasks.Length == 0)
            {
                endCb(true, new AppTaskError[0]);
                return;
            }

            InternalRunTasks(man, progressCb, endCb,
                taskApps, new List<AppTaskError>(),
                tasks, 0);
        }

        public static void RunTasks(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb,
            params BenchTask[] tasks)
        {
            RunTasks(man, progressCb, endCb,
                new ICollection<AppFacade>[] { new List<AppFacade>(man.Config.Apps.ActiveApps) },
                tasks);
        }

        public static void RunTasks(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb,
            ICollection<AppFacade> apps,
            params BenchTask[] tasks)
        {
            RunTasks(man, progressCb, endCb,
                new ICollection<AppFacade>[] { apps },
                tasks);
        }

        public static void RunTasks(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb,
            string appId,
            params BenchTask[] tasks)
        {
            RunTasks(man, progressCb, endCb,
                new[] { man.Config.Apps[appId] },
                tasks);
        }

        private static T FindLastNotNull<T>(T[] a, int p)
        {
            p = Math.Min(a.Length - 1, p);
            while (p > 0 && a[p] == null) p--;
            return a[p];
        }

        private static void InternalRunTasks(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb,
            ICollection<AppFacade>[] taskApps, List<AppTaskError> collectedErrors,
            BenchTask[] tasks, int i)
        {
            var taskProgress = i / tasks.Length;
            ProgressCallback pcb = (info, err, progress) =>
            {
                if (progressCb != null)
                {
                    progressCb(info, err || collectedErrors.Count > 0, taskProgress + progress / tasks.Length);
                }
            };

            var apps = new List<AppFacade>(FindLastNotNull(taskApps, i));
            foreach (var err in collectedErrors)
            {
                apps.RemoveAll(app => app.ID == err.AppId);
            }

            tasks[i](man, pcb, (success, errors) =>
            {
                if (errors != null)
                {
                    collectedErrors.AddRange(errors);
                }
                if (i == tasks.Length - 1)
                {
                    if (progressCb != null)
                    {
                        progressCb("Finished.", collectedErrors.Count > 0, 1f);
                    }
                    if (endCb != null)
                    {
                        endCb(collectedErrors.Count == 0, collectedErrors);
                    }
                }
                else
                {
                    InternalRunTasks(man, progressCb, endCb, taskApps, collectedErrors, tasks, i + 1);
                }
            }, apps);
        }

        #endregion

        #region Download App Resources

        public static void DownloadAppResources(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb,
            ICollection<AppFacade> apps)
        {
            var targetDir = man.Config.GetStringValue(PropertyKeys.DownloadDir);
            FileSystem.AsureDir(targetDir);

            var tasks = new List<DownloadTask>();
            var finished = 0;
            var errorCnt = 0;

            EventHandler<DownloadEndEventArgs> downloadEndedHandler = (o, e) =>
            {
                finished++;
                if (e.HasFailed) errorCnt++;
                if (progressCb != null)
                {
                    progressCb(e.HasFailed ? e.ErrorMessage : "Finished download for " + e.Task.Id,
                       errorCnt > 0, (float)finished / tasks.Count);
                }
            };

            EventHandler workFinishedHandler = null;
            workFinishedHandler = (EventHandler)((o, e) =>
            {
                man.Downloader.DownloadEnded -= downloadEndedHandler;
                man.Downloader.WorkFinished -= workFinishedHandler;
                var errors = new List<AppTaskError>();
                foreach (var t in tasks)
                {
                    if (!t.Success) errors.Add(new AppTaskError(t.Id, t.ErrorMessage));
                }
                foreach (var app in apps)
                {
                    app.DiscardCachedValues();
                }
                if (progressCb != null)
                {
                    progressCb("Finished downloads", errors.Count > 0, 1.0f);
                }
                if (endCb != null)
                {
                    endCb(errors.Count == 0, errors);
                }
            });
            man.Downloader.DownloadEnded += downloadEndedHandler;
            man.Downloader.WorkFinished += workFinishedHandler;

            if (progressCb != null)
            {
                progressCb("Downloading app resources...", false, 0);
            }

            var selectedApps = new List<AppFacade>();
            foreach (var app in apps)
            {
                if (app.Typ == AppTyps.Default && app.HasResource && !app.IsResourceCached) selectedApps.Add(app);
            }

            foreach (var app in selectedApps)
            {
                var targetFile = app.ResourceFileName ?? app.ResourceArchiveName;
                var targetPath = Path.Combine(targetDir, targetFile);

                var task = new DownloadTask(app.ID, new Uri(app.Url), targetPath);
                task.Headers = app.DownloadHeaders;
                task.Cookies = app.DownloadCookies;
                tasks.Add(task);

                man.Downloader.Enqueue(task);
            }

            if (tasks.Count == 0)
            {
                man.Downloader.DownloadEnded -= downloadEndedHandler;
                man.Downloader.WorkFinished -= workFinishedHandler;
                if (progressCb != null)
                {
                    progressCb("Nothing to download", false, 1.0f);
                }
                if (endCb != null)
                {
                    endCb(true, null);
                }
            }
        }

        public static void DownloadAppResources(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb)
        {
            DownloadAppResources(man, progressCb, endCb, man.Config.Apps.ActiveApps);
        }

        public static void DownloadAppResources(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb,
            string appId)
        {
            DownloadAppResources(man, progressCb, endCb, new[] { man.Config.Apps[appId] });
        }

        #endregion

        #region Delete App Resources

        public static void DeleteAppResources(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb,
            ICollection<AppFacade> apps)
        {
            var downloadDir = man.Config.GetStringValue(PropertyKeys.DownloadDir);

            if (progressCb != null)
            {
                progressCb("Deleting app resources", false, 0);
            }

            AsyncManager.StartTask(() =>
            {
                var selectedApps = new List<AppFacade>();
                foreach (var app in apps)
                {
                    if (app.HasResource && app.IsResourceCached) selectedApps.Add(app);
                }

                var errors = new List<AppTaskError>();
                var cnt = 0;
                foreach (var app in selectedApps)
                {
                    cnt++;
                    var progress = (float)cnt / selectedApps.Count;

                    var resourceFile = app.ResourceFileName ?? app.ResourceArchiveName;
                    var resourcePath = Path.Combine(downloadDir, resourceFile);
                    try
                    {
                        File.Delete(resourcePath);
                    }
                    catch (Exception e)
                    {
                        errors.Add(new AppTaskError(app.ID, e.Message));
                        if (progressCb != null)
                        {
                            progressCb(e.Message, true, progress);
                        }
                        continue;
                    }
                    if (progressCb != null)
                    {
                        progressCb("Deleted app resources for " + app.ID, errors.Count > 0, progress);
                    }
                    app.DiscardCachedValues();
                }
                if (progressCb != null)
                {
                    progressCb("Finished deleting app resources", errors.Count > 0, 1.0f);
                }
                if (endCb != null)
                {
                    endCb(errors.Count == 0, errors);
                }
            });
        }

        public static void DeleteAppResources(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb)
        {
            DeleteAppResources(man, progressCb, endCb, new List<AppFacade>(man.Config.Apps));
        }

        public static void DeleteAppResources(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb,
            string appId)
        {
            DeleteAppResources(man, progressCb, endCb, new[] { man.Config.Apps[appId] });
        }

        #endregion

        #region Setup Environment

        public static void CleanExecutionProxies(BenchConfiguration config)
        {
            FileSystem.EmptyDir(config.GetStringValue(PropertyKeys.AppAdornmentBaseDir));
        }

        public static AppTaskError CreateExecutionProxies(BenchConfiguration config, AppFacade app)
        {
            var adornedExePaths = app.AdornedExecutables;
            if (adornedExePaths.Length > 0)
            {
                try
                {
                    var proxyBaseDir = FileSystem.EmptyDir(app.AdornmentProxyBasePath);
                    foreach (var exePath in adornedExePaths)
                    {
                        var proxyPath = app.GetExecutableProxy(exePath);
                        var code = new StringBuilder();
                        code.AppendLine("@ECHO OFF");
                        code.AppendLine(string.Format("runps Run-Adorned {0}, \"{1}\" %*", app.ID, exePath));
                        File.WriteAllText(proxyPath, code.ToString());
                    }
                }
                catch (Exception e)
                {
                    return new AppTaskError(app.ID, "Creating the execution proxy failed: " + e.Message);
                }
            }
            return null;
        }

        public static void CleanLaunchers(BenchConfiguration config)
        {
            FileSystem.EmptyDir(config.GetStringValue(PropertyKeys.LauncherDir));
            FileSystem.EmptyDir(config.GetStringValue(PropertyKeys.LauncherScriptDir));
        }

        private static void CreateBenchDashboardLauncher(BenchConfiguration config)
        {
            var benchDashboard = Path.Combine(config.GetStringValue(PropertyKeys.BenchAuto), @"bin\BenchDashboard.exe");
            var benchDashboardShortcut = Path.Combine(config.GetStringValue(PropertyKeys.LauncherDir), "Bench Dashboard.lnk");
            FileSystem.CreateShortcut(benchDashboardShortcut, benchDashboard,
                string.Format("-root \"{0}\"", config.BenchRootDir), config.BenchRootDir,
                benchDashboard);
        }

        private static void CreateActionLauncher(BenchConfiguration config, string label, string action, string icon)
        {
            var launcherDir = config.GetStringValue(PropertyKeys.LauncherDir);
            var actionDir = config.GetStringValue(PropertyKeys.ActionDir);
            var shortcut = Path.Combine(launcherDir, label + ".lnk");
            var target = Path.Combine(actionDir, action + ".cmd");
            FileSystem.CreateShortcut(shortcut, target, null, config.BenchRootDir, icon);
        }

        public static void CreateActionLaunchers(BenchConfiguration config)
        {
            CreateBenchDashboardLauncher(config);

            //CreateActionLauncher(config, "Bench Control", "bench-ctl", @"%SystemRoot%\System32\imageres.dll,109");
            CreateActionLauncher(config, "Command Line", "bench-cmd", @"%SystemRoot%\System32\cmd.exe");
            CreateActionLauncher(config, "PowerShell", "bench-ps", @"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe");
            CreateActionLauncher(config, "Bourne Again Shell", "bench-bash", @"%SystemRoot%\System32\imageres.dll,89");
        }

        public static AppTaskError CreateLauncher(BenchConfiguration config, AppFacade app)
        {
            var label = app.Launcher;
            if (label == null) return null;

            try
            {
                var executable = app.LauncherExecutable;
                var args = CommandLine.FormatArgumentList(app.LauncherArguments);
                var script = app.GetLauncherScriptFile();
                var autoDir = config.GetStringValue(PropertyKeys.BenchAuto);

                var code = new StringBuilder();
                code.AppendLine("@ECHO OFF");
                code.AppendLine(string.Format("ECHO.Launching {0} in Bench Context ...", label));
                code.AppendLine(string.Format("CALL \"{0}\\env.cmd\"", autoDir));
                if (app.IsExecutableAdorned(executable))
                {
                    code.AppendLine(string.Format("\"{0}\\runps.cmd\" Run-Adorned {1} \"{2}\" {3}",
                        autoDir, app.ID, executable, args));
                }
                else
                {
                    code.AppendLine(string.Format("START \"{0}\" \"{1}\" {2}", label, executable, args));
                }
                File.WriteAllText(script, code.ToString());

                var shortcut = app.GetLauncherFile();
                FileSystem.CreateShortcut(shortcut, script, null, config.BenchRootDir, app.LauncherIcon,
                    FileSystem.ShortcutWindowStyle.Minimized);
            }
            catch (Exception e)
            {
                return new AppTaskError(app.ID, "Creating the launcher failed: " + e.Message);
            }
            return null;
        }

        public static void UpdateEnvironment(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb,
            ICollection<AppFacade> _)
        {
            AsyncManager.StartTask(() =>
            {
                try
                {
                    man.Env.WriteEnvironmentFile();
                }
                catch (Exception e)
                {
                    progressCb("Writing the environment file failed: " + e.Message, true, 0f);
                    endCb(false, null);
                    return;
                }
                try
                {
                    CleanExecutionProxies(man.Config);
                }
                catch (Exception e)
                {
                    progressCb("Cleaning execution proxies failed: " + e.Message, true, 0f);
                    endCb(false, null);
                    return;
                }
                try
                {
                    CleanLaunchers(man.Config);
                }
                catch (Exception e)
                {
                    progressCb("Cleaning launchers failed: " + e.Message, true, 0f);
                    endCb(false, null);
                    return;
                }
                try
                {
                    CreateActionLaunchers(man.Config);
                }
                catch (Exception e)
                {
                    progressCb("Creating bench action launchers failed: " + e.Message, true, 0f);
                    endCb(false, null);
                    return;
                }
                var selectedApps = man.Config.Apps.ActiveApps;
                var errors = new List<AppTaskError>();
                var cnt = 0;
                foreach (var app in selectedApps)
                {
                    cnt++;
                    var progress = (float)cnt / selectedApps.Length;
                    AppTaskError error = null;
                    error = CreateExecutionProxies(man.Config, app);
                    if (error == null)
                    {
                        error = CreateLauncher(man.Config, app);
                    }
                    var envScript = app.GetCustomScriptFile("env");
                    if (error == null && envScript != null)
                    {
                        error = RunCustomScript(man.Config, man.ProcessExecutionHost, app.ID, envScript);
                    }
                    if (error != null) errors.Add(error);
                    if (progressCb != null)
                    {
                        progressCb("Setup environment for " + app.ID, errors.Count > 0, progress);
                    }
                }
                if (progressCb != null)
                {
                    progressCb("Finished updating environment.", errors.Count > 0, 1f);
                }
                if (endCb != null)
                {
                    endCb(errors.Count == 0, errors);
                }
            });
        }

        public static void UpdateEnvironment(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb)
        {
            UpdateEnvironment(man, progressCb, endCb, null);
        }

        #endregion

        #region Install Apps

        private static AppTaskError CopyAppResourceFile(BenchConfiguration config, AppFacade app)
        {
            var resourceFile = Path.Combine(config.GetStringValue(PropertyKeys.DownloadDir), app.ResourceFileName);
            if (!File.Exists(resourceFile))
            {
                return new AppTaskError(app.ID,
                    "Application resource not found: " + app.ResourceFileName);
            }
            var targetDir = Path.Combine(config.GetStringValue(PropertyKeys.LibDir), app.Dir);
            try
            {
                FileSystem.AsureDir(targetDir);
                File.Copy(resourceFile, Path.Combine(targetDir, app.ResourceFileName), true);
            }
            catch (Exception e)
            {
                return new AppTaskError(app.ID, e.Message);
            }
            return null;
        }

        public static void ExtractAppArchiveAsync(BenchConfiguration config, IProcessExecutionHost execHost,
            AppTaskCallback endCb,
            string appId)
        {
            AsyncManager.StartTask(() =>
            {
                var error = ExtractAppArchive(config, execHost, config.Apps[appId]);
                if (error != null)
                {
                    endCb(false, new[] { error });
                }
                else
                {
                    endCb(true, new AppTaskError[0]);
                }
            });
        }

        private static AppTaskError ExtractAppArchive(BenchConfiguration config, IProcessExecutionHost execHost, AppFacade app)
        {
            var tmpDir = Path.Combine(config.GetStringValue(PropertyKeys.TempDir), app.ID + "_extract");
            var archiveFile = Path.Combine(config.GetStringValue(PropertyKeys.DownloadDir), app.ResourceArchiveName);
            if (!File.Exists(archiveFile))
            {
                return new AppTaskError(app.ID,
                    "Application resource not found: " + app.ResourceArchiveName);
            }
            var targetDir = Path.Combine(config.GetStringValue(PropertyKeys.LibDir), app.Dir);
            var extractDir = app.ResourceArchivePath != null ? tmpDir : targetDir;
            FileSystem.AsureDir(extractDir);
            var success = false;
            var customExtractScript = app.GetCustomScriptFile("extract");
            switch (app.ResourceArchiveTyp)
            {
                case AppArchiveTyps.Auto:
                    if (customExtractScript != null)
                    {
                        success = RunCustomScript(config, execHost, app.ID, customExtractScript, archiveFile, extractDir) == null;
                    }
                    else if (archiveFile.EndsWith(".msi", StringComparison.InvariantCultureIgnoreCase))
                    {
                        success = ExtractMsiPackage(config, execHost, app.ID, archiveFile, extractDir);
                    }
                    else if (archiveFile.EndsWith(".0"))
                    {
                        success = ExtractInnoSetup(config, execHost, app.ID, archiveFile, extractDir);
                    }
                    else
                    {
                        success = ExtractArchiveGeneric(config, execHost, app.ID, archiveFile, extractDir);
                    }
                    break;
                case AppArchiveTyps.Generic:
                    success = ExtractArchiveGeneric(config, execHost, app.ID, archiveFile, extractDir);
                    break;
                case AppArchiveTyps.Msi:
                    success = ExtractMsiPackage(config, execHost, app.ID, archiveFile, extractDir);
                    break;
                case AppArchiveTyps.InnoSetup:
                    success = ExtractInnoSetup(config, execHost, app.ID, archiveFile, extractDir);
                    break;
                case AppArchiveTyps.Custom:
                    success = customExtractScript != null
                        ? RunCustomScript(config, execHost, app.ID, customExtractScript, archiveFile, extractDir) == null
                        : false;
                    break;
            }
            if (!success)
            {
                return new AppTaskError(app.ID,
                    "Extracting application resource failed: " + app.ResourceArchiveName);
            }
            if (app.ResourceArchivePath != null)
            {
                try
                {
                    FileSystem.PurgeDir(targetDir);
                    FileSystem.MoveContent(Path.Combine(extractDir, app.ResourceArchivePath), targetDir);
                    FileSystem.PurgeDir(extractDir);
                }
                catch (Exception e)
                {
                    return new AppTaskError(app.ID,
                        "Moving extracted application resources into the target location failed: " + e.Message);
                }
            }
            return null;
        }

        private static bool ExtractArchiveGeneric(BenchConfiguration config, IProcessExecutionHost execHost, string id,
            string archiveFile, string targetDir)
        {
            var sevenZipExe = config.Apps[AppKeys.SevenZip].Exe;
            if (sevenZipExe != null && File.Exists(sevenZipExe))
            {
                return ExtractArchive7z(config, execHost, id, archiveFile, targetDir);
            }
            else
            {
                return ExtractArchiveClr(archiveFile, targetDir);
            }
        }

        private static bool ExtractArchiveClr(string archiveFile, string targetDir)
        {
            try
            {
                var zip = new ZipFile(archiveFile);
                zip.ExtractAll(targetDir, ExtractExistingFileAction.OverwriteSilently);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool ExtractArchive7z(BenchConfiguration config, IProcessExecutionHost execHost, string id,
            string archiveFile, string targetDir)
        {
            if (Regex.IsMatch(Path.GetFileName(archiveFile), @"\.tar\.\w+$",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                var tmpDir = Path.Combine(config.GetStringValue(PropertyKeys.TempDir), id + "_tar");
                try
                {
                    FileSystem.EmptyDir(tmpDir);
                }
                catch (Exception)
                {
                    return false;
                }
                if (Run7zExtract(config, execHost, archiveFile, tmpDir))
                {
                    // extracting the compressed file succeeded, extracting tar
                    var tarFile = Path.Combine(tmpDir, Path.GetFileNameWithoutExtension(archiveFile));
                    var success = Run7zExtract(config, execHost, tarFile, targetDir);
                    try
                    {
                        FileSystem.PurgeDir(tmpDir);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    return success;
                }
                else
                {
                    // extracting the compressed tar file failed
                    try
                    {
                        FileSystem.PurgeDir(tmpDir);
                    }
                    catch (Exception)
                    {
                        // nothing
                    }
                    return false;
                }
            }
            else
            {
                return Run7zExtract(config, execHost, archiveFile, targetDir);
            }
        }

        private static bool Run7zExtract(BenchConfiguration config, IProcessExecutionHost execHost,
            string archiveFile, string targetDir)
        {
            var svnZipExe = config.Apps[AppKeys.SevenZip].Exe;
            if (svnZipExe == null || !File.Exists(svnZipExe)) return false;
            var env = new BenchEnvironment(config);
            var exitCode = execHost.RunProcess(env, targetDir, svnZipExe,
                    CommandLine.FormatArgumentList("x", "-y", "-bd", "-o" + targetDir, archiveFile));
            return exitCode == 0;
        }

        private static bool ExtractMsiPackage(BenchConfiguration config, IProcessExecutionHost execHost,
            string id, string archiveFile, string targetDir)
        {
            var lessMsiExe = config.Apps[AppKeys.LessMSI].Exe;
            if (lessMsiExe == null || !File.Exists(lessMsiExe)) return false;
            var env = new BenchEnvironment(config);
            var exitCode = execHost.RunProcess(env, targetDir, lessMsiExe,
                CommandLine.FormatArgumentList("x", archiveFile, @".\"));
            return exitCode == 0;
        }

        private static bool ExtractInnoSetup(BenchConfiguration config, IProcessExecutionHost execHost,
            string id, string archiveFile, string targetDir)
        {
            var innoUnpExe = config.Apps[AppKeys.InnoSetupUnpacker].Exe;
            if (innoUnpExe == null || !File.Exists(innoUnpExe)) return false;
            var env = new BenchEnvironment(config);
            var exitCode = execHost.RunProcess(env, targetDir, innoUnpExe,
                CommandLine.FormatArgumentList("-q", "-x", archiveFile));
            return exitCode == 0;
        }

        public static AppTaskError InstallNodePackage(BenchConfiguration config, IProcessExecutionHost execHost, AppFacade app)
        {
            var npmExe = config.Apps[AppKeys.Npm].Exe;
            if (npmExe == null || !File.Exists(npmExe)) return new AppTaskError(app.ID, "The NodeJS package manager was not found.");
            var packageName = app.Version != null
                ? string.Format("{0}@{1}", app.PackageName, app.Version)
                : app.PackageName;
            var exitCode = execHost.RunProcess(new BenchEnvironment(config), config.BenchRootDir, npmExe,
                CommandLine.FormatArgumentList("install", packageName, "--global"));
            if (exitCode != 0)
            {
                return new AppTaskError(app.ID,
                    "Installing the NPM package " + app.PackageName + " failed with exit code " + exitCode + ".");
            }
            return null;
        }

        public static AppTaskError InstallPythonPackage(BenchConfiguration config, IProcessExecutionHost execHost, PythonVersion pyVer, AppFacade app)
        {
            var pipExe = PipExe(config, pyVer);
            if (pipExe == null)
            {
                return new AppTaskError(app.ID, "The " + pyVer + " package manager PIP was not found.");
            }

            var args = new List<string>();
            args.Add("install");
            args.Add(app.PackageName);
            if (app.Version != null) args.Add(app.Version);
            if (app.IsInstalled) args.Add("--upgrade");
            args.Add("--quiet");

            var exitCode = execHost.RunProcess(new BenchEnvironment(config), config.BenchRootDir, pipExe,
                    CommandLine.FormatArgumentList(args.ToArray()));

            if (exitCode != 0)
            {
                return new AppTaskError(app.ID,
                    "Installing the " + pyVer + " package " + app.PackageName + " failed with exit code " + exitCode + ".");
            }
            return null;
        }

        public static void InstallApps(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb,
            ICollection<AppFacade> apps)
        {
            AsyncManager.StartTask(() =>
            {
                var selectedApps = new List<AppFacade>();
                foreach (var app in apps)
                {
                    if (!app.CanCheckInstallation || !app.IsInstalled || app.Force)
                    {
                        selectedApps.Add(app);
                    }
                }

                var errors = new List<AppTaskError>();
                var cnt = 0;

                foreach (var app in selectedApps)
                {
                    cnt++;
                    var progress = (float)cnt / selectedApps.Count;

                    AppTaskError error = null;

                    // 1. Extraction / Installation
                    if (progressCb != null)
                    {
                        progressCb(string.Format("Installing app {0}.", app.ID), errors.Count > 0, progress);
                    }
                    switch (app.Typ)
                    {
                        case AppTyps.Meta:
                            // no resource extraction
                            break;
                        case AppTyps.Default:
                            if (app.ResourceFileName != null)
                            {
                                error = CopyAppResourceFile(man.Config, app);
                            }
                            else if (app.ResourceArchiveName != null)
                            {
                                error = ExtractAppArchive(man.Config, man.ProcessExecutionHost, app);
                            }
                            break;
                        case AppTyps.NodePackage:
                            error = InstallNodePackage(man.Config, man.ProcessExecutionHost, app);
                            break;
                        case AppTyps.Python2Package:
                            error = InstallPythonPackage(man.Config, man.ProcessExecutionHost, PythonVersion.Python2, app);
                            break;
                        case AppTyps.Python3Package:
                            error = InstallPythonPackage(man.Config, man.ProcessExecutionHost, PythonVersion.Python3, app);
                            break;
                        default:
                            error = new AppTaskError(app.ID, "Unknown app typ: '" + app.Typ + "'.");
                            break;
                    }
                    if (error != null)
                    {
                        errors.Add(error);
                        if (progressCb != null)
                        {
                            progressCb(error.ErrorMessage, true, progress);
                        }
                        continue;
                    }

                    // 2. Custom Setup-Script
                    var customSetupScript = app.GetCustomScriptFile("setup");
                    if (customSetupScript != null)
                    {
                        if (progressCb != null)
                        {
                            progressCb(string.Format("Executing custom setup script for {0}.", app.ID), errors.Count > 0, progress);
                        }
                        error = RunCustomScript(man.Config, man.ProcessExecutionHost, app.ID, customSetupScript);
                        if (error != null)
                        {
                            errors.Add(error);
                            if (progressCb != null)
                            {
                                progressCb(string.Format("Execution of custom setup script for {0} failed.", app.ID), true, progress);
                            }
                            continue;
                        }
                    }

                    // 3. Create Execution Proxy
                    error = CreateExecutionProxies(man.Config, app);
                    if (error != null)
                    {
                        errors.Add(error);
                        if (progressCb != null)
                        {
                            progressCb(string.Format("Creating the execution proxy for {0} failed.", app.ID), true, progress);
                        }
                        continue;
                    }

                    // 4. Create Launcher
                    error = CreateLauncher(man.Config, app);
                    if (error != null)
                    {
                        errors.Add(error);
                        if (progressCb != null)
                        {
                            progressCb(string.Format("Creating the launcher for {0} failed.", app.ID), true, progress);
                        }
                        continue;
                    }

                    // 5. Run Custom Environment Script
                    var envScript = app.GetCustomScriptFile("env");
                    if (envScript != null)
                    {
                        error = RunCustomScript(man.Config, man.ProcessExecutionHost, app.ID, envScript);
                        if (error != null)
                        {
                            errors.Add(error);
                            if (progressCb != null)
                            {
                                progressCb(string.Format("Running the custom environment script for {0} failed.", app.ID), true, progress);
                            }
                            continue;
                        }
                    }

                    app.DiscardCachedValues();
                }
                if (progressCb != null)
                {
                    progressCb("Finished installing apps.", errors.Count > 0, 1f);
                }
                if (endCb != null)
                {
                    endCb(errors.Count == 0, errors);
                }
            });
        }

        public static void InstallApps(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb)
        {
            InstallApps(man, progressCb, endCb, man.Config.Apps.ActiveApps);
        }

        public static void InstallApp(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb,
            string appId)
        {
            InstallApps(man, progressCb, endCb, new[] { man.Config.Apps[appId] });
        }

        #endregion

        #region Uninstall App

        public static AppTaskError UninstallGeneric(BenchConfiguration config, AppFacade app)
        {
            var appDir = app.Dir;
            if (appDir != null)
            {
                try
                {
                    FileSystem.PurgeDir(appDir);
                }
                catch (Exception e)
                {
                    return new AppTaskError(app.ID,
                        string.Format("Uninstalling app {0} failed: {1}", app.ID, e.Message));
                }
            }
            return null;
        }

        public static AppTaskError UninstallNodePackage(BenchConfiguration config, IProcessExecutionHost execHost, AppFacade app)
        {
            var npmExe = config.Apps[AppKeys.Npm].Exe;
            if (npmExe == null || !File.Exists(npmExe)) return new AppTaskError(app.ID, "The NodeJS package manager was not found.");
            var exitCode = execHost.RunProcess(new BenchEnvironment(config), config.BenchRootDir, npmExe,
                CommandLine.FormatArgumentList("uninstall", app.PackageName, "--global"));
            if (exitCode != 0)
            {
                return new AppTaskError(app.ID,
                    "Uninstalling the NPM package " + app.PackageName + " failed with exit code " + exitCode + ".");
            }
            return null;
        }

        public static AppTaskError UninstallPythonPackage(BenchConfiguration config, IProcessExecutionHost execHost, PythonVersion pyVer, AppFacade app)
        {
            var pipExe = PipExe(config, pyVer);
            if (pipExe == null)
            {
                return new AppTaskError(app.ID, "The " + pyVer + " package manager PIP was not found.");
            }

            var exitCode = execHost.RunProcess(new BenchEnvironment(config), config.BenchRootDir, pipExe,
                    CommandLine.FormatArgumentList("uninstall", app.PackageName, "--yes", "--quiet"));

            if (exitCode != 0)
            {
                return new AppTaskError(app.ID,
                    "Uninstalling the " + pyVer + " package " + app.PackageName + " failed with exit code " + exitCode + ".");
            }
            return null;
        }

        public static void UninstallApps(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb,
            ICollection<AppFacade> apps)
        {
            AsyncManager.StartTask(() =>
            {
                var selectedApps = new List<AppFacade>();
                foreach (var app in apps)
                {
                    if (app.IsInstalled) selectedApps.Add(app);
                }
                selectedApps.Reverse();

                var errors = new List<AppTaskError>();
                var cnt = 0;

                foreach (var app in selectedApps)
                {
                    cnt++;
                    var progress = (float)cnt / selectedApps.Count;

                    AppTaskError error = null;
                    if (progressCb != null)
                    {
                        progressCb(string.Format("Uninstalling app {0}.", app.ID), errors.Count > 0, progress);
                    }
                    switch (app.Typ)
                    {
                        case AppTyps.Meta:
                            error = UninstallGeneric(man.Config, app);
                            break;
                        case AppTyps.Default:
                            error = UninstallGeneric(man.Config, app);
                            break;
                        case AppTyps.NodePackage:
                            error = UninstallNodePackage(man.Config, man.ProcessExecutionHost, app);
                            break;
                        case AppTyps.Python2Package:
                            error = UninstallPythonPackage(man.Config, man.ProcessExecutionHost, PythonVersion.Python2, app);
                            break;
                        case AppTyps.Python3Package:
                            error = UninstallPythonPackage(man.Config, man.ProcessExecutionHost, PythonVersion.Python3, app);
                            break;
                        default:
                            error = new AppTaskError(app.ID, "Unknown app typ: '" + app.Typ + "'.");
                            break;
                    }
                    app.DiscardCachedValues();
                }
                if (progressCb != null)
                {
                    progressCb("Finished uninstalling apps.", errors.Count > 0, 1f);
                }
                if (endCb != null)
                {
                    endCb(errors.Count == 0, errors);
                }
            });
        }

        public static void UninstallApps(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb)
        {
            AsyncManager.StartTask(() =>
            {
                if (progressCb != null)
                {
                    progressCb("Uninstalling all apps.", false, 0f);
                }
                var success = false;
                var libDir = man.Config.GetStringValue(PropertyKeys.LibDir);
                if (libDir != null && Directory.Exists(libDir))
                {
                    try
                    {
                        FileSystem.EmptyDir(libDir);
                        success = true;
                    }
                    catch (Exception e)
                    {
                        if (progressCb != null)
                        {
                            progressCb("Uninstalling apps failed: " + e.Message, true, 1f);
                        }
                    }
                    if (success)
                    {
                        if (progressCb != null)
                        {
                            progressCb("Finished uninstalling apps.", false, 1f);
                        }
                    }
                }
                if (endCb != null)
                {
                    endCb(success, null);
                }
            });
        }

        public static void UninstallApp(IBenchManager man,
            ProgressCallback progressCb, AppTaskCallback endCb,
            string appId)
        {
            UninstallApps(man, progressCb, endCb, new[] { man.Config.Apps[appId] });
        }

        #endregion
    }
}
