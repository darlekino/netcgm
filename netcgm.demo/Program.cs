using codessentials.CGM;
using System;
using System.IO;
using System.Threading.Tasks;

namespace netcgm.demo
{
    class Program
    {
        public static async Task Test()
        {
            var binaryFile = new BinaryCgmFile("D:\\projects\\job\\corvette.cgm");
            var cleanTextFile = new ClearTextCgmFile(binaryFile);

            //using var fileStream = File.CreateText("C:\\Users\\bodya\\Downloads\\corvette_clear_text.cgm");
            //await fileStream.WriteAsync(cleanTextFile.GetContent());
        }

        static async Task Main(string[] args)
        {
            await Task.Delay(500);
            Console.WriteLine("\n\nPress any key to close app...");
            Console.ReadKey();
        }
    }
}
