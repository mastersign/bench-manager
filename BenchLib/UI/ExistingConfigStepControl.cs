using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Mastersign.Bench.UI
{
    public partial class ExistingConfigStepControl : WizzardStepControlBase
    {
        public ExistingConfigStepControl()
        {
            Description = "Choose pre-existing configuration...";
            InitializeComponent();
        }

        public bool IsConfigGitRepoExisting
        {
            get { return radExistingConfigInGitRepo.Checked; }
            set
            {
                if (value)
                {
                    radExistingConfigInGitRepo.Checked = true;
                }
                else
                {
                    radNoExistingConfig.Checked = true;
                }
            }
        }

        public string ConfigGitRepo
        {
            get { return txtConfigGitRepo.Text; }
            set { txtConfigGitRepo.Text = value; }
        }

        private void radExistingConfigInGitRepo_CheckedChanged(object sender, EventArgs e)
        {
            lblConfigGitRepo.Enabled = radExistingConfigInGitRepo.Checked;
            txtConfigGitRepo.Enabled = radExistingConfigInGitRepo.Checked;
        }
    }
}
