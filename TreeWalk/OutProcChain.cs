using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    public class OutProcChain: OutputProcessor
    {
        private string outputPath;
        private List<OutputProcessor> chain = new List<OutputProcessor>();

        public OutProcChain(string outputPath)
        {
            this.outputPath = outputPath;
        }

        public void AddProcessor(OutputProcessor child) { chain.Add(child); }

        public override bool PostProcessing(InputTreeNode input)
        {
            foreach(var proc in chain) {
                if ( proc.PostProcessing(input) )                
                    return true;                
            }
            return false;
        }

        public override bool PreProcessing(InputTreeNode input)
        {
            foreach (var proc in chain) {
                if (proc.PreProcessing(input))
                    return true;
            }
            return false;
        }

        public override bool ProcessAttribute(InputTreeAttribute a, InputTreeNode i)
        {
           return base.ProcessAttribute(a, i);
        }

        public override bool ProcessNode(InputTreeNode input)
        {
            foreach(var p in chain)
            {
                if (p.ProcessNode(input)) return true;
            }
            return true;
        }

        internal void SetProcessorSchema(string proc, InputSchema schema)
        {
            foreach (var p in chain)
            {
                if (p.GetType().Name == "PSOutputProcessor" && proc == "PSScript") p.SetSchema(schema);
                if (p.GetType().Name == "T4DirProcessor" && proc == "T4Dir") p.SetSchema(schema);
            }
        }

        public override void setCurrentFilter(QueryRecord value)
        {
            base.setCurrentFilter(value);
            foreach (var p in chain) p.setCurrentFilter(value);
        }
            
    }
}
