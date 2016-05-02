using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mastersign.Bench.UI
{
    public partial class InitializeWizzardForm : Form
    {
        public InitializeWizzardForm()
        {
            InitializeComponent();
            picIcon.Image = new Icon(Icon, new Size(48, 48)).ToBitmap();
        }
    }
}
