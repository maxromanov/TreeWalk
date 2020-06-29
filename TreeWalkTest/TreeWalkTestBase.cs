using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeWalk;

namespace TreeWalkTest
{
    public class TreeWalkTestBase
    {
        private string outFilePath;
        private string inFilePath;

        public string InFilePath { get => inFilePath; set => inFilePath = ConfigConst.MkPath(value); }
        public string OutFilePath { get => outFilePath; set => outFilePath = ConfigConst.MkPath(value); }
        public TestContext TestContext { get; set; }


        [ClassInitialize]
        public void TestSuiteSetup()
        {
            ConfigConst.AssemblyDir();
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
        }

        [ClassCleanup]
        public void TestSuiteTearDown()
        {

        }

        [TestInitialize]
        public void TestCaseSetup()
        {
            ConfigConst.AssemblyDir();
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Info);
            Logging.Info("Start: " + TestContext.TestName);
            Runner.Defaults();
            InFilePath = (string)TestContext.Properties["input_file"];
            OutFilePath = (string)TestContext.Properties["output_file"];
            if(File.Exists(OutFilePath))
            {
                string result_file = "result" + Path.GetExtension(OutFilePath);
                if (File.Exists(ConfigConst.MkPath(result_file))) File.Delete(ConfigConst.MkPath(result_file));
                File.Copy(OutFilePath, ConfigConst.MkPath(result_file));
                OutFilePath = result_file;
            }
        }

        [TestCleanup]
        public void TestCaseTearDown()
        {
            Logging.Info("End: " + TestContext.TestName);
            Logging.SetLevel(Level.Warn);
        }

    }
}
