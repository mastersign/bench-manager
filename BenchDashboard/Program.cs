using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Mastersign.Bench.Dashboard
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Core = new Core(GetBenchRoot(args));

            Application.Run(new MainForm(Core));

            Core.Dispose();
            return 0;
        }

        private static string GetBenchRoot(string[] args)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "-root")
                {
                    return args[i + 1];
                }
            }
            var codeBase = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            var rootPath = Path.Combine(Path.Combine(Path.GetDirectoryName(codeBase), ".."), "..");
            return Path.GetFullPath(rootPath);
        }

        public static Core Core { get; private set; }
    }
}
