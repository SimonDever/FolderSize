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
        public PathInfo(string path, bool isFile)
        {
            Path = path;
            IsFile = isFile;
            Size = (IsFile ? new FileInfo(path).Length : DirSize);
        }
        public string Path { get; private set; }
        public bool IsFile { get; private set; }
        private long _size;

        public long Size
        {
            get { return _size; }
            set
            {
                _size = value;
                var handle = OnSizeChanged;
                if (handle != null)
                {
                    var e = new SizeChangedEventArgs { SizeChange = _size - Size };
                    handle(this, e);
                }
            }
        }

        public event EventHandler<SizeChangedEventArgs> OnSizeChanged;
    }
}
