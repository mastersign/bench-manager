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
        public void StartProcess(BenchEnvironment env, string cwd, string exe, string arguments, ProcessExitCallback cb)
        {
            new Thread(() => cb(RunProcess(env, cwd, exe, arguments))).Start();
        }

        public int RunProcess(BenchEnvironment env, string cwd, string exe, string arguments)
        {
            var p = new Process();
            if (!File.Exists(exe))
            {
                throw new FileNotFoundException("The executable could not be found.", exe);
            }

            var si = new ProcessStartInfo(exe, arguments);
            si.UseShellExecute = false;
            si.WorkingDirectory = cwd;
            env.Load(si.EnvironmentVariables);
            p.StartInfo = si;
            p.Start();
            p.WaitForExit();
            return p.ExitCode;
        }
    }
}
