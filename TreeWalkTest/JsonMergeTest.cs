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
    public class JsonMergeTest: TreeWalkTestBase
    {
        [TestMethod()]
        [TestProperty("input_file", "json/1/in.json")]
        [TestProperty("output_file", "json/1/out.json")]
        public void JsonMergeUseCaseTest()
        {
            Runner.runnerType = "Tree";
            Runner.outputProcessorType = "Merge";
            Runner.Run( InFilePath, OutFilePath, "");
        }
    }
}
