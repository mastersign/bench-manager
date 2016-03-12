﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;

namespace Mastersign.Bench
{
    public class ConsoleUserInterface : IUserInterface
    {
        public BenchUserInfo ReadUserInfo(string prompt)
        {
            var userInfo = new BenchUserInfo();
            Console.WriteLine(prompt);
            Console.Write("Username: ");
            userInfo.Name = Console.ReadLine();
            Console.Write("Email: ");
            userInfo.Email = Console.ReadLine();
            return userInfo;
        }

        public SecureString ReadPassword(string prompt)
        {
            Console.WriteLine(prompt);

            // http://stackoverflow.com/questions/3404421/password-masking-console-application

            Console.Write("Password: ");
            SecureString pwd = new SecureString();
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.RemoveAt(pwd.Length - 1);
                        //Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd.AppendChar(i.KeyChar);
                    //Console.Write("*");
                }
            }
            Console.WriteLine();
            return pwd;
        }

        public void EditTextFile(string prompt, string file)
        {
            Console.WriteLine(prompt);
            Console.WriteLine("Close the text editor to proceed ...");
            var p = Process.Start(
                Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "notepad.exe"),
                file);
            p.WaitForExit();
        }
    }
}
