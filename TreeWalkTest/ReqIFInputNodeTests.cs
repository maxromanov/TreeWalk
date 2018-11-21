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
    public class ReqIFInputNodeTests
    {
        [TestMethod()]
        public void ReqIFInputNodeTest()
        {
            ReqIFInputNode node = new ReqIFInputNode("src/Requirements.reqif");
        }

        [TestMethod]
        public void ReqIFApplyToModelTest()
        {
            ConfigConst.AssemblyDir();
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Info);

            ReqIFInputNode node = new ReqIFInputNode("src/Requirements.reqif");

            Logging.Info("Start: ReqIFApplyToModelTest");
            Runner.Defaults();
            Runner.outputProcessorType = "PSScript";
            Runner.Run(
                ConfigConst.MkPath("src\\Requirements.reqif"),
                "",
                ConfigConst.MkPath("ReqIFApply\\"));            
            Logging.Info("End: ReqIFApplyToModelTest");
            Logging.SetLevel(Level.Warn);
        }
    }
}