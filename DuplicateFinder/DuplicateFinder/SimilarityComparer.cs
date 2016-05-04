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
    class SimilarityComparer
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
            var result = r1.nGrams.Intersect(r2.nGrams);
            double rc = result.Count();
            double r1c = r1.nGrams.Count();
            double r2c = r2.nGrams.Count();
            double similarity = (rc*2)/(r1c+r2c);
            return similarity;
        }
    }
}
