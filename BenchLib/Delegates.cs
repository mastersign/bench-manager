﻿using System.Collections.Generic;
using System.Security;

namespace Mastersign.Bench
{
    public delegate bool PropertyCriteria(string group, string name);

    public delegate string BasePathSource(string group, string name);

    public delegate BenchUserInfo UserInfoSource(string prompt);

    public delegate SecureString PasswordSource(string prompt);

    public delegate void TextFileEditor(string prompt, string filePath);

    public delegate void DictionaryEntryHandler(string key, string value);

    public delegate void AppTaskCallback(bool success, ICollection<AppTaskError> errors);

    public delegate void ProgressCallback(string info, bool errors, float progress);

    public delegate void ProcessExitCallback(int exitCode);
}