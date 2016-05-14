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
            double sim = DuplicateFinder.StringComparer.proximity("CAARTER AMY", "CARTER AMY J");
        }

    }
}
