using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mastersign.Bench.Dashboard.Properties;

namespace Mastersign.Bench.Dashboard
{
    public partial class MarkdownViewer : Form
    {
        private readonly static string template;

        static MarkdownViewer()
        {
            template = Resources.MarkdownViewerTemplate.Replace("$CSS$", Resources.MarkdownViewerStyle);
        }

        private readonly IBenchManager core;
        private readonly string windowTitle;
        private string tempFile;

        public MarkdownViewer(IBenchManager core)
        {
            this.core = core;
            InitializeComponent();
            this.windowTitle = Text;
        }

        private void MarkdownViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            TempFile = null;
        }

        private string TempFile
        {
            get { return tempFile; }
            set
            {
                if (tempFile != null && File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
                tempFile = value;
            }
        }

        private string NewTempFilePath()
        {
            return Path.Combine(
                core.Config.GetStringValue(PropertyKeys.TempDir),
                Path.GetRandomFileName() + ".html");
        }

        public void LoadMarkdown(string file, string title = null)
        {
            TempFile = NewTempFilePath();
            title = title ?? Path.GetFileNameWithoutExtension(file);
            Text = windowTitle + " - " + title;
            string source;
            string html;
            try
            {
                source = File.ReadAllText(file, Encoding.UTF8);
                html = new MarkdownSharp.Markdown().Transform(source);
                html = template.Replace("$TITLE$", title).Replace("$CONTENT$", html);
                File.WriteAllText(TempFile, html, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                TempFile = null;
                return;
            }
            webBrowser.Navigate(TempFile);
        }
    }
}
