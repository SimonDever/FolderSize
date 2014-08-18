using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileTree
{
    public static class FastFileOperations
    {
        public const int MAX_PATH = 260;
        public const int MAX_ALTERNATE = 14;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WIN32_FIND_DATA
        {
            public FileAttributes dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public uint nFileSizeHigh; //changed all to uint, otherwise you run into unexpected overflow
            public uint nFileSizeLow;  //|
            public uint dwReserved0;   //|
            public uint dwReserved1;   //v
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ALTERNATE)]
            public string cAlternate;
        }

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll")]
        public static extern bool FindClose(IntPtr hFindFile);

        public static uint GetFileSize(string filename)
        {
            WIN32_FIND_DATA findData;
            FindFirstFile(filename, out findData);
            return findData.nFileSizeLow;
        }

        public static PathInfo[] GetDirectoriesAndFiles(string path)
        {
            IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            WIN32_FIND_DATA findData;

            IntPtr findHandle;
            List<PathInfo> ret = new List<PathInfo>();
            // please note that the following line won't work if you try this on a network folder, like \\Machine\C$
            // simply remove the \\?\ part in this case or use \\?\UNC\ prefix
            findHandle = FindFirstFile(@"\\?\" + path + @"\*", out findData);
            if (findHandle != INVALID_HANDLE_VALUE)
            {
                do
                {
                    bool isFile = ((findData.dwFileAttributes & FileAttributes.Directory) == 0);
                    string subdirectory = path + (path.EndsWith(@"\") ? "" : @"\") + findData.cFileName;
                    ret.Add(new PathInfo { Path = subdirectory, IsFile = isFile, Size = isFile ? GetFileSize(subdirectory) : 0 });
                }
                while (FindNextFile(findHandle, out findData));
                FindClose(findHandle);
            }
            return ret.ToArray();
        }

        public static string[] GetFiles(string path)
        {
            IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            WIN32_FIND_DATA findData;

            IntPtr findHandle;
            List<string> ret = new List<string>();

            findHandle = FindFirstFile(@"\\?\" + path + @"\*", out findData);
            if (findHandle != INVALID_HANDLE_VALUE)
            {
                do
                {
                    if ((findData.dwFileAttributes & FileAttributes.Directory) == 0)
                    {

                        string subdirectory = path + (path.EndsWith(@"\") ? "" : @"\") + findData.cFileName;
                        ret.Add(subdirectory);
                    }
                }
                while (FindNextFile(findHandle, out findData));
                FindClose(findHandle);
            }
            return ret.ToArray();
        }
    }
}
