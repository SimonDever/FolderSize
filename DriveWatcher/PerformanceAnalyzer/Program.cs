using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTree;
namespace PerformanceAnalyzer
{
    class Program
    {
        static TimeSpan ExecutionTime(Action a)
        {
            var t = DateTime.Now;
            a();
            return (DateTime.Now - t).Duration();
        }
        static void Main(string[] args)
        {
            var path = @"C:\";

            var l = FastFileOperations.GetDirectoriesAndFiles(path);

            Console.WriteLine("Reading {0} files and diectories took {1} milliseconds", path, ExecutionTime(() => FastFileOperations.GetDirectoriesAndFiles(path)).Milliseconds);
            Console.WriteLine("Reading {0} files and diectories took {1} milliseconds", path, ExecutionTime(() => Directory.GetDirectories(path)).Milliseconds + ExecutionTime(() => Directory.GetFiles(path)).Milliseconds);
        }
    }
}
