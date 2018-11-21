using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeWalk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using TreeWalkTest;

namespace TreeWalk.Tests
{
    [TestClass()]
    public class ArchimateSchemaTests
    {
        [TestMethod()]
        public void ArchimateSchemaTest()
        {
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Warn);
            Logging.Info("Start: ArchimateSchemaTest");
            ArchimateSchema schema = new ArchimateSchema("src/OwnBM.archimate");
            Logging.Info("End: ArchimateSchemaTest");
            Logging.SetLevel(Level.Warn);
        }
    }
}