using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    public class Runner
    {
        private static Walker walker = null;
        private static string m_inputType = "";
        private static string m_outputProcessorType = "";
        private static string m_runnerType = "Tree";
        private static OutputProcessorType m_runnerOutType = OutputProcessorType.None;

        public static void Defaults()
        {
            walker = null;
            m_outputProcessorType = "";
            m_runnerType = "Tree";
            m_runnerOutType = OutputProcessorType.None;
            m_inputType = "";
        }

        public static string outputProcessorType { get => m_outputProcessorType; set => m_outputProcessorType = value; }
        public static OutputProcessorType RunnerOutType { get => m_runnerOutType; }

        public static string runnerType { get => m_runnerType; set => m_runnerType = value; }
        public static string inputType { get => m_inputType; set => m_inputType = value; }

        public static void Run( string InputPath, string OutputPath, string SchemaPath )
        { 
            Logging.Info("Init walker");
            Logging.log.InfoFormat("Input: {0}",InputPath);
            Logging.log.InfoFormat("Output: {0}", OutputPath);
            Logging.log.InfoFormat("Schema: {0}", SchemaPath);
            //try
            //{
                walker = CreateWalker();
                CreateInput(InputPath);
                CreateProcessors(OutputPath);
                CreateSchemas(SchemaPath);
                Logging.Info("Running walker");
                walker.Walk();
            //}
            //catch(Exception e)
            //{
            //    Logging.log.Error("Run exception:", e);
            //    throw e;
            //}
        }

        private static void CreateInput(string InputPath)
        {
            if( inputType != "")
            {

            }
            else if(Path.HasExtension(InputPath))
            {
               walker.root_input = InputTreeNode.Create(InputPath);
            }
            else if( Directory.Exists(InputPath) )
            {
               walker.root_input = new DirInputNode(InputPath);
            }
        }

        private static void CreateSchemas(string schemaPath)
        {
            string[] procChain = m_outputProcessorType.Split(';');
            if (procChain.Length > 1)
            {
                bool first = true;
                foreach (string proc in procChain)
                {
                    InputSchema schema = CreateSchema(proc,schemaPath);
                    ((OutProcChain)walker.output).SetProcessorSchema(proc,schema);
                    if (first)
                    {
                        walker.schema = schema;
                        first = false;
                    }

                }
            }
            else
            {
                walker.schema = CreateSchema(m_outputProcessorType, schemaPath);
                walker.output.SetSchema(walker.schema);
            }
            
        }

        private static InputSchema CreateSchema(string proc, string schemaPath)
        {
            OutputProcessorType pt = (OutputProcessorType)Enum.Parse(typeof(OutputProcessorType), proc);
            InputSchema schema = null;
            switch (pt)
            {
                case OutputProcessorType.ODataUrl:                 break;
                case OutputProcessorType.FileDir:                  break;
                case OutputProcessorType.XmlFile:                  break;
                case OutputProcessorType.T4Dir:         schema = new T4DirSchema(schemaPath); break;
                case OutputProcessorType.DifAndMerge:   schema = null; break;
                case OutputProcessorType.Merge:         schema = null; break;
                case OutputProcessorType.QueryPSScript: schema = new QuerySchema(schemaPath); break;
                case OutputProcessorType.PSScript:      schema = new QuerySchema(schemaPath); break;
                default: break;
            }
            return schema;
        }

        private static Walker CreateWalker()
        {
            RunnerType rt = (RunnerType)Enum.Parse(typeof(RunnerType), m_runnerType);
            switch (rt)
            {
                case RunnerType.Tree:          return new TreeWalker();                    
                case RunnerType.Query:         return new QueryWalker();                    
                case RunnerType.FilteredTree:  return new FilteredTreeWalker();                    
                default:  return null;
            }
        }

        private static void CreateProcessors(string OutputPath)
        {
            string [] procChain = m_outputProcessorType.Split(';');
            if(procChain.Length > 1)
            {
                walker.output = new OutProcChain(OutputPath);
                foreach (string proc in procChain)
                {
                    ((OutProcChain)walker.output).AddProcessor(CreateProcessor(OutputPath,proc));
                }
            }
            else
            {
                walker.output = CreateProcessor(OutputPath, m_outputProcessorType);
            }
        }

        private static OutputProcessor CreateProcessor(string outputPath,string processorType)
        {
            OutputProcessorType proc = (OutputProcessorType)Enum.Parse(typeof(OutputProcessorType), processorType);
            if (m_runnerOutType == OutputProcessorType.None) m_runnerOutType = proc;
            switch (proc)
            {
                case OutputProcessorType.ODataUrl:
                    break;
                case OutputProcessorType.FileDir:
                    break;
                case OutputProcessorType.XmlFile:
                    break;
                case OutputProcessorType.T4Dir:         return new T4DirProcessor(outputPath);                    
                case OutputProcessorType.DifAndMerge:   return new DifAndMerge(outputPath, OutputProcessorType.DifAndMerge);
                case OutputProcessorType.Merge:         return new DifAndMerge(outputPath, OutputProcessorType.Merge);
                case OutputProcessorType.QueryPSScript: return new PSOutputProcessor(outputPath);
                case OutputProcessorType.PSScript:      return new PSOutputProcessor(outputPath);
                default:
                    break;
            }
            return null;            
        }
    }
}
