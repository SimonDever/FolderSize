using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileTree
{
    public class SizeChangedEventArgs : EventArgs
    {
        public long SizeChange{ get; set; }
       
    }
}
