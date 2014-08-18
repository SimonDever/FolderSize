using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTree
{
    public class NNode<T>
    {
        public bool Visited = false;
        public NNode(T value = default(T))
        {
            Value = value;
            Children = new List<NNode<T>>();

        }


        public T Value { get; set; }
        public List<NNode<T>> Children { get; private set; }

        public NNode<T> Parent { get; private set; }
        #region Shorthand readonly properties
        public int NumberofChildren { get { return Children.Count(); } }
        public bool HasChildren { get { return NumberofChildren != 0; } }
        public bool IsNull { get { return Value == null; } }
        #endregion
    }
}
