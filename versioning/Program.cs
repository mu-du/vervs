using System;
using System.Linq;

using versioning.shell;

namespace versioning
{
    class Program
    {
        static int Main(string[] args)
        {
            Version? version = typeof(Program).Assembly.GetName().Version;
            string title = $"C# Code Versioning for Visual Studio, v{version}";
            Console.Title = title;

            int result = -1;
            try
            {
                var mainMenu = new MainMenu();
                result = mainMenu.Run(args, title);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            return result;
        }

        private static void Help()
        {
            Console.WriteLine($"Versioning for Visual Studio, v{System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version}");
            Console.WriteLine("     Copyright Mudu (c) 2024. All rights reserved");
            Console.WriteLine();
        }
    }
}
