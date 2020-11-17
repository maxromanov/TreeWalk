using System;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeWalk;
using TreeWalkDocx;
using TreeWalkTest;

namespace TreeWalkDocxTest
{
    [TestClass]
    public class PptxTests
    {
        [TestMethod]
        public void PrintPptxStructTest()
        {
            ConfigConst.AssemblyDir();
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Info);

            PptxInputNode node = new PptxInputNode("src/test1.pptx");

            Logging.Info("Start: PrintPptxStructTest");
            Runner.Defaults();
            Runner.outputProcessorType = "PSScript";
            Runner.Run(
                ConfigConst.MkPath("src/test1.pptx"),
                "",
                ConfigConst.MkPath("PrintStruct\\"));
            Logging.Info("End: PrintPptxStructTest");
            Logging.SetLevel(Level.Warn);
        }

        [TestMethod]
        public void MergePptxTest()
        {
            ConfigConst.AssemblyDir();
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Info);

            Logging.Info("Start: MergePptxTest");

            Runner.Defaults();
            TreeWalk.Runner.runnerType = "Tree";
            TreeWalk.Runner.outputProcessorType = "Merge";
            /*
            TreeWalk.Runner.Run(
                ConfigConst.MkPath("in.pptx"),
                ConfigConst.MkPath("out.pptx"),
                 "");
            */
            Logging.Info("End: MergePptxTest");

            Logging.SetLevel(Level.Warn);
        }


    }
}
