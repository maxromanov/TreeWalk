using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeWalk;


namespace TreeWalkTest
{
    [TestClass]
    public class XMLMergeTest
    {
        [TestMethod()]
        public void XMLMergeUseCaseTest()
        {
            ConfigConst.AssemblyDir();
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Info);
            Logging.Info("Start: XMLMergeUseCaseTest");
            Runner.Defaults();
            TreeWalk.Runner.runnerType = "Tree";
            TreeWalk.Runner.outputProcessorType = "Merge";

                   
            TreeWalk.Runner.Run(
                ConfigConst.MkPath("in.xml"),
                ConfigConst.MkPath("out.xml"),
                 "");

            /* 
            TreeWalk.Runner.Run(
                "C:\\Users\\Maksym Romanov\\BrightEye nv\\Academy - General\\Academy\\source\\courses\\Fundamentals\\items\\screen_TestNew\\imsmanifest.xml",
                "C:\\Users\\Maksym Romanov\\BrightEye nv\\Academy - General\\Academy\\source\\courses\\Fundamentals\\imsmanifest.xml",
                 "");
             */
            Logging.Info("End: XMLMergeUseCaseTest");
            Logging.SetLevel(Level.Warn);

        }

    }
}
