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

        /// <summary>
        /// Main kickoff method for the duplicate matching algorithm
        /// </summary>
        /// <param name="tolerance"></param>
        /// <param name="pqSize"></param>
        public void prune(double tolerance, int pqSize)
        {
            listPQ = new ListPQ<Cluster>(pqSize);
            //TODO figure out how to do this with an enumerator
            List<Record> records = data.getRows();

            //row-by-row traversal of data set
            foreach(Record current in records)
            {
                //Record current = data.getCurrent();
                bool inPQ = false;

                //first check if the record's cluster is already on the PQ
                if (searchPQ(current))
                {
                    continue;       //if  yes, move on to the next record
                }
                else
                {   //if PQ is empty, add this cluster
                    if(listPQ.Count() == 0)
                    {
                        listPQ.insertMax(current.getCluster());
                        continue;
                    }
                    
                    //search for membership in each cluster of the PQ
                    foreach(ListPQNode<Cluster> node in listPQ)
                    {
                        Cluster cluster = node.getValue();
                        if (compareRecordToCluster(current, cluster, tolerance, node))
                        {
                            inPQ = true;
                            break;  //if match, stop
                        }
                        //if no match, keep looking
                    }
                    if (inPQ == false)
                    {
                        insertPQ(current.getCluster());
                    }
                }

                //if (data.MoveNext() == false) break;
            }


        }

        /// <summary>
        /// Checks whether a record belongs in a cluster using string comparison
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool compareRecordToCluster(Record queryRecord, Cluster cluster, double tolerance, ListPQNode<Cluster> node)
        {
            bool result = false;
            foreach(Record r in cluster.getRecords())
            {   //check if record is similar enough to record in cluster to be added
                double similarity = strComp.nGramCompare(queryRecord, r);
                if(similarity >= tolerance)
                {   //if yes, update the cluster
                    addRecordToCluster(queryRecord, cluster);
                    updatePQ(node);         //and update the priority queue
                    result = true;
                    break; 
                }
                else if(similarity < 0.2)    //if the similarity is way off, don't bother checking the rest of the cluster
                {
                    break;
                }
            }
            //if we've looked through all the cluster records with no luck, this must belong to its own cluster, so add it to PQ
            //insertPQ(queryRecord.getCluster());

            return result;
        }

        /// <summary>
        /// Adds the record to the given cluster and updates the underlying clusters collection
        /// </summary>
        /// <param name="record"></param>
        /// <param name="cluster"></param>
        private void addRecordToCluster(Record record, Cluster cluster)
        {
            //first update the union-find structure
            data.associateRecords(record, cluster.getRepresentativeElement());
            //then update the clusters
            data.mergeClusters(cluster, record.getCluster());
        }

        /// <summary>
        /// Updates the PQ with the given cluster
        /// </summary>
        /// <param name="node"></param>
        private void updatePQ(ListPQNode<Cluster> node)
        {
            listPQ.setMax(node);
        }

        private void insertPQ(Cluster c)
        {
            listPQ.insertMax(c);
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
