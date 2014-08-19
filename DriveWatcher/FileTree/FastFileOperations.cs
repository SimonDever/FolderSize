using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileTree
{
    public static class FastFileOperations
    {
        static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        public static ulong GetFileSize(string path)
        {
            Kernel32.WIN32_FIND_DATA findData;
            var findHandle = Kernel32.FindFirstFileW(@"\\?\" + path + "*", out findData);
            if (findHandle == INVALID_HANDLE_VALUE)
                throw new IOException(String.Format("Cannot open file {0}.", path));
            if (findData.nFileSizeLow == 0)
                Debugger.Break();
            return GetSizeByHandle(ref findData);
        }
        public static bool IsFile(string path)
        {
            Kernel32.WIN32_FIND_DATA findData;
            var findHandle = Kernel32.FindFirstFileW(@"\\?\" + path + "*", out findData);
            return IsFile(ref findData);
        }
        public static PathInfo[] GetDirectoriesAndFiles(string path)
        {
            path += path.EndsWith(@"\") ? "" : @"\";
            
            Kernel32.WIN32_FIND_DATA findData;

            IntPtr findHandle;
            List<PathInfo> ret = new List<PathInfo>();
            // please note that the following line won't work if you try this on a network folder, like \\Machine\C$
            // simply remove the \\?\ part in this case or use \\?\UNC\ prefix
            findHandle = Kernel32.FindFirstFileW(@"\\?\" + path + "*", out findData);
            if (findHandle != INVALID_HANDLE_VALUE)
            {
                do
                {
                    if (findData.cFileName == "." || findData.cFileName == "..")
                        continue;
                    bool isFile = IsFile(ref findData);
                    string subdirectory = path + findData.cFileName;
                    ret.Add(new PathInfo { Path = subdirectory, IsFile = isFile, Size = GetSizeByHandle(ref findData) });
                }
                while (Kernel32.FindNextFileW(findHandle, out findData));
                Kernel32.FindClose(findHandle);
            }
            return ret.ToArray();
        }

        private static ulong GetSizeByHandle(ref Kernel32.WIN32_FIND_DATA findData)
        {
            return IsFile(ref findData) ? (findData.nFileSizeHigh * (((ulong)uint.MaxValue) + 1)) + findData.nFileSizeLow : 0;
        }

        private static bool IsFile(ref Kernel32.WIN32_FIND_DATA findData)
        {
            return ((findData.dwFileAttributes & FileAttributes.Directory) == 0);
        }

        #region Create Tree

        public static NTree<PathInfo> GetDirectoryTree(string path, out ulong FileCount, out ulong DirectoryCount)
        {
            var cur_dir = path;
            var bfsQ = new Queue<NNode<PathInfo>>();
            var root = new NNode<PathInfo>(new PathInfo { Path = path });
            var cur_node = root;
            bfsQ.Enqueue(root);
            ulong dirCounter = 1, fileCounter = 0;
            while (bfsQ.Count > 0)
            {
                cur_node = bfsQ.Dequeue();
                var subdirs = FastFileOperations.GetDirectoriesAndFiles(cur_node.Value.Path);
                foreach (var sd in subdirs)
                {
                    var child = new NNode<PathInfo>(sd);
                    bfsQ.Enqueue(child);
                    cur_node.AddChild(child);
                    dirCounter++;
                }
            }
            var dirTree = new NTree<PathInfo>(root);
            FileCount = fileCounter;
            DirectoryCount = dirCounter;
            return dirTree;
        }

        public static NTree<PathInfo> GetDirectoryTree(string path)
        {
            ulong a, b;
            return GetDirectoryTree(path, out a, out b);
        }
        #endregion

        #region Helper

        public static string HumanReadableSize(ulong p)
        {
            ulong b = p % (1 << 10);
            ulong kb = (p >> 10) % (1 << 10);
            ulong mb = (p >> 20) % (1 << 10);
            ulong gb = (p >> 30) % (1 << 10);
            return String.Format("{0} GB and {1} MB and {2} KB and {3} Bytes", gb, mb, kb, b);
        }

        #endregion
    }
}
