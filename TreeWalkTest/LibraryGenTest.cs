using System;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeWalk;

namespace TreeWalkTest
{
    [TestClass]
    public class LibraryGenTest
    {
        [TestMethod]
        public void TestPython()
        {
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Info);
            Logging.Info("Start: TestPython");
            Runner.Defaults();
            Runner.outputProcessorType = "T4Dir";
          /* 
           *   
           *    TODO: looks like unfinished copypasting 
           *     
           *    Runner.Run(
                ConfigConst.MkPath("src\\Requirements.reqif"),
                "",
                ConfigConst.MkPath("..\\TreeWalk\\ReqIFApply\\"));
           */
            Logging.Info("End: ReqIFApplyToModelTest");
            Logging.SetLevel(Level.Warn);
        }

        [TestMethod]
        public void TestCSharp()
        {

        }

        [TestMethod]
        public void TestJava()
        {

        }
    }
}
