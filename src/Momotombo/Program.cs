using System;
using System.IO;
using System.Linq;

namespace Momotombo
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("--green")) Console.ForegroundColor = ConsoleColor.DarkGreen;

            var baud = args.SingleOrDefault(p => p.StartsWith("--baud=")) is { } s && s.Split(new[] { '=' }, 2)[1] is { } v && int.TryParse(v, out var b) ? b : 300;
            var slp = (int)(1000.0 / (baud / 8.0));
            using var cin = Console.OpenStandardInput();
            using var cout = Console.OpenStandardOutput();
            using var sr = new BinaryReader(cin);
            using var sw = new BinaryWriter(cout);
            try
            {
                while (cin.CanRead)
                {
                    sw.Write(sr.ReadChar());
                    System.Threading.Thread.Sleep(slp);
                }
            }
            catch { }
        }
    }
}
