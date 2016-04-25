using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public static class PowerShell
    {
        public static string Executable
        {
            get
            {
                return Environment.ExpandEnvironmentVariables(@"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe");
            }
        }

        public static string FormatArgumentList(params string[] args)
        {
            var list = new string[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                list[i] = CommandLine.EscapeArgument(args[i]);
            }
            return "@(" + string.Join(", ", list) + ")";
        }

        public static ProcessExecutionResult RunScript(BenchEnvironment env, IProcessExecutionHost execHost, string cwd, string script, params string[] args)
        {
            var command = Convert.ToBase64String(Encoding.Unicode.GetBytes(
                string.Format("{0} {1}", script, string.Join(" ", args))));
            return execHost.RunProcess(env, cwd, PowerShell.Executable,
                CommandLine.FormatArgumentList(
                    "-NoLogo", "-NoProfile", "-ExecutionPolicy", "Unrestricted",
                    "-EncodedCommand", command),
                ProcessMonitoring.ExitCodeAndOutput);
        }
    }
}
