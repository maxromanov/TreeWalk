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
           
            Logging.Info("End: XMLMergeUseCaseTest");
            Logging.SetLevel(Level.Warn);

        }

        [TestMethod()]
        public void CsprojMergeUseCaseTest()
        {
            ConfigConst.AssemblyDir();
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Info);
            Logging.Info("Start: CsprojMergeUseCaseTest");
            Runner.Defaults();
            TreeWalk.Runner.runnerType = "Tree";
            TreeWalk.Runner.inputType = ".xml";
            TreeWalk.Runner.outputProcessorType = "Merge";


            TreeWalk.Runner.Run(
                ConfigConst.MkPath("src/sourseproject._csproj"),
                ConfigConst.MkPath("dst/destproject._csproj"),
                 "");
            
            Logging.Info("End: CsprojMergeUseCaseTest");
            Logging.SetLevel(Level.Warn);

        }

    }
}
