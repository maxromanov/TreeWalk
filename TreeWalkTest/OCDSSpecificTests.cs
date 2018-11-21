using System;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeWalk;

namespace TreeWalkTest
{
    [TestClass]
    public class OCDSSpecificTests
    {
        [TestMethod]
        public void OCDSCreateWithExtentionsSchemaTest()
        {
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Warn);
            Logging.Info("Start: OCDSCreateWithExtentionsSchemaTest");

            OCDSInputTreeNode node1 = new OCDSInputTreeNode("job_~012f1a5fe770bb6afe_release.ocds");
            OCDSInputTreeNode node2 = new OCDSInputTreeNode("database.ocds");

            Logging.Info("End: OCDSCreateWithExtentionsSchemaTest");
            Logging.SetLevel(Level.Warn);
        }

        [TestMethod]
        public void OCDSGetPropertySchemaTest()
        {
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Warn);
            Logging.Info("Start: OCDSCreateWithExtentionsSchemaTest");

            OCDSInputTreeNode node = new OCDSInputTreeNode("database.ocds");
            /*
            Assert.IsNotNull(((JSONSchema)node.Schema).getPropertySchema("description_semantic",
                "releases[*].tender"));
            */
            Logging.Info("End: OCDSGetPropertySchemaTest");
            Logging.SetLevel(Level.Warn);
        }


        [TestMethod]
        public void OCDSMergeWithExtentinsTest()
        {
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Warn);
            Logging.Info("Start: OCDSCreateWithExtentionsSchemaTest");

            TreeWalker test_walk = new TreeWalker();

            test_walk.root_input = new OCDSInputTreeNode("job_~012f1a5fe770bb6afe_release.ocds");
            test_walk.output = new DifAndMerge("database.ocds", OutputProcessorType.Merge);

            /*  Assert.IsNotNull(((JSONSchema)test_walk.input.Schema).getPropertySchema("description_semantic",
                  "releases[*].tender"));

              Assert.IsNotNull(((JSONSchema)((DifAndMerge)test_walk.output).target.Schema).getPropertySchema("description_semantic",
                       "releases[*].tender"));

              test_walk.Walk();
           */
            Logging.Info("End: OCDSMergeWithExtentinsTest");
            Logging.SetLevel(Level.Warn);
        }

        [TestMethod]
        public void OCDSRemovePropertyTest()
        {
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Warn);
            Logging.Info("Start: OCDSCreateWithExtentionsSchemaTest");

            OCDSInputTreeNode node = new OCDSInputTreeNode("job_~012f1a5fe770bb6afe_release.ocds");
            foreach (OCDSInputTreeNode c in node.Query(QueryType.JSONPath,
                               "$.releases[*].tender"))
            {
                c.DeleteProperty("description_semantic");
                Assert.AreEqual(c.Property("description_semantic"), "");
            }

            Logging.Info("End: OCDSRemovePropertyTest");
            Logging.SetLevel(Level.Warn);
        }

        [TestMethod]
        public void OCDSExtractChildTest()
        {
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Warn);
            Logging.Info("Start: OCDSCreateWithExtentionsSchemaTest");

            OCDSInputTreeNode node = new OCDSInputTreeNode("job_~012f1a5fe770bb6afe_release.ocds");

            foreach (OCDSInputTreeNode c in node.Query(QueryType.JSONPath,
                                "$.releases[*]"))
            {
                c.FileName = "test_export.ocds";
                c.SaveChild();
            }

            foreach (OCDSInputTreeNode c in node.Query(QueryType.JSONPath,
                    "$.releases[*].tender"))
            {
                c.FileName = "test_export.ocds";
                c.SaveChild();
            }

            foreach (OCDSInputTreeNode c in node.Query(QueryType.JSONPath,
                    "$.releases[*].tender.description_semantic"))
            {
                c.FileName = "test_export.ocds";
                c.SaveChild();
            }

            foreach (OCDSInputTreeNode c in node.Query(QueryType.JSONPath,
                    "$.releases[*].tender.description_semantic.sentences[*]"))
            {
                c.FileName = "test_export.ocds";
             //   c.SaveChild();
            }

            Logging.Info("End: OCDSExtractChildTest");
            Logging.SetLevel(Level.Warn);
        }


        [TestMethod]
        public void OCDSRemoveTest()
        {
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Warn);
            Logging.Info("Start: OCDSCreateWithExtentionsSchemaTest");

            OCDSInputTreeNode node = new OCDSInputTreeNode("job_~012f1a5fe770bb6afe_release.ocds");

            foreach (OCDSInputTreeNode c in node.Query(QueryType.JSONPath,
                                "$.releases[*].tender.description_semantic"))
            {
                c.MarkToDelete();
            }

            node.DeleteMarked();

            Logging.Info("End: OCDSRemoveTest");
            Logging.SetLevel(Level.Warn);
        }

        [TestMethod]
        public void OCDSRemoveTest1()
        {
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Warn);
            Logging.Info("Start: OCDSCreateWithExtentionsSchemaTest");

            OCDSInputTreeNode node = new OCDSInputTreeNode("job_~012f1a5fe770bb6afe_release.ocds");

            foreach (OCDSInputTreeNode c in node.Query(QueryType.JSONPath,
                                "$.releases[*].tender"))
            {
                c.MarkToDelete();
            }

            node.DeleteMarked();

            Logging.Info("End: OCDSRemoveTest1");
            Logging.SetLevel(Level.Warn);
        }

        [TestMethod]
        public void OCDSRemoveTest2()
        {
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Warn);
            Logging.Info("Start: OCDSCreateWithExtentionsSchemaTest");

            OCDSInputTreeNode node = new OCDSInputTreeNode("job_~012f1a5fe770bb6afe_release.ocds");

            foreach (OCDSInputTreeNode c in node.Query(QueryType.JSONPath,
                                "$.releases[*]"))
            {
                c.MarkToDelete();
            }

            node.DeleteMarked();

            Logging.Info("End: OCDSRemoveTest2");
            Logging.SetLevel(Level.Warn);
        }

        [TestMethod]
        public void OCDSRemoveTest3()
        {
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Warn);
            Logging.Info("Start: OCDSCreateWithExtentionsSchemaTest");

            OCDSInputTreeNode node = new OCDSInputTreeNode("job_~012f1a5fe770bb6afe_release.ocds");

            foreach (OCDSInputTreeNode c in node.Query(QueryType.JSONPath,
                                "$.releases[*].tender.description_semantic.sentences[*]"))
            {
              //  c.MarkToDelete();
            }

            node.DeleteMarked();

            Logging.Info("End: OCDSRemoveTest3");
            Logging.SetLevel(Level.Warn);
        }

        [TestMethod]
        public void OCDSRemoveTest4()
        {
            Environment.SetEnvironmentVariable("BASEDIR", ConfigConst.BASEDIR);
            Logging.SetLevel(Level.Warn);
            Logging.Info("Start: OCDSCreateWithExtentionsSchemaTest");

            OCDSInputTreeNode node = new OCDSInputTreeNode("incoming.ocds");

            foreach (OCDSInputTreeNode c in node.Query(QueryType.JSONPath,
                                "$.releases[*].tender"))
            {
                c.MarkToDelete();
            }

            node.DeleteMarked();

            Logging.Info("End: OCDSRemoveTest4");
            Logging.SetLevel(Level.Warn);
        }
    }
}
