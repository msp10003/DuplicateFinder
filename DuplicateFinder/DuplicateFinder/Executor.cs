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
            DataRetriever dataRetriever = new DataRetriever("C:\\Users\\mpierce\\Duplicate Project\\Sample2.xlsx", "C", null, "I", "J");
            DataSet data = new DataSet(dataRetriever);
            DuplicatePruner pruner = new DuplicatePruner(data);

            pruner.prune(0.6, 10, data.getRows());
            pruner.prune(0.6, 10, data.getReverseRows());
            List<Cluster> clusters = data.getClusters();

            //Console.Read();
        }
    }
}
