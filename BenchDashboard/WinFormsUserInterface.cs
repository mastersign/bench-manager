using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Mastersign.Bench.Dashboard
{
    class WinFormsUserInterface : IUserInterface
    {
        public BenchUserInfo ReadUserInfo(string prompt)
        {
            return UserInfoDialog.GetUserInfo(prompt);
        }

        public System.Security.SecureString ReadPassword(string prompt)
        {
            return PasswordDialog.GetPassword(prompt);
        }

        public void EditTextFile(string path)
        {
            var p = Process.Start(
                Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "notepad.exe"),
                path);
            p.WaitForExit();
        }

        public void EditTextFile(string path, string prompt)
        {
            MessageBox.Show(prompt
                + Environment.NewLine + Environment.NewLine
                + "Close the editor to continue.");
            EditTextFile(path);
        }
    }
}
