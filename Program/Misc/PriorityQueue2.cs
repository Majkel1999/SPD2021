using System;
using System.Collections.Generic;

namespace SPD1.Misc
{
    public class PriorityQueue<T>
    {
        class Node
        {
            public int PriorityOfObject { get; set; }
            public T Object { get; set; }
        }

        int size = -1;
        List<Node> queue = new List<Node>();
        bool _isMin;
        public int Count => queue.Count; 

        /// Określenie czy kolejka będzie od najmniejszych czy od największych danych
        public PriorityQueue(bool isMin = false)
        {
            _isMin = isMin;
        }

        private void Swap(int i, int j)
        {
            var tmp = queue[i];
            queue[i] = queue[j];
            queue[j] = tmp;
        }
        private int ChildLeft(int i)
        {
            return i * 2 + 1;
        }
        private int ChildRight(int i)
        {
            return i * 2 + 2;
        }

        private void MaxHeap(int i)
        {
            int left = ChildLeft(i);
            int right = ChildRight(i);

            int highest = i;

            if (left <= size && queue[highest].PriorityOfObject < queue[left].PriorityOfObject)
                highest = left;
            if (right <= size && queue[highest].PriorityOfObject < queue[right].PriorityOfObject)
                highest = right;

            if (highest != i)
            {
                Swap(highest, i);
                MaxHeap(highest);
            }
        }
        private void MinHeap(int i)
        {
            int left = ChildLeft(i);
            int right = ChildRight(i);

            int lowest = i;

            if (left <= size && queue[lowest].PriorityOfObject > queue[left].PriorityOfObject)
                lowest = left;
            if (right <= size && queue[lowest].PriorityOfObject > queue[right].PriorityOfObject)
                lowest = right;

            if (lowest != i)
            {
                Swap(lowest, i);
                MinHeap(lowest);
            }
        }

        private void BuildMaxHeap(int i)
        {
            while (i >= 0 && queue[(i - 1) / 2].PriorityOfObject < queue[i].PriorityOfObject)
            {
                Swap(i, (i - 1) / 2);
                i = (i - 1) / 2;
            }
        }
        private void BuildMinHeap(int i)
        {
            while (i >= 0 && queue[(i - 1) / 2].PriorityOfObject > queue[i].PriorityOfObject)
            {
                Swap(i, (i - 1) / 2);
                i = (i - 1) / 2;
            }
        }

        public void Enqueue(int priority, T obj)
        {
            Node node = new Node() { PriorityOfObject = priority, Object = obj };
            queue.Add(node);
            size++;
            if (_isMin)
                BuildMinHeap(size);
            else
                BuildMaxHeap(size);
        }

        public T Dequeue()
        {
            if (size > -1)
            {
                var returnVal = queue[0].Object;
                queue[0] = queue[size];
                queue.RemoveAt(size);
                size--;
                
                if (_isMin)
                    MinHeap(0); //od najmniejszego do największego
                else
                    MaxHeap(0); //od największego do najmniejszego
                return returnVal;
            }
            else
                throw new Exception("Pusta kolejka");
        }

        public T GetValueAtZero()
        {
            if (size > -1)
            {
                var val = queue[0].Object;
                return val;
            }
            else
                throw new Exception("PUsta kolejka");
        }

        public void UpdatePriority(T obj, int priority)
        {
            int i = 0;
            for (; i <= size; i++)
            {
                Node node = queue[i];
                if (object.ReferenceEquals(node.Object, obj))
                {
                    node.PriorityOfObject = priority;
                    if (_isMin)
                    {
                        BuildMinHeap(i);
                        MinHeap(i);
                    }
                    else
                    {
                        BuildMaxHeap(i);
                        MaxHeap(i);
                    }
                }
            }
        }

        public bool IsInQueue(T obj)
        {
            foreach (Node node in queue)
                if (object.ReferenceEquals(node.Object, obj))
                    return true;
            return false;
        }
    }
}
