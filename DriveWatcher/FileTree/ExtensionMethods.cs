using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTree
{
    public static class ExtensionMethods
    {    /// <summary>
        /// 
        /// </summary>
        /// <param name="ft"></param>
        /// <param name="IsRerun">To be implemented</param>
        public static void Refresh(this NTree<PathInfo> ft, bool IsRerun = false)
        {
            ft.BFS(o => { o.Nodes = 0; if (!o.Value.IsFile) o.Value.Size = 0; return null; });
            ft.DFS(o =>
            {
                if (o.Value.IsFile && o.Value.Size == 0)
                    o.Value.Size = FastFileOperations.GetFileSize(o.Value.Path);
                if (o.Parent != null)
                {
                    o.Parent.Value.Size += o.Value.Size;
                    o.Parent.Nodes += o.Nodes;
                }
                return null;
            },
            VisitingOrder.Post);
        }

    }
}
