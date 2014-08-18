using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTree
{
    public class ObservableQueue <T> : ObservableCollection<T>
    {
        public void Enqueue(T item)
        {
            this.Add(item);
        }
        public T Dequeue()
        {
            return this.First();
        }
    }
}
