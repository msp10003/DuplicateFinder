using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            DataRetriever dataRetriever = new DataRetriever("C:\\Users\\Matthew\\Documents\\Duplicate Project\\Losses - Kimon Working 2 (for Matt).xlsx", "C", null, "I", "J");
            DataSet data = new DataSet(dataRetriever);
            Console.Read();
        }
    }
}
