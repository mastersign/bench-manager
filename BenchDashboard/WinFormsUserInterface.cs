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

        public void EditTextFile(string prompt, string file)
        {
            MessageBox.Show(prompt 
                + Environment.NewLine + Environment.NewLine
                + "Close the editor to continue.");
            var p = Process.Start(
                Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "notepad.exe"),
                file);
            p.WaitForExit();
        }
    }
}
