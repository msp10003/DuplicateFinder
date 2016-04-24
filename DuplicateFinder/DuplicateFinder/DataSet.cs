/*Class: DataSet
 * Abstraction of row/spreadsheet data, provides API methods to be used for data access 
 * */

using System;
using System.Collections.Generic;
using GemBox.Spreadsheet;

namespace DuplicateFinder
{
    class DataSet
    {
        private DataRetriever data;
        private SortedDictionary<String , Record> lastNameTree;

        public DataSet(DataRetriever dataRetriever)
        {
            data = dataRetriever;
            lastNameTree = new SortedDictionary<string, Record>();
        }

        private void initializeDataSet()
        {

        }


    }
}
