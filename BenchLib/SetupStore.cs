using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Mastersign.Bench
{
    public class SetupStore
    {
        public BenchUserInfo UserInfo { get; set; }

        public SecureString SshPrivateKeyPassword { get; set; }
    }
}
