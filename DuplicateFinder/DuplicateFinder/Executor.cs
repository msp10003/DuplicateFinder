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

        public void execute(String inputPath, String outputPath, String nameCol, String dateCol, String descCol, int numberOfColumns, double namePrecision, bool? Scan_Dates, bool? Scan_Descriptions, double datePrecision, double descriptionPrecision, bool? Search_Enhance)
        {
            try
            {
                DataRetriever dataRetriever = new DataRetriever(inputPath, nameCol, dateCol, descCol, numberOfColumns);
                DataSet data = new DataSet(dataRetriever);
                DuplicatePruner pruner = new DuplicatePruner(data);

                //convert the stupid nullables to non-nullables
                bool scanDates = Scan_Dates ?? default(bool);
                bool scanDescriptions = Scan_Descriptions ?? default(bool);
                bool searchEnhance = Search_Enhance ?? default(bool);

                pruner.prune(0.6, 20, data.getRows(), scanDates, scanDescriptions, namePrecision, datePrecision, convertToQuadScale(descriptionPrecision), searchEnhance);
                pruner.prune(0.6, 20, data.getReverseRows(), scanDates, scanDescriptions, namePrecision, datePrecision, convertToQuadScale(descriptionPrecision), searchEnhance);
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

        private double convertToQuadScale(double value)
        {
            //some math went into determining this
            double quadValue = (-0.5*(Math.Pow(value,2))) + (1.05 * value) + (0.45);

            return quadValue;
        }
    }
}
