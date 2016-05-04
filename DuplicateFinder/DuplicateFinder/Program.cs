using System;
using System.Collections.Generic;
using SpreadsheetLight;

namespace DuplicateFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            DataRetriever dataRetriever = new DataRetriever("C:\\Users\\Matthew\\Documents\\Duplicate Project\\Sample1.xlsx", "C", null, "I", "J");
            DataSet data = new DataSet(dataRetriever);
            System.Console.Out.Write(data.ToString());

            DateTime d = new DateTime(2015, 1, 1);
            Record r1 = new Record(1, "Smit", "hJohn", "S", d, 1, "a desc");
            Record r2 = new Record(2, "Smith", "John", "S", d, 1, "a desc");

            SimilarityComparer simComp = new SimilarityComparer();
            NameParser parser = new NameParser();
            r1.nGrams = parser.parseRecordNGrams(r1, 3);
            r2.nGrams = parser.parseRecordNGrams(r2, 3);
            System.Console.Out.Write(r1.nGramsString()+"\n");
            System.Console.Out.Write(r2.nGramsString());

            System.Console.Out.Write("\n"+simComp.nGramCompare(r1, r2));
            Console.Read();
        }
    }
}
