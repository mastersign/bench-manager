using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Mastersign.Bench.UI
{
    public partial class ProxyStepControl : WizzardStepControlBase
    {
        public ProxyStepControl()
        {
            Description = "Setup HTTP(S) proxy...";
            InitializeComponent();
        }

        public string HttpProxy
        {
            get { return txtHttpProxy.Text; }
            set { txtHttpProxy.Text = value; }
        }

        public string HttpsProxy
        {
            get { return txtHttpsProxy.Text; }
            set { txtHttpsProxy.Text = value; }
        }
    }
}
