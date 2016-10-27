using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    class DuplicatePruner
    {
        private DataSet data;
        private StringComparer strComp;
        private ListPQ<Cluster> listPQ;
        public const int MAX_DAYS = 30;
        public const double MIN_DESCRIPTION_SIM = 0.50;
        public const double MAX_DESCRIPTION_SIM = 0.90;
        public const double MIN_NAME_SIM = 0.76;
        public const int TOLERANCE_DISCARD_FACTOR = 2;
       
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
        public void prune(double tolerance, int pqSize, List<Record> sortedTree, bool scanDates, bool scanDescriptions, double namePrecision, double datePrecision, double descriptionPrecision, bool autoSearch, List<String> ignoreList)
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
                                inCluster = compareRecordToClusterAuto(current, cluster, tolerance, node, scanDates, scanDescriptions, ignoreList);
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
        private bool compareRecordToClusterAuto(Record queryRecord, Cluster cluster, double tolerance, ListPQNode<Cluster> node, bool scanDates, bool scanDescriptions, List<String> ignoreList)
        {
            bool result = false;
            
            foreach (Record r in cluster.getRecords())
            {   //check if record is similar enough to record in cluster to be added
                double nameWeight = 0; double dateWeight = 0; double descriptionWeight = 0; //will be used to determine how much weight to give to each field
                double nameSimilarity = 0; double dateSimilarity = 0; double descriptionSimilarity = 0;

                nameSimilarity = normalize(MIN_NAME_SIM, 1, strComp.jaroWinklerCompare(queryRecord, r));
                //account for cases where one or both of the records are missing a date - ignore the field in calculation
                if (queryRecord.getDate().Equals(new DateTime(1900,1,1)) || r.getDate().Equals(new DateTime(1900,1,1)))
                {
                    scanDates = false;
                }
                else
                {
                    dateSimilarity = normalize(0, MAX_DAYS, (MAX_DAYS - compareDates(queryRecord, r)));
                }
                
                //do the same for descriptions
                if ((scanDescriptions == false) || IgnoreDescriptions(queryRecord,r,ignoreList))
                {
                    scanDescriptions = false;
                }
                else
                {
                    descriptionSimilarity = normalize(MIN_DESCRIPTION_SIM, MAX_DESCRIPTION_SIM, compareDescriptions(queryRecord, r));
                }

                calculateWeights(ref nameWeight,ref dateWeight,ref descriptionWeight, scanDates, scanDescriptions);

                double similarity = (nameSimilarity * nameWeight) + (dateSimilarity * dateWeight) + (descriptionSimilarity * descriptionWeight);
            
                if (similarity >= tolerance)
                {   //if yes, update the cluster
                    addRecordToCluster(queryRecord, cluster);
                    updatePQ(node);         //and update the priority queue
                    result = true;
                    break;
                }
                else if (similarity < tolerance/TOLERANCE_DISCARD_FACTOR)    //if the similarity is way off, don't bother checking the rest of the cluster
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Normalizes a a number in a range between 0 and 1.
        /// </summary>
        private double normalize(double min, double max, double x)
        {
            //drop things that fall below the desired range
            if (x < min) 
            {
                return 0;
            }
            //if anything is higher than our threshold, just give full credit
            if (x > max)
            {
                return 1;
            }

            return ((x-min) / (max - min));
        }

        /// <summary>
        /// Caluclates relative weights for the total similarity
        /// </summary>
        private void calculateWeights(ref double nameWeight,ref double dateWeight,ref double descriptionWeight, bool scanDates, bool scanDescriptions)
        {
            if (!scanDates && scanDescriptions)
            {
                descriptionWeight = 0.3;
            }
            else if (!scanDescriptions && scanDates)
            {
                dateWeight = 0.3;
            }
            else if (!scanDates && !scanDescriptions)
            {
                dateWeight = 0;
                descriptionWeight = 0;
            }
            else
            {
                dateWeight = 0.2;
                descriptionWeight = 0.2;
            }
            nameWeight = 1 - (dateWeight + descriptionWeight);

            return;
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
            return dateDiff;
        }

        /// <summary>
        /// Compares similarity alteration based on date. To be used with auto-search. Keeps similarity normalized between 0 and 1.
        /// </summary>
        private double updateSimilarityFromDates(Record queryRecord, Record r1, double similarity)
        {
            double dateSimilarity = compareDates(queryRecord, r1);
            //add or deduct from the similarity using the factor defined by the line y = 2x -1
            double scaleFactor = (2 * dateSimilarity - 1);
            if (scaleFactor >= 0)
            {
                similarity = similarity + ((1 - 0.999 * similarity) * scaleFactor);
            }
            else
            {
                similarity = similarity + (similarity) * scaleFactor/2;
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

        /// <summary>
        /// Checks if the descriptions are either empty or contain something that should be ignored
        /// </summary>
        private bool IgnoreDescriptions(Record queryRecord, Record r, List<String> ignoreList){

            if (String.IsNullOrEmpty(queryRecord.getDescription()) || String.IsNullOrEmpty(r.getDescription()))
            {
                return true;
            }

            foreach (String s in ignoreList)
            {
                if (queryRecord.getDescription() == s)
                {
                    return true;
                }
                else if (r.getDescription() == s)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
