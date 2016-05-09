using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    class DuplicatePruner
    {
        DataSet data;
        StringComparer strComp;
        ListPQ<Cluster> listPQ;

        public DuplicatePruner(DataSet d)
        {
            data = d;
            strComp = new StringComparer();
        }

        public void prune(double tolerance, int windowSize, int pqSize)
        {
            listPQ = new ListPQ<Cluster>(pqSize);

            //row-by-row traversal of data set
            while (true)
            {
                Record current = data.getCurrent();

                //first check if the record's cluster is already on the PQ
                if (searchPQ(current))
                {
                    continue;       //if  yes, move on to the next record
                }
                else
                {

                }

                if (data.MoveNext() == false) break;
            }


        }

        private void compareRecords(Record r1, Record r2)
        {
            Cluster r1Clust = r1.getCluster();
            Cluster r2Clust = r2.getCluster();


        }

        /// <summary>
        /// Searches through the PQ for a matching cluster, if found updates the PQ
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private bool searchPQ(Record r)
        {
            foreach (ListPQNode<Cluster> n in listPQ) {
                if(r.getCluster() == n.getValue())
                {
                    listPQ.setMax(n);
                    return true;
                }
            }
            return false;
        }
    }
}
