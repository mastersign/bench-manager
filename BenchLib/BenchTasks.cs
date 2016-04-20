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

        public static void DownloadAppResources(BenchConfiguration config, Downloader downloader,
            ProgressCallback progressCb, AppTaskCallback endCb,
            ICollection<AppFacade> apps)
        {
            var targetDir = config.GetStringValue(PropertyKeys.DownloadDir);
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
                downloader.DownloadEnded -= downloadEndedHandler;
                downloader.WorkFinished -= workFinishedHandler;
                var errors = new List<AppTaskError>();
                foreach (var t in tasks)
                {
                    if (!t.Success) errors.Add(new AppTaskError(t.Id, t.ErrorMessage));
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
            downloader.DownloadEnded += downloadEndedHandler;
            downloader.WorkFinished += workFinishedHandler;

            downloader.UrlResolver.Clear();
            downloader.UrlResolver.Add(EclipseMirrorResolver);
            downloader.UrlResolver.Add(EclipseDownloadLinkResolver);

            if (progressCb != null)
            {
                progressCb("Starting downloads...", false, 0);
            }

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
                downloader.DownloadEnded -= downloadEndedHandler;
                downloader.WorkFinished -= workFinishedHandler;
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

        public static void DownloadAppResources(BenchConfiguration config, Downloader downloader,
            ProgressCallback progressCb, AppTaskCallback endCb)
        {
            DownloadAppResources(config, downloader, progressCb, endCb, config.Apps.ActiveApps);
        }

        public static void DownloadAppResources(BenchConfiguration config, Downloader downloader,
            ProgressCallback progressCb, AppTaskCallback endCb,
            string appId)
        {
            DownloadAppResources(config, downloader, progressCb, endCb, new[] { config.Apps[appId] });
        }

        public static void DeleteAppResources(BenchConfiguration config,
            ProgressCallback progressCb, AppTaskCallback endCb,
            ICollection<AppFacade> apps)
        {
            var downloadDir = config.GetStringValue(PropertyKeys.DownloadDir);

            if (progressCb != null)
            {
                progressCb("Deleting app resources", false, 0);
            }

            AsyncManager.StartTask(() =>
            {
                var errors = new List<AppTaskError>();
                var cnt = 0;
                foreach (var app in apps)
                {
                    cnt++;
                    if (app.Typ != AppTyps.Default) continue;

                    try
                    {
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
                        if (progressCb != null)
                        {
                            progressCb("Deleted app resources for " + app.ID, errors.Count > 0, (float)cnt / apps.Count);
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add(new AppTaskError(app.ID, e.Message));
                        if (progressCb != null)
                        {
                            progressCb(e.Message, true, (float)cnt / apps.Count);
                        }
                    }
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

        public static void DeleteAppResources(BenchConfiguration config,
            ProgressCallback progressCb, AppTaskCallback endCb)
        {
            DeleteAppResources(config, progressCb, endCb, new List<AppFacade>(config.Apps));
        }

        public static void DeleteAppResources(BenchConfiguration config,
            ProgressCallback progressCb, AppTaskCallback endCb,
            string appId)
        {
            DeleteAppResources(config, progressCb, endCb, new[] { config.Apps[appId] });
        }

        private static AppTaskError CopyAppResourceFile(BenchConfiguration config, string appId)
        {
            var app = config.Apps[appId];
            var resourceFile = Path.Combine(config.GetStringValue(PropertyKeys.DownloadDir), app.ResourceFileName);
            if (!File.Exists(resourceFile))
            {
                return new AppTaskError(appId,
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
                return new AppTaskError(appId, e.Message);
            }
            return null;
        }

        public static void ExtractAppArchiveAsync(BenchConfiguration config, IProcessExecutionHost execHost,
            AppTaskCallback endCb,
            string appId)
        {
            AsyncManager.StartTask(() =>
            {
                var error = ExtractAppArchive(config, execHost, appId);
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

        private static AppTaskError ExtractAppArchive(BenchConfiguration config, IProcessExecutionHost execHost, string appId)
        {
            var tmpDir = Path.Combine(config.GetStringValue(PropertyKeys.TempDir), appId + "_extract");
            var app = config.Apps[appId];
            var archiveFile = Path.Combine(config.GetStringValue(PropertyKeys.DownloadDir), app.ResourceArchiveName);
            if (!File.Exists(archiveFile))
            {
                return new AppTaskError(appId,
                    "Application resource not found: " + app.ResourceArchiveName);
            }
            var targetDir = Path.Combine(config.GetStringValue(PropertyKeys.LibDir), app.Dir);
            var extractDir = app.ResourceArchivePath != null ? tmpDir : targetDir;
            FileSystem.AsureDir(extractDir);
            var success = false;
            var customExtractScript = CustomScript(config, "extract", app);
            switch (app.ResourceArchiveTyp)
            {
                case AppArchiveTyps.Auto:
                    if (customExtractScript != null)
                    {
                        success = RunCustomScript(config, execHost, appId, customExtractScript, archiveFile, extractDir) == null;
                    }
                    else if (archiveFile.EndsWith(".msi", StringComparison.InvariantCultureIgnoreCase))
                    {
                        success = ExtractMsiPackage(config, execHost, appId, archiveFile, extractDir);
                    }
                    else if (archiveFile.EndsWith(".0"))
                    {
                        success = ExtractInnoSetup(config, execHost, appId, archiveFile, extractDir);
                    }
                    else
                    {
                        success = ExtractArchiveGeneric(config, execHost, appId, archiveFile, extractDir);
                    }
                    break;
                case AppArchiveTyps.Generic:
                    success = ExtractArchiveGeneric(config, execHost, appId, archiveFile, extractDir);
                    break;
                case AppArchiveTyps.Msi:
                    success = ExtractMsiPackage(config, execHost, appId, archiveFile, extractDir);
                    break;
                case AppArchiveTyps.InnoSetup:
                    success = ExtractInnoSetup(config, execHost, appId, archiveFile, extractDir);
                    break;
                case AppArchiveTyps.Custom:
                    success = customExtractScript != null
                        ? RunCustomScript(config, execHost, appId, customExtractScript, archiveFile, extractDir) == null
                        : false;
                    break;
            }
            if (!success)
            {
                return new AppTaskError(appId,
                    "Extracting application resource failed: " + app.ResourceArchiveName);
            }
            if (app.ResourceArchivePath != null)
            {
                FileSystem.PurgeDir(targetDir);
                FileSystem.MoveContent(Path.Combine(extractDir, app.ResourceArchivePath), targetDir);
                FileSystem.PurgeDir(extractDir);
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
                FileSystem.EmptyDir(tmpDir);
                if (Run7zExtract(config, execHost, archiveFile, tmpDir))
                {
                    // extracting the compressed file succeeded, extracting tar
                    var tarFile = Path.Combine(tmpDir, Path.GetFileNameWithoutExtension(archiveFile));
                    var success = Run7zExtract(config, execHost, tarFile, targetDir);
                    FileSystem.PurgeDir(tmpDir);
                    return success;
                }
                else
                {
                    // extracting the compressed tar file failed
                    FileSystem.PurgeDir(tmpDir);
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
            if (lessMsiExe == null) return false;
            var env = new BenchEnvironment(config);
            var exitCode = execHost.RunProcess(env, targetDir, lessMsiExe,
                CommandLine.FormatArgumentList("x", archiveFile, @".\"));
            return exitCode == 0;
        }

        private static bool ExtractInnoSetup(BenchConfiguration config, IProcessExecutionHost execHost,
            string id, string archiveFile, string targetDir)
        {
            var innoUnpExe = config.Apps[AppKeys.InnoSetupUnpacker].Exe;
            if (innoUnpExe == null) return false;
            var env = new BenchEnvironment(config);
            var exitCode = execHost.RunProcess(env, targetDir, innoUnpExe,
                CommandLine.FormatArgumentList("-q", "-x", archiveFile));
            return exitCode == 0;
        }

        public static AppTaskError InstallNodePackage(BenchConfiguration config, IProcessExecutionHost execHost, string appId)
        {
            var npmExe = config.Apps[AppKeys.Npm].Exe;
            if (npmExe == null) return new AppTaskError(appId, "The NodeJS package manager was not found.");
            var app = config.Apps[appId];
            var packageName = app.Version != null
                ? string.Format("{0}@{1}", app.PackageName, app.Version)
                : app.PackageName;
            var exitCode = execHost.RunProcess(new BenchEnvironment(config), config.BenchRootDir, npmExe,
                CommandLine.FormatArgumentList("install", packageName, "--global"));
            if (exitCode != 0)
            {
                return new AppTaskError(appId,
                    "Installing the NPM package " + app.PackageName + " failed with exit code " + exitCode + ".");
            }
            return null;
        }

        public static AppTaskError InstallPythonPackage(BenchConfiguration config, IProcessExecutionHost execHost, PythonVersion pyVer, string appId)
        {
            string pipExe = null;
            switch (pyVer)
            {
                case PythonVersion.Python2:
                    pipExe = Path.Combine(
                        Path.Combine(config.GetStringValue(PropertyKeys.LibDir), config.Apps[AppKeys.Python2].Dir),
                        @"Scripts\pip2.exe");
                    break;
                case PythonVersion.Python3:
                    pipExe = Path.Combine(
                        Path.Combine(config.GetStringValue(PropertyKeys.LibDir), config.Apps[AppKeys.Python3].Dir),
                        @"Scripts\pip3.exe");
                    break;
            }
            if (pipExe == null) return new AppTaskError(appId, "The " + pyVer + " package manager PIP was not found.");
            var app = config.Apps[appId];

            var args = new List<string>();
            args.Add("install");
            if (app.Force) args.Add("--upgrade");
            args.Add(app.PackageName);
            if (app.Version != null) args.Add(app.Version);

            var exitCode = execHost.RunProcess(new BenchEnvironment(config), config.BenchRootDir, pipExe,
                    CommandLine.FormatArgumentList(args.ToArray()));

            if (exitCode != 0)
            {
                return new AppTaskError(appId,
                    "Installing the " + pyVer + " package " + app.PackageName + " failed with exit code " + exitCode + ".");
            }
            return null;
        }

        public static void InstallApps(BenchConfiguration config, IProcessExecutionHost execHost,
            ProgressCallback progressCb, AppTaskCallback endCb,
            ICollection<AppFacade> apps)
        {
            AsyncManager.StartTask(() =>
            {
                var errors = new List<AppTaskError>();
                var cnt = 0;
                foreach (var app in apps)
                {
                    cnt++;
                    var progress = (float)cnt / apps.Count;

                    AppTaskError error = null;
                    if (!app.IsInstalled)
                    {
                        // 1. Extraction / Installation
                        progressCb(string.Format("Installing app {0}.", app.ID), errors.Count > 0, progress);
                        switch (app.Typ)
                        {
                            case AppTyps.Meta:
                                // no resource extraction
                                break;
                            case AppTyps.Default:
                                if (app.ResourceFileName != null)
                                {
                                    error = CopyAppResourceFile(config, app.ID);
                                }
                                else if (app.ResourceArchiveName != null)
                                {
                                    error = ExtractAppArchive(config, execHost, app.ID);
                                }
                                break;
                            case AppTyps.NodePackage:
                                error = InstallNodePackage(config, execHost, app.ID);
                                break;
                            case AppTyps.Python2Package:
                                error = InstallPythonPackage(config, execHost, PythonVersion.Python2, app.ID);
                                break;
                            case AppTyps.Python3Package:
                                error = InstallPythonPackage(config, execHost, PythonVersion.Python3, app.ID);
                                break;
                            default:
                                error = new AppTaskError(app.ID, "Unknown app typ: '" + app.Typ + "'.");
                                break;
                        }
                        if (error != null)
                        {
                            errors.Add(error);
                            progressCb(error.ErrorMessage, true, progress);
                            continue;
                        }
                        // 2. Custom Setup-Script
                        var customSetupScript = CustomScript(config, "setup", app);
                        if (customSetupScript != null)
                        {
                            progressCb(string.Format("Executing custom setup script for {0}.", app.ID), errors.Count > 0, progress);
                            error = RunCustomScript(config, execHost, app.ID, customSetupScript);
                            if (error != null)
                            {
                                errors.Add(error);
                                progressCb(string.Format("Execution of custom setup script for {0} failed.", app.ID), true, progress);
                            }
                        }
                        // TODO 3. Create Execution Proxy
                        // TODO 4. Create Launcher
                    }
                }
                progressCb("Finished installing apps.", errors.Count > 0, 1f);
                endCb(errors.Count == 0, errors);
            });
        }

        public static AppTaskError RunCustomScript(BenchConfiguration config, IProcessExecutionHost execHost, string appId, string path, params string[] args)
        {
            var customScriptRunner = Path.Combine(config.GetStringValue(PropertyKeys.BenchScripts), "Run-CustomScript.ps1");
            var exitCode = PowerShell.RunScript(new BenchEnvironment(config), execHost, config.BenchRootDir, customScriptRunner,
                path, PowerShell.FormatArgumentList(args));
            return exitCode != 0
                ? new AppTaskError(appId, string.Format("Executing custom script '{0}' failed.", Path.GetFileName(path)))
                : null;
        }

        private static string CustomScript(BenchConfiguration config, string typ, AppFacade app)
        {
            var path = Path.Combine(CustomScriptDir(config),
                app.ID.ToLowerInvariant() + "." + typ + ".ps1");
            return File.Exists(path) ? path : null;
        }

        private static string CustomScriptDir(BenchConfiguration config)
        {
            return Path.Combine(config.GetStringValue(PropertyKeys.BenchAuto), "apps");
        }

        public static void InstallApps(BenchConfiguration config, IProcessExecutionHost execHost,
            ProgressCallback progressCb, AppTaskCallback endCb)
        {
            InstallApps(config, execHost, progressCb, endCb, config.Apps.ActiveApps);
        }

        public static void InstallApp(BenchConfiguration config, IProcessExecutionHost execHost,
            ProgressCallback progressCb, AppTaskCallback endCb,
            string appId)
        {
            InstallApps(config, execHost, progressCb, endCb, new[] { config.Apps[appId] });
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
                exe, CommandLine.SubstituteArgumentList(app.LauncherArguments, args));
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
    }

}
