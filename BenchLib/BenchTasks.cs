using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;

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
                AddUserInfoToCustomConfiguration(customConfigFile, setupStore.UserInfo);
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

        private static void AsureDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Debug.WriteLine("Creating directory: " + path);
                Directory.CreateDirectory(path);
            }
        }

        private static void AddUserInfoToCustomConfiguration(string file, BenchUserInfo userInfo)
        {
            using (var w = new StreamWriter(file, true, Encoding.UTF8))
            {
                w.WriteLine();
                w.WriteLine("The developer identity:");
                w.WriteLine();
                w.WriteLine("* UserName: " + userInfo.Name);
                w.WriteLine("* UserEmail: `" + userInfo.Email + "`");
            }
        }
    }

    public delegate BenchUserInfo UserInfoSource(string prompt);

    public delegate SecureString PasswordSource(string prompt);

    public delegate void TextFileEditor(string prompt, string filePath);
}
