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
    public class JSONInputTreeNodeTests
    {
        [TestMethod()]
        public void CreationdTest()
        {
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Warn);
            Logging.Info("Start: CreationdTest");

            JSONInputTreeNode node = new JSONInputTreeNode("job_~012f1a5fe770bb6afe_release.ocds");

            Logging.Info("End: CreationdTest");
            Logging.SetLevel(Level.Warn);

        }
    }
}