using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTree
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class NNode<T>
    {
        #region ctor
        public NNode(T value = default(T))
        {
            Value = value;
            _children = new List<NNode<T>>();
            //UnvisitedChildren = new List<NNode<T>>();
            Nodes = 0;
        }
        #endregion

        #region Properites
        public long Nodes { get; set; }
        public T Value { get; set; }
        public NNode<T> Parent { get; private set; }
        private List<NNode<T>> _children;

        #endregion


        #region Shorthand readonly properties        
        public int NumberOfChildren { get { return _children.Count(); } }
        public bool HasChildren { get { return NumberOfChildren != 0; } }
        public bool IsNull { get { return Value == null; } }
        public IList<NNode<T>> Children { get { return _children.AsReadOnly(); } }

        #endregion

        #region Basic operations
        public void AddChild(NNode<T> child)
        {
            child.Parent = this;
            _children.Add(child);
            Nodes++;
        }
        public void RemoveChild(NNode<T> child)
        {
            child.Parent = null;
            _children.Remove(child);
            Nodes--;
        }

        #endregion
    }
}
