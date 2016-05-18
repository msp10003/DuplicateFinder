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
        public void prune(double tolerance, int pqSize, List<Record> sortedTree)
        {
            try
            {
                listPQ = new ListPQ<Cluster>(pqSize);
                //TODO figure out how to do this with an enumerator
                //List<Record> records = data.getRows();
                List<Record> records = sortedTree;

                //row-by-row traversal of data set
                foreach (Record current in records)
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
                        if (listPQ.Count() == 0)
                        {
                            listPQ.insertMax(current.getCluster());
                            continue;
                        }

                        //search for membership in each cluster of the PQ
                        foreach (ListPQNode<Cluster> node in listPQ)
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
            catch
            {
                throw (new Exception("Scanning Failed"));
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

                if (queryRecord.getID() == 120 && r.getID() == 119)
                {
                    int x = 0;
                }
                double similarity = strComp.jaroWinklerCompare(queryRecord, r);   //get initial similarity measure

                //calculateDeductions(queryRecord, r, similarity);            //apply deductions

                if(similarity < (tolerance+(tolerance*0.07))                //if the similarity is close around the tolerance, check the date
                    && similarity > tolerance - (tolerance * 0.07))
                {
                    similarity = compareDates(queryRecord, r, similarity);
                }

                if (similarity < (tolerance + (tolerance * 0.05))            //if the similarity is still indecisive, check the description as a last resort
                    && similarity > tolerance - (tolerance * 0.05))
                {
                    double descSimilarity = strComp.nGramCompareDesc(queryRecord, r);
                    if(descSimilarity > tolerance)
                    {
                        similarity = similarity + (similarity * 0.08);
                    }
                    else if(descSimilarity < (0.5) * tolerance)
                    {
                        similarity = similarity - (similarity * 0.15);
                    }
                }

                if (similarity >= tolerance)
                {   //if yes, update the cluster
                    addRecordToCluster(queryRecord, cluster);
                    updatePQ(node);         //and update the priority queue
                    result = true;
                    break; 
                }
                else if(similarity < 0.5)    //if the similarity is way off, don't bother checking the rest of the cluster
                {
                    break;
                }
            }
            //if we've looked through all the cluster records with no luck, this must belong to its own cluster, so add it to PQ
            //insertPQ(queryRecord.getCluster());

            return result;
        }

        /// <summary>
        /// Takes into account various similarity deductions based on corner cases
        /// </summary>
        /// <param name="queryRecord"></param>
        /// <param name="r"></param>
        /// <param name="similarity"></param>
        private void calculateDeductions(Record queryRecord, Record r, double similarity)
        {
            //TODO this section is hideous
            double firstNameDiff = 0.0;
            double lastNameDiff = 0.0;
            //if the names differ in length by 3 or more characters, a deduction is in order
            if (!String.IsNullOrEmpty(queryRecord.getFirstName()) && !String.IsNullOrEmpty(r.getFirstName()))
            {
                firstNameDiff = queryRecord.getFirstName().Length - r.getFirstName().Length;
            }
            if (!String.IsNullOrEmpty(queryRecord.getLastName()) && !String.IsNullOrEmpty(r.getLastName()))
            {
                lastNameDiff = queryRecord.getLastName().Length - r.getLastName().Length;
            }
            if (firstNameDiff > 2)
            {
                similarity = similarity - (0.075 * (firstNameDiff - 2) * similarity);
            }
            if (lastNameDiff > 2)
            {
                similarity = similarity - (0.075 * (lastNameDiff - 2) * similarity);
            }

            //put emphasis on the first letter of the last names
            if(queryRecord.getLastName()[0] != r.getLastName()[0])
            {
                similarity = similarity - (0.1 * similarity);
            }
        }

        /// <summary>
        /// Compares similarity between the dates of the records in question
        /// </summary>
        /// <param name="queryRecord"></param>
        /// <param name="r1"></param>
        /// <param name="similarity"></param>
        private double compareDates(Record queryRecord, Record r1, double similarity)
        {
            DateTime d1 = queryRecord.getDate();
            DateTime d2 = r1.getDate();
            if(d1.Equals(new DateTime(1900,1,1)) || d2.Equals(new DateTime(1900, 1, 1))){
                //one of the records didn't have a date, disregard this comparison
                return similarity;
            }
            if(d1.Year < 10)
            {
                d1.AddYears(2000);
            }

            if (d2.Year < 10)
            {
                d2.AddYears(2000);
            }
            double dateDiff = Math.Abs((d1 - d2).TotalDays);
            if(dateDiff == 0)
            {
                similarity = similarity + (similarity * 0.2);
            }
            else if(dateDiff <= 2)
            {
                similarity = similarity + (similarity * 0.1);
            }
            else if(dateDiff >= 14)
            {
                similarity = similarity - (similarity * 0.1);
            }
            return similarity;
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
