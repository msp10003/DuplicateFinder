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
            DataRetriever dataRetriever = new DataRetriever("C:\\Users\\Matthew\\Documents\\Duplicate Project\\List 2.xlsx", "B", null, "D", "H");
            DataSet data = new DataSet(dataRetriever);
            DuplicatePruner pruner = new DuplicatePruner(data);

            pruner.prune(0.90, 20, data.getRows());
            List<Cluster> clusters1 = data.getClusters();
            pruner.prune(0.90, 20, data.getReverseRows());
            List<Cluster> clusters2 = data.getClusters();

            dataRetriever.copySpreadsheetToFile("C:\\Users\\Matthew\\Documents\\Duplicate Project\\TestOutput1.xlsx");
            //Console.Read();
        }
    }
}
