/*Class: Similarity Comparer
 * Contains methods used to detect the similarity between two records
 * Makes use of two different types of comparisons
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    //TODO refactor all methods to take strings instead of records
    public class StringComparer
    {
        delegate int del();

        public double jaroWinklerCompare(Record r1, Record r2)
        {
            return jaroWinklerCompare(r1.getFullName(), r2.getFullName());
        }

        public double jaroWinklerCompare(String s1, String s2)
        {
            String maxStr, minStr;
            
            if (s1.Length >= s2.Length)
            {
                maxStr = s1;
                minStr = s2;
            }
            else
            {
                maxStr = s2;
                minStr = s1;
            }

            int m = (maxStr.Length / 2) - 1;

            for (int i = 0; i < maxStr.Length; i++)
            {
                del winMax = () => Math.Min(i+m, minStr.Length);
                del winMin = () => Math.Max(i-m, 0);
                Console.Out.WriteLine("min:" + winMin + "max:" + winMax);
            }
            return 0;
        }

        //TODO use a hashtable here
        public double nGramCompare(Record r1, Record r2)
        {
            //handle numbers
            int x, y;
            if (Int32.TryParse(r1.getFullName(),out x) && Int32.TryParse(r2.getFullName(), out y))
            {
                if (x == y)
                {
                    return 0.999;
                }
                else
                {
                    return 0.00;
                }
            }
            else if(Int32.TryParse(r1.getFullName(), out x) || Int32.TryParse(r2.getFullName(), out y))
            {
                return 0.00;
            }
            var result = r1.nGrams.Intersect(r2.nGrams);
            double rc = result.Count();
            double r1c = r1.nGrams.Count();
            double r2c = r2.nGrams.Count();
            double similarity = (rc*2)/(r1c+r2c);
            return similarity;
        }

        public double nGramCompareDesc(Record r1, Record r2)
        {
            NameParser parser = new NameParser();
            List<String> r1NGrams = parser.parseNGrams(r1.getDescription(), 3);
            List<String> r2NGrams = parser.parseNGrams(r2.getDescription(), 3);
            //TODO code reuse here
            var result = r1NGrams.Intersect(r2NGrams);
            double rc = result.Count();
            double r1c = r1NGrams.Count;
            double r2c = r2NGrams.Count;
            double similarity = (rc * 2) / (r1c + r2c);
            return similarity;
        }
    }
}
