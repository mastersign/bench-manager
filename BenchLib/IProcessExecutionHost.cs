using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public interface IProcessExecutionHost
    {
        void StartProcess(BenchEnvironment env, string cwd, string executable, string arguments, ProcessExitCallback cb);

        int RunProcess(BenchEnvironment env, string cwd, string executable, string arguments);
    }
}
