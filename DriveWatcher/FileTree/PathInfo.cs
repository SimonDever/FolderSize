using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTree
{
    public class PathInfo
    {
        /// <summary>
        /// A directory size is 0 bytes under windows
        /// </summary>
        /// 
        public static long DirSize = 0;
        //public PathInfo(string path, bool isFile)
        //{
        //    Path = path;
        //    IsFile = isFile;
        //    Size = 0;
        //}
        public string Path { get;  set; }
        public bool IsFile { get;  set; }
        public long Size {get; set;}
        
    }
}
