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
    [TestClass()]
    public class TemplateTest
    {

        [TestMethod()]
        public void TemplatingUseCaseTest()
        { 
            Environment.SetEnvironmentVariable("BASEDIR",ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Info);
            Logging.Info("Start: TemplatingUseCaseTest");
            Runner.Defaults();
            TreeWalk.Runner.runnerType = "FilteredTree";
            TreeWalk.Runner.outputProcessorType = "PSScript;T4Dir";

            TreeWalk.Runner.Run(
                ConfigConst.MkPath("src\\InFolder1\\"),
                ConfigConst.MkPath("dst\\OutFolder\\"),
                ConfigConst.MkPath("..\\TreeWalk\\STPL\\")
            );           
           

            Logging.Info("End: ReqIFApplyToModelTest");
            Logging.SetLevel(Level.Warn);

        }
    }
}
