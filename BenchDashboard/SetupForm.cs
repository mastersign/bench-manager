using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Mastersign.Bench.Dashboard
{
    public partial class SetupForm : Form
    {
        private readonly Core core;

        public SetupForm(Core core)
        {
            this.core = core;
            InitializeComponent();
            InitializeDownloadList();
        }

        private void InitializeDownloadList()
        {
            downloadList.Downloader = core.Downloader;
            IsDownloadListVisible = true;
        }

        protected bool IsDownloadListVisible
        {
            get { return downloadList.Visible; }
            set
            {
                SuspendLayout();
                downloadList.Visible = value;
                splitterBottom.Visible = value;
                ResumeLayout();
            }
        }

        private void EditTextFile(string name, string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show(
                    "File of " + name + " not found."
                    + Environment.NewLine + Environment.NewLine
                    + path,
                    "Edit " + name + " ...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            core.UI.EditTextFile(path);
        }

        private void EditCustomConfigHandler(object sender, EventArgs e)
        {
            EditTextFile("Custom Configuration",
                core.Config.GetStringValue(PropertyKeys.CustomConfigFile));
        }

        private void EditCustomAppsHandler(object sender, EventArgs e)
        {
            EditTextFile("Custom App Index",
                core.Config.GetStringValue(PropertyKeys.CustomAppIndexFile));
        }

        private void ActivationListHandler(object sender, EventArgs e)
        {
            EditTextFile("App Activation",
                core.Config.GetStringValue(PropertyKeys.AppActivationFile));
        }

        private void DeactivationListHandler(object sender, EventArgs e)
        {
            EditTextFile("App Deactivation",
                core.Config.GetStringValue(PropertyKeys.AppDeactivationFile));
        }

        private void DownloadAllHandler(object sender, EventArgs e)
        {
            core.DownloadAppResources();
        }
    }
}
