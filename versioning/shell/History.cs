using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versioning.Shell
{
    public class History
    {
        private static object? _syncRoot;
        private static object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
        }

        public static void WriteLine(string text)
        {
            WriteLine(Console.ForegroundColor, text);
        }

        public static void Error(string text)
        {
            WriteLine(ConsoleColor.DarkRed, text);
        }

        public static void WriteLine(ConsoleColor color, string text)
        {
            var old = Console.ForegroundColor;
            try
            {
                lock (SyncRoot)
                {
                    Console.ForegroundColor = color;
                    Console.WriteLine(text);
                }
            }
            finally
            {
                Console.ForegroundColor = old;
            }
        }
    }
}
