using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTree
{
    class NNode<T> where T : object
    {
        public NNode(T value = default(T), ObservableCollection<NNode<T>> children = null)
        {
            Value = value;
            Children = children;
        }

        public T Value { get; set; }
        public ObservableCollection<NNode<T>> Children { get; set; }

        public NNode<T> Parent { get; private set; }
        #region Shorthand readonly properties
        public int NumberofChildren { get { return Children.Count(); } }
        public bool HasChildren { get { return NumberofChildren != 0; } }
        public bool IsNull { get { return Value == null; } }
        #endregion
    }
}
