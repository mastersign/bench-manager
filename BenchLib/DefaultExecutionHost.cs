using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Mastersign.Bench
{
    public class DefaultExecutionHost : IProcessExecutionHost
    {
        public void StartProcess(BenchEnvironment env,
            string cwd, string exe, string arguments,
            ProcessExitCallback cb, ProcessMonitoring monitoring)
        {
            AsyncManager.StartTask(() =>
            {
                cb(RunProcess(env, cwd, exe, arguments, monitoring));
            });
        }

        public ProcessExecutionResult RunProcess(BenchEnvironment env,
            string cwd, string exe, string arguments,
            ProcessMonitoring monitoring)
        {
            var p = new Process();
            if (!File.Exists(exe))
            {
                throw new FileNotFoundException("The executable could not be found.", exe);
            }
            var collectOutput = (monitoring & ProcessMonitoring.Output) == ProcessMonitoring.Output;
            StringBuilder sb = null;
            var si = new ProcessStartInfo(exe, arguments);
            si.UseShellExecute = false;
            si.WorkingDirectory = cwd;
            si.CreateNoWindow = collectOutput;
            si.RedirectStandardOutput = collectOutput;
            si.RedirectStandardError = collectOutput;
            env.Load(si.EnvironmentVariables);
            p.StartInfo = si;
            if (collectOutput)
            {
                sb = new StringBuilder();
                p.OutputDataReceived += (s, e) => sb.AppendLine(e.Data);
                p.ErrorDataReceived += (s, e) => sb.AppendLine(e.Data);
            }
            p.Start();
            if (collectOutput)
            {
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
            }
            p.WaitForExit();
            if (collectOutput)
            {
                return new ProcessExecutionResult(p.ExitCode, sb.ToString());
            }
            else
            {
                return new ProcessExecutionResult(p.ExitCode);
            }
        }
    }
}
