using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    public class ListPQ<Cluster> : IEnumerable<ListPQNode<Cluster>>
    {
        private ListPQNode<Cluster> head;
        private ListPQNode<Cluster> tail;
        int N, M;

        public ListPQ(int max)
        {
            M = max;
        }

        public void insertMax(Cluster c)
        {
            if (N == 0)
            {
                head = new ListPQNode<Cluster>(c);
                tail = head;
                N++;
                return;
            }
            ListPQNode<Cluster> n = new ListPQNode<Cluster>(c);
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

        public void setMax(ListPQNode<Cluster> n)
        {
            if (N == 1 || head == n)
            {
                return;
            }
            else if(tail == n)
            {
                tail = n.next;
                tail.prev = null;
            }
            else
            {
                ListPQNode<Cluster> left = n.prev;
                ListPQNode<Cluster> right = n.next;
                left.next = right;
                right.prev = left;
            }

            //set selected node to max
            head.next = n;
            n.prev = head;
            head = n;
        }

        public Cluster peekTop()
        {
            return head.getValue();
        }

        public Cluster peekBottom()
        {
            return tail.getValue();
        }

        public int Count()
        {
            return N;
        }

        public IEnumerator<ListPQNode<Cluster>> GetEnumerator()
        {
            ListPQNode<Cluster> current = head;
            while(current != null)
            {
                yield return current;
                current = current.prev;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
