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
        /// <param name="tolerance">pre-determined level of acceptance, for use with auto search</param>
        /// <param name="pqSize">size of the priority queue to be used for storing clusters</param>
        public void prune(double tolerance, int pqSize, List<Record> sortedTree, bool scanDates, bool scanDescriptions, double namePrecision, double datePrecision, double descriptionPrecision, bool autoSearch)
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
                        continue;       //if yes, move on to the next record
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
                            bool inCluster;

                            //if Auto Search is on, do one Compare method, if it's not, do the other one
                            if (autoSearch)
                            {
                                inCluster = compareRecordToCluster(current, cluster, tolerance, node, scanDates, scanDescriptions);
                            }else{
                                inCluster = compareRecordToCluster(current, cluster, tolerance, node, scanDates, scanDescriptions, namePrecision, datePrecision, descriptionPrecision);
                            }

                            if (inCluster)
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
        private bool compareRecordToCluster(Record queryRecord, Cluster cluster, double tolerance, ListPQNode<Cluster> node, bool scanDates, bool scanDescriptions, double namePrecision, double datePrecision, double descriptionPrecision)
        {
            bool result = false;
            foreach(Record clusterRecord in cluster.getRecords())
            {   //check if record is similar enough to record in cluster to be added

                //TODO change logic so that it only calculates all three measures if search enhance is on
                bool similarityFail = false;
                double totalSimilarity = 0;
                int divisor = 1;

                //first perform mandatory name check
                double nameSimilarity = strComp.jaroWinklerCompare(queryRecord, clusterRecord);
                if (nameSimilarity < namePrecision) similarityFail = true;

                double dateSimilarity = compareDates(queryRecord, clusterRecord);
                if (scanDates && (dateSimilarity < datePrecision)){
                    similarityFail = true;
                    divisor++;
                }

                double descriptionSimilarity = compareDescriptions(queryRecord, clusterRecord);
                if (scanDescriptions && (descriptionSimilarity < descriptionPrecision)){
                    similarityFail = true;
                    divisor++;
                }

                //calculate total similairty
                //TODO smarter weighting
                totalSimilarity = (nameSimilarity + dateSimilarity + descriptionPrecision)/divisor;


                //if all three similarity checks succeeded, it's a match
                if (!similarityFail)
                {   //if yes, update the cluster
                    addRecordToCluster(queryRecord, cluster);
                    updatePQ(node);         //and update the priority queue
                    result = true;
                    break; 
                }
                else if(totalSimilarity < 0.4)    //if the similarity is way off, don't bother checking the rest of the cluster
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Checks whether record is in cluster, using pre-defined tolerance
        /// </summary>
        private bool compareRecordToCluster(Record queryRecord, Cluster cluster, double tolerance, ListPQNode<Cluster> node, bool scanDates, bool scanDescriptions)
        {
            bool result = false;
            foreach (Record r in cluster.getRecords())
            {   //check if record is similar enough to record in cluster to be added

                //get initial similarity measure based on name
                double similarity = strComp.jaroWinklerCompare(queryRecord, r);

                //if the similarity is close to the tolerance, check the date
                if (scanDates && (similarity < (tolerance + (tolerance * 0.07)))
                    && similarity > tolerance - (tolerance * 0.07))
                {
                    similarity = compareDates(queryRecord, r, similarity);
                }

                //If still indecisive, check description
                if (scanDescriptions && (similarity < (tolerance + (tolerance * 0.05)))    
                    && similarity > tolerance - (tolerance * 0.05))
                {
                    if (queryRecord.getDescription() == null || r.getDescription() == null)
                    {
                        //deduct for not having a description, but don't kill it since it's likely it was simply forgotten
                        similarity = similarity - (similarity * 0.1);
                    }
                    else
                    {
                        double descSimilarity = strComp.nGramCompareDesc(queryRecord, r);
                        if (descSimilarity > tolerance)
                        {
                            similarity = similarity + (similarity * 0.08);
                        }
                        else if (descSimilarity < (0.5) * tolerance)
                        {
                            similarity = similarity - (similarity * 0.15);
                        }
                    }
                }
            
                if (similarity >= tolerance)
                {   //if yes, update the cluster
                    addRecordToCluster(queryRecord, cluster);
                    updatePQ(node);         //and update the priority queue
                    result = true;
                    break;
                }
                else if (similarity < 0.5)    //if the similarity is way off, don't bother checking the rest of the cluster
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Compares similarity between the dates of the records in question
        /// </summary>
        private double compareDates(Record queryRecord, Record clusterRecord)
        {
            //TODO if precision is max, we want to disqualify anything that's not an exact match, rather than simply docking points for it
            DateTime d1 = queryRecord.getDate();
            DateTime d2 = clusterRecord.getDate();
            if(d1.Equals(new DateTime(1900,1,1)) || d2.Equals(new DateTime(1900, 1, 1))){
                //one of the records didn't have a date, disregard this comparison
                return 0;
            }
            //ensure that Excel didn't do a dumb thing where the date was off by 2000 years
            if(d1.Year < 10)
            {
                d1.AddYears(2000);
            }
            if (d2.Year < 10)
            {
                d2.AddYears(2000);
            }

            double dateDiff = Math.Abs((d1 - d2).TotalDays);

            //TODO: scale the similarity based on a bell curve, with close dates yielding high and futher dates disproportionately less
            return (dateDiff / (-120) + 1);
        }

        /// <summary>
        /// Compares similarity alteration based on date. To be used with auto-search
        /// </summary>
        private double compareDates(Record queryRecord, Record r1, double similarity)
        {
            DateTime d1 = queryRecord.getDate();
            DateTime d2 = r1.getDate();
            if (d1.Equals(new DateTime(1900, 1, 1)) || d2.Equals(new DateTime(1900, 1, 1)))
            {
                //one of the records didn't have a date, disregard this comparison
                return similarity;
            }
            if (d1.Year < 10)
            {
                d1.AddYears(2000);
            }

            if (d2.Year < 10)
            {
                d2.AddYears(2000);
            }
            double dateDiff = Math.Abs((d1 - d2).TotalDays);
            if (dateDiff == 0)
            {
                similarity = similarity + (similarity * 0.2);
            }
            else if (dateDiff <= 2)
            {
                similarity = similarity + (similarity * 0.1);
            }
            else if (dateDiff >= 14)
            {
                similarity = similarity - (similarity * 0.1);
            }
            return similarity;
        }

        /// <summary>
        /// Calculates similarity between descriptions
        /// </summary>
        private double compareDescriptions(Record queryRecord, Record r)
        {
            double descSimilarity = strComp.nGramCompareDesc(queryRecord, r);
            return descSimilarity;
        }
        
        /// <summary>
        /// Adds the record to the given cluster and updates the underlying clusters collection
        /// </summary>
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
