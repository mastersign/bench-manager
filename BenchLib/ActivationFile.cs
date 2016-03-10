using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Mastersign.Bench
{
    public class ActivationFile : IEnumerable<string>
    {
        private static readonly Regex SpaceExp = new Regex(@"\s");

        private readonly string FilePath;

        public ActivationFile(string path)
        {
            FilePath = path;
        }

        private static bool IsValidLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return false;
            if (line.StartsWith("#")) return false;
            return true;
        }

        private static string CleanLine(string line)
        {
            var m = SpaceExp.Match(line);
            if (m.Success)
            {
                return line.Substring(0, m.Index);
            }
            else
            {
                return line;
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            if (!File.Exists(FilePath)) yield break;
            using (var r = File.OpenText(FilePath))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (IsValidLine(line))
                    {
                        yield return CleanLine(line);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
