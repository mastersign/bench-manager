using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

                ui.EditTextFile("Adapt the custom configuration to your preferences.",
                    customConfigFile);

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
                ui.EditTextFile("Activate some of the included apps.", activationFile);
            }
            var deactivationFile = cfg.GetStringValue(PropertyKeys.AppDeactivationFile);
            if (!File.Exists(deactivationFile))
            {
                var deactivationTemplateFile = cfg.GetStringValue(PropertyKeys.AppDeactivationTemplateFile);
                File.Copy(deactivationTemplateFile, deactivationFile, false);
            }

            return new BenchConfiguration(benchRootDir, true, true);
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

        public static void DownloadAppResources(BenchConfiguration config, Downloader downloader)
        {
            var targetDir = config.GetStringValue(PropertyKeys.DownloadDir);
            AsureDir(targetDir);

            downloader.UrlResolver.Clear();
            downloader.UrlResolver.Add(EclipseMirrorResolver);
            downloader.UrlResolver.Add(EclipseDownloadLinkResolver);

            foreach (var app in config.Apps.ActiveApps)
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

                downloader.Enqueue(task);
            }
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
