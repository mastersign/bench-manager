using System.Security;

namespace Mastersign.Bench
{
    public delegate bool PropertyCriteria(string group, string name);

    public delegate string BasePathSource(string group, string name);

    public delegate BenchUserInfo UserInfoSource(string prompt);

    public delegate SecureString PasswordSource(string prompt);

    public delegate void TextFileEditor(string prompt, string filePath);
}