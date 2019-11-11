using System;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeWalk;
using TreeWalkDocx;
using TreeWalkTest;

namespace TreeWalkDocxTest
{
    [TestClass]
    public class DocxTests
    {
        [TestMethod]
        public void PrintDocxStructTest()
        {
            ConfigConst.AssemblyDir();
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Info);

            DocxInputNode node = new DocxInputNode("src/test1.docx");

            Logging.Info("Start: PrintDocxStructTest");
            Runner.Defaults();
            Runner.outputProcessorType = "PSScript";
            Runner.Run(
                ConfigConst.MkPath("src/test1.docx"),
                "",
                ConfigConst.MkPath("PrintStruct\\"));
            Logging.Info("End: PrintDocxStructTest");
            Logging.SetLevel(Level.Warn);
        }
    }
}
