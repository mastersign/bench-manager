using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public class AppTaskError
    {
        public string AppId { get; private set; }

        public string ErrorMessage { get; private set; }

        public AppTaskError(string id, string message)
        {
            AppId = id;
            ErrorMessage = message;
        }

        public override string ToString()
        {
            return string.Format("Error for '{0}': {1}", AppId, ErrorMessage);
        }
    }
}
