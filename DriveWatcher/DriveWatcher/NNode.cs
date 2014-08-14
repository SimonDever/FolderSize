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
        public NNode(T value = default(T))
        {
            Value = value;
            Children = new ObservableCollection<NNode<T>>();
            Children.CollectionChanged += Children_CollectionChanged;
        }

        void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var i in e.NewItems)
                (i as NNode<T>).Parent = this;
        }

        public T Value { get; set; }
        public ObservableCollection<NNode<T>> Children { get; private set; }

        public NNode<T> Parent { get; private set; }
        #region Shorthand readonly properties
        public int NumberofChildren { get { return Children.Count(); } }
        public bool HasChildren { get { return NumberofChildren != 0; } }
        public bool IsNull { get { return Value == null; } }
        #endregion
    }
}
