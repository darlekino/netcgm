using netcgm.Commands.GraphicalPrimitiveElements;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace netcgm.console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var fileName = "D:\\projects\\job\\EXAMPLES\\jcgm-core-master\\jcgm-core-master\\samples\\allelm01.cgm";
                foreach (var item in Enumerable.Range(0, 5000))
                {
                    var cgmFile = await CgmFile.ReadBinaryFormatAsync(fileName);
                    Console.WriteLine(item);
                    await Task.Delay(50);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("\n\nPress any key to close app...");
            Console.ReadKey();
        }
    }
}