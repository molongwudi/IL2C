﻿using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace IL2C
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: il2c.exe <target assembly> <output path>");
                return 0;
            }

            var assemblyPath = args[0];
            var outputPath = args[1];

            if (Directory.Exists(outputPath) == false)
            {
                try
                {
                    Directory.CreateDirectory(outputPath);
                }
                catch
                {
                }
            }

            var assembly = Assembly.LoadFrom(assemblyPath);

            var translateContext = new TranslateContext(assembly);
            var assemblyName = assembly.GetName().Name;
            var filePath = Path.Combine(outputPath, assemblyName);

            using (var fsSource = new FileStream(
                filePath + ".c",
                FileMode.Create,
                FileAccess.ReadWrite,
                FileShare.None))
            {
                var twSource = new StreamWriter(fsSource, Encoding.UTF8);

                Converter.ConvertToSourceCode(twSource, assembly, translateContext, "    ");

                twSource.Flush();
            }

            using (var fsHeader = new FileStream(
                filePath + ".h",
                FileMode.Create,
                FileAccess.ReadWrite,
                FileShare.None))
            {
                var twHeader = new StreamWriter(fsHeader, Encoding.UTF8);

                Converter.ConvertToHeader(twHeader, assembly, translateContext, "    ");

                twHeader.Flush();
            }

            return 0;
        }
    }
}
