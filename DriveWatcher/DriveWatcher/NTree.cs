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
        public NNode<T> Root { get; set; }

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



        public IEnumerable<object> BFS(Func<NNode<T>, object> func, NNode<T> startNode)
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

        public IEnumerable<object> BFS(Func<NNode<T>, object> func) { return BFS(func, this.Root); }
        public IEnumerable<object> DFS(Func<NNode<T>, object> func, VisitingOrder visitingOrder) { return DFS(func, this.Root, visitingOrder); }

        public IEnumerable<object> DFS(Func<NNode<T>, object> func, NNode<T> startNode, VisitingOrder visitingOrder)
        {
            Stack<NNode<T>> dfsStack = new Stack<NNode<T>>();
            var visitedList = new HashSet<NNode<T>>();
            var curr = Root;
            while (curr != null)
            {
                if (visitingOrder == VisitingOrder.Pre)
                {
                    if (!visitedList.Contains(curr))
                    {
                        yield return func(curr);
                        visitedList.Add(curr);
                    }
                }

                var next = curr.Children.FirstOrDefault(p => !visitedList.Contains(p));
                if (next == null)
                {
                    if (visitingOrder == VisitingOrder.Post)
                    {
                        if (!visitedList.Contains(curr))
                        {
                            yield return func(curr);
                            visitedList.Add(curr);
                        }
                    }
                    if (dfsStack.Count != 0)
                        next = dfsStack.Pop();
                }
                else
                    dfsStack.Push(curr);
                curr = next;
            }
        }
       

    }
}
