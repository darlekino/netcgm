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
                var cgmFile = await CgmFile.ReadBinaryFormatAsync(fileName);
                var polylines = cgmFile.Commands.Where(c => c is Polyline).ToList();
                var disjointPolyline = cgmFile.Commands.Where(c => c is DisjointPolyline).ToList();
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