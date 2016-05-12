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
    class StringComparer
    {
        //TODO
        /*
        public double jaroWinklerCompare(Record r1, Record r2)
        {

        }*/

        //TODO Make this also work for description as well as name
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
