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
    [Serializable]
    public class NTree<T>
    {

        #region Public properties
        public NNode<T> Root { get; set; }
        public long Nodes { get { return Root.Nodes; } }
        
        #endregion

        #region ctor
        public NTree()
        {
        }
        public NTree(NNode<T> root)
        {
            Root = root;
        }
        public NTree(T rootValue)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region BFS and DFS
        public void BFS(Func<NNode<T>, object> func, NNode<T> startNode)
        {
            Queue<NNode<T>> bfsQ = new Queue<NNode<T>>();
            bfsQ.Enqueue(Root);
            while (bfsQ.Count != 0)
            {
                var curr = bfsQ.Dequeue();
                func(curr);
                foreach (var n in curr.Children)
                    bfsQ.Enqueue(n);
            }
        }

        public void BFS(Func<NNode<T>, object> func) { BFS(func, this.Root); }
        public void DFS(Func<NNode<T>, object> func, VisitingOrder visitingOrder) { DFS(func, this.Root, visitingOrder); }

        public void DFS(Func<NNode<T>, object> func, NNode<T> startNode, VisitingOrder visitingOrder)
        {
            HashSet <NNode<T>> visitedNodes = new HashSet<NNode<T>>();
            Stack<NNode<T>> dfsStack = new Stack<NNode<T>>();
            var curr = Root;            
            while (curr != null)
            {
                
                //if (visitingOrder == VisitingOrder.Pre)
                //{
                //    if (!curr.Visited)
                //    {
                //        func(curr);
                //        curr.Visited = true;
                //    }
                //}

                NNode<T> next = (curr.Children.Count == 0) ? null : curr.Children.FirstOrDefault(p => !visitedNodes.Contains(p));                
                
                if (next == null)
                {
                    if (visitingOrder == VisitingOrder.Post)
                    {
                        func(curr);
                        visitedNodes.Add(curr);
                    }
                    if (dfsStack.Count != 0)
                        next = dfsStack.Pop();
                }
                else
                    dfsStack.Push(curr);
                curr = next;
            }
        }
        #endregion

        #region Basic Operations
        public void RemoveNode(NNode<T> node)
        {
            if (node == Root)
                this.Root = null;
            else
                node.Parent.RemoveChild(node);
        }
        #endregion
    }
}
