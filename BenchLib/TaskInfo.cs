using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public abstract class TaskInfo
    {
        public DateTime Timestamp { get; private set; }

        public string AppId { get; private set; }

        public string Message { get; private set; }

        public string ProcessOutput { get; private set; }

        protected TaskInfo(string message, string appId, string processOutput)
        {
            if (message == null) throw new ArgumentNullException("message");
            Timestamp = DateTime.Now;
            AppId = appId;
            Message = message;
            ProcessOutput = processOutput;
        }
    }

    public class TaskProgress : TaskInfo
    {
        public float Progress { get; private set; }

        public TaskProgress(string message, float progress, string appId = null, string processOutput = null)
            : base(message, appId, processOutput)
        {
            Progress = progress;
        }

        public TaskProgress ScaleProgress(float globalBase, float factor)
        {
            return new TaskProgress(
                Message,
                globalBase + Progress * factor,
                AppId,
                ProcessOutput);
        }
    }

    public class TaskError : TaskInfo
    {
        public Exception Exception { get; private set; }

        public TaskError(string message, string appId = null, string processOutput = null, Exception exception = null)
            : base(message, appId, processOutput)
        {
            Exception = exception;
        }
    }
}
