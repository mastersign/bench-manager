﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Mastersign.Bench
{
    public static class FileSystem
    {
        public static void EmptyDir(string path)
        {
            if (Directory.Exists(path))
            {
                Debug.WriteLine("Cleaning directory: " + path);
                foreach (var dir in Directory.GetDirectories(path))
                {
                    Directory.Delete(dir, true);
                }
                foreach (var file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                }
            }
            else
            {
                Debug.WriteLine("Creating directory: " + path);
                Directory.CreateDirectory(path);
            }
        }

        public static void AsureDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Debug.WriteLine("Creating directory: " + path);
                Directory.CreateDirectory(path);
            }
        }

        public static void PurgeDir(string path)
        {
            if (!Directory.Exists(path)) return;
            Debug.WriteLine("Purging directory: " + path);
            Directory.Delete(path, true);
        }

        public static void MoveContent(string sourceDir, string targetDir)
        {
            Debug.WriteLine("Moving content from: " + sourceDir + " to: " + targetDir);
            AsureDir(targetDir);
            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                Directory.Move(
                    dir,
                    Path.Combine(targetDir, Path.GetFileName(dir)));
            }
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                File.Move(
                    file,
                    Path.Combine(targetDir, Path.GetFileName(file)));
            }
        }
    }
}