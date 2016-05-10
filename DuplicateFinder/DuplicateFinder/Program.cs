using System;
using System.Collections.Generic;
using SpreadsheetLight;

namespace DuplicateFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            DataRetriever dataRetriever = new DataRetriever("C:\\Users\\Matthew\\Documents\\Duplicate Project\\Sample4.xlsx", "C", null, "I", "J");
            DataSet data = new DataSet(dataRetriever);
            DuplicatePruner pruner = new DuplicatePruner(data);

            pruner.prune(0.6, 10);
            List<Cluster> clusters = data.getClusters();
            
            Console.Read();
        }
    }
}
