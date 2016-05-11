using System;
using System.Collections.Generic;
using SpreadsheetLight;

namespace DuplicateFinder
{
    public class Executor
    {
        static void Main(string[] args)
        {

        }

        public static void execute()
        {
            DataRetriever dataRetriever = new DataRetriever("C:\\Users\\Matthew\\Documents\\Duplicate Project\\Sample4.xlsx", "C", null, "I", "J");
            DataSet data = new DataSet(dataRetriever);
            DuplicatePruner pruner = new DuplicatePruner(data);

            pruner.prune(0.6, 10, data.getRows());
            List<Cluster> clusters1 = data.getClusters();
            pruner.prune(0.6, 10, data.getReverseRows());
            List<Cluster> clusters2 = data.getClusters();

            //Console.Read();
        }
    }
}
