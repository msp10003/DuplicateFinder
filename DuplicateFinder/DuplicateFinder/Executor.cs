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

        public static void execute(String inputPath, String outputPath, String nameCol, String IDCol, String dateCol, String descCol)
        {
            DataRetriever dataRetriever = new DataRetriever("C:\\Users\\mpierce\\Duplicate Project\\Sample2.xlsx", "C", null, "F", "J");
            DataSet data = new DataSet(dataRetriever);
            DuplicatePruner pruner = new DuplicatePruner(data);

            pruner.prune(0.90, 20, data.getRows());
            List<Cluster> clusters1 = data.getClusters();
            pruner.prune(0.90, 20, data.getReverseRows());
            List<Cluster> clusters2 = data.getClusters();

            dataRetriever.copySpreadsheetToFile("C:\\Users\\mpierce\\Duplicate Project\\TestOutput.xlsx");
            dataRetriever.writeDuplicates(data.getClusters(), "C:\\Users\\mpierce\\Duplicate Project\\TestOutput.xlsx");
            
            //Console.Read();
        }
    }
}
