using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    class UnionFind
    {
        private int N, rowOffset;
        private Record[] idArray;
        private int[] sizeArray;
        
        public UnionFind(int size, int rowOff)
        {
            N = size;
            idArray = new Record[N];
            sizeArray = new int[N];
            rowOffset = rowOff;
        }

        public void initialInsert(Record r)
        {
            idArray[r.getID()-rowOffset] = r;
            sizeArray[r.getID()-rowOffset] = 1;
        }

        public void union(Record r1, Record r2)
        {
            int i = find(r1);
            int j = find(r2);
            if(i == j)
            {
                return;
            }
            else if(sizeArray[i-rowOffset] > sizeArray[j-rowOffset])
            {
                idArray[i-rowOffset] = r2;
                sizeArray[j-rowOffset] += sizeArray[i-rowOffset];
            }
            else
            {
                idArray[j-rowOffset] = r1;
                sizeArray[i-rowOffset] += sizeArray[j-rowOffset];
            }
            N--;
        }

        public bool connected(Record r1, Record r2)
        {
            return find(r1)==find(r2);
        }

        private int find(Record r)
        {
            int p = r.getID();
            while(p != idArray[p-rowOffset].getID())
            {
                p = idArray[p-rowOffset].getID();
            }

            return p;
        }

        public int count()
        {
            return N;
        }
    }
}
