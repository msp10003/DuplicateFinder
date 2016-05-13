using System;
using DuplicateFinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DuplicateFinderUnitTests
{
    [TestClass]
    public class StringComparerTests
    {
        [TestMethod]
        public void jaroWinklerTest()
        {
            DuplicateFinder.StringComparer strComp = new DuplicateFinder.StringComparer();
            strComp.jaroWinklerCompare("DUANE", "DWAYNE");
        }

    }
}
