using System;
using System.Collections.Generic;
using System.Text;

namespace Mastersign.Bench
{
    public class BenchUserInfo
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public BenchUserInfo() { }

        public BenchUserInfo(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }
}
