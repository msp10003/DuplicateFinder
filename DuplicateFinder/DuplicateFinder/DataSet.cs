/*Class: DataSet
 * Abstraction of row/spreadsheet data, provides API methods to be used for data access 
 * */

using System;
using System.Linq;
using System.Collections.Generic;


namespace DuplicateFinder
{
    class DataSet
    {
        private DataRetriever data;
        private SortedDictionary<String , Record> lastNameTree, reverseNameTree;
        private RowMapper mapper;
        private StringComparer simComp;
        private SortedDictionary<String, Record>.Enumerator dictEnum;
        private int numRecords;
        private UnionFind unionFind;
        private List<Cluster> clusters;
        private int enumCount;

        //TODO !!! see if i can reduce how many times we take the rows of data and iterate through them
        public DataSet(DataRetriever dataRetriever)
        {
            data = dataRetriever;
            simComp = new StringComparer();
            lastNameTree = new SortedDictionary<String, Record>();
            reverseNameTree = new SortedDictionary<String, Record>();
            mapper = new RowMapper();
            ICollection<Record> records = mapper.mapRows(data);
            //TODO ensure that the number of records is truly reflective of how many "RECORDS" we have, not counting first
            numRecords = records.Count;
            unionFind = new UnionFind(numRecords, data.getNumRowsOffset());
            clusters = new List<Cluster>();
            //dictEnum = new SortedDictionary<string, Record>.Enumerator();
            //fill the trees, which will sort them fill the union-find structure
            foreach(Record r in records)
            {
                Console.SetBufferSize(500, 600);
                lastNameTree.Add(r.getKey(), r);
                reverseNameTree.Add(r.getReverseKey(), r);
                Cluster cluster = new Cluster(r);
                r.setCluster(cluster);
                clusters.Add(cluster);
                unionFind.initialInsert(r);
            }
            //TODO reverse sort the list
            initEnum();            
        }

        private void initEnum()
        {
            dictEnum = lastNameTree.GetEnumerator();
            dictEnum.MoveNext();
        }

        public Record getCurrent()
        {
            return dictEnum.Current.Value;
        }

        public Record getNext()
        {
            dictEnum.MoveNext();
            return dictEnum.Current.Value;
        }

        public List<Cluster> getClusters()
        {
            return clusters;
        }

        public void addCluster(Cluster c)
        {
            clusters.Add(c);
        }
        
        //TODO find a way to do this without this brute force way
        public List<Record> getRows()
        {
            return lastNameTree.Values.ToList();
        }

        public List<Record> getReverseRows()
        {
            return reverseNameTree.Values.ToList();
        }

        public void associateRecords(Record r1, Record r2)
        {
            unionFind.union(r1, r2);
        }

        public bool recordsAssociated(Record r1, Record r2)
        {
            return unionFind.connected(r1, r2);
        }

        public bool MoveNext()
        {
            return dictEnum.MoveNext();
        }
        
        //TODO change wording
        public void mergeClusters(Cluster existingCluster, Cluster newCluster)
        {
            Cluster oldCluster = existingCluster;
            existingCluster.merge(newCluster);
            clusters.Remove(newCluster);
        }

        public override string ToString()
        {
            Record r = getCurrent();
            string s = "";
            while (MoveNext())
            {
                s = s + getCurrent().ToKeyString() + "\n";
            }
            initEnum();
            return s;
        }

    }
}
