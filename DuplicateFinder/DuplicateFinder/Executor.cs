using System;
using System.Collections.Generic;
using SpreadsheetLight;

namespace DuplicateFinder
{
    public class Executor
    {
        int dupCount = 0;
        int rowCount = 0;

        static void Main(string[] args)
        {

        }

        public void execute(String inputPath, String outputPath, String nameCol, String dateCol, String descCol)
        {
            try
            {
                DataRetriever dataRetriever = new DataRetriever(inputPath, nameCol, dateCol, descCol);
                DataSet data = new DataSet(dataRetriever);
                DuplicatePruner pruner = new DuplicatePruner(data);

                pruner.prune(0.90, 20, data.getRows());
                pruner.prune(0.90, 20, data.getReverseRows());
                List<Cluster> clusters = data.getClusters();

                foreach (Cluster c in data.getClusters())
                {
                    int rCount = c.getRecords().Count;
                    if (rCount > 1)
                    {
                        dupCount = dupCount + rCount - 1;
                    }
                }

                rowCount = data.getNumRows();

                dataRetriever.copySpreadsheetToFile(outputPath);
                dataRetriever.writeDuplicates(data.getClusters(), outputPath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int getDupCount()
        {
            return dupCount;
        }

        public int getTotalRows()
        {
            return rowCount;
        }
    }
}
