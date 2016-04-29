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

        public DataSet(DataRetriever dataRetriever)
        {
            data = dataRetriever;
            lastNameTree = new SortedDictionary<String, Record>();
            mapper = new RowMapper();
            ICollection<Record> records = mapper.mapRows(data);
            foreach(Record r in records)
            {
                Console.SetBufferSize(500, 600);
                System.Console.Out.Write(r.ToString()+"\n");
                //lastNameTree.Add(r.getFullName(), r);
            }
        }
        
    }
}
