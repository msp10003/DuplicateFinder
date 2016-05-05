/*Class: DataSet
 * Abstraction of row/spreadsheet data, provides API methods to be used for data access 
 * */

using System;
using System.Collections.Generic;
namespace DuplicateFinder
{
    class DataSet
    {
        private DataRetriever data;
        private SortedDictionary<String , Record> lastNameTree;
        private RowMapper mapper;
        private StringComparer simComp;
        private SortedDictionary<String, Record>.Enumerator dictEnum;
        private int numRecords;
        private UnionFind unionFind;
        private List<Cluster> clusters;

        public DataSet(DataRetriever dataRetriever)
        {
            data = dataRetriever;
            simComp = new StringComparer();
            lastNameTree = new SortedDictionary<String, Record>();
            mapper = new RowMapper();
            ICollection<Record> records = mapper.mapRows(data);
            //TODO ensure that the number of records is truly reflective of how many "RECORDS" we have, not counting first
            numRecords = records.Count;
            unionFind = new UnionFind(numRecords, data.getNumRowsOffset());
            clusters = new List<Cluster>();

            //fill the trees, fill the union-find structure
            foreach(Record r in records)
            {
                Console.SetBufferSize(500, 600);
                lastNameTree.Add(r.getKey(), r);
                Cluster cluster = new Cluster(r);
                r.setCluster(cluster);
                clusters.Add(cluster);
                unionFind.initialInsert(r);
            }

            initEnum();

            for(int i=0; i<100; i++)
            {
                MoveNext();
            }
            Record r1 = getCurrent();
            Record r2 = getNext();
            Record r3 = getNext();

            unionFind.union(r1, r2);
            Cluster oldCluster = r2.getCluster();
            r1.getCluster().merge(r2.getCluster());
            clusters.Remove(oldCluster);


            //Testing purposes
            foreach (Cluster c in clusters)
            {
                Console.Out.Write(c.ToString());
            }

            
        }

        private void initEnum()
        {
            dictEnum = lastNameTree.GetEnumerator();
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
        
        public bool MoveNext()
        {
            return dictEnum.MoveNext();
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
