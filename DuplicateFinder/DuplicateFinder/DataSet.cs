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
        SimilarityComparer simComp;
        SortedDictionary<String, Record>.Enumerator dictEnum;

        public DataSet(DataRetriever dataRetriever)
        {
            data = dataRetriever;
            simComp = new SimilarityComparer();
            lastNameTree = new SortedDictionary<String, Record>();
            mapper = new RowMapper();
            ICollection<Record> records = mapper.mapRows(data);
            foreach(Record r in records)
            {
                Console.SetBufferSize(500, 600);
                lastNameTree.Add(r.getKey(), r);
            }
            initEnum();
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
