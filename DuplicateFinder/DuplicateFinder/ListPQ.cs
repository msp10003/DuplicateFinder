using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    class ListPQ<Cluster> : IEnumerable<Cluster>
    {
        private ListPQNode<Cluster> head;
        private ListPQNode<Cluster> tail;
        int N, M;

        public ListPQ(int max)
        {
            M = max;
        }

        public void insertMax(Cluster r)
        {
            if (N == 0)
            {
                head = new ListPQNode<Cluster>(r);
                tail = head;
                N++;
                return;
            }
            ListPQNode<Cluster> n = new ListPQNode<Cluster>(r);
            head.next = n;
            n.prev = head;
            head = n;
            N++;

            if (N > M)
            {
                removeMin();
            }
        }

        public void removeMin()
        {
            if (N == 1)
            {
                N--;
                head = null;
                tail = null;
                return;
            }
            tail.next.prev = null;
            tail = tail.next;
            N--;
        }

        public IEnumerator<Cluster> GetEnumerator()
        {
            ListPQNode<Cluster> current = head;
            while(current.prev != null)
            {
                yield return current.prev.getValue();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
