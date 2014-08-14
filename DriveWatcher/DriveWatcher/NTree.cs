using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTree
{
    /// <summary>
    /// A queue based n-ary tree structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NTree<T>
    {
        public NNode<T> Root { get; private set; }

        #region ctor
        public NTree(NNode<T> root)
        {
            Root = root;
        } 
        public NTree(T rootValue)
        {
            throw new NotImplementedException();
        }
        
        #endregion

        public void AddNode(NNode<T> node, T value)
        {
            node = new NNode<T>(value, new ObservableQueue<NNode<T>>());
        }

        public IEnumerable<object> BFS (Func<NNode<T>,object> func, NNode<T> startNode)
        {
            Queue<NNode<T>> bfsQ = new Queue<NNode<T>>();
            bfsQ.Enqueue(Root);
            while (bfsQ.Count != 0)
            {
                var curr = bfsQ.Dequeue();
                yield return func(curr);
                foreach (var n in curr.Children)
                    bfsQ.Enqueue(n);
            }
        }

        public IEnumerable<object> BFS(Func<NNode<T>, object> func) { yield return BFS(func, this.Root); }

    }
}
