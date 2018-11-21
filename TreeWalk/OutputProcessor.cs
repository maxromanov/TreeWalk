using System;

namespace TreeWalk
{

    public class OutputProcessor
    {

        private InputSchema m_schema = null;
        protected InputSchema Schema { get => m_schema; }
        private QueryRecord current_filter = null;
        public QueryRecord currentFillter { get => current_filter; set { setCurrentFilter(value); } }

        public virtual void setCurrentFilter(QueryRecord value) { current_filter = value; }

        public virtual string getOutputPath() { throw new NotImplementedException(); }

        // returns true if done - for break chain 
        public virtual bool ProcessNode(InputTreeNode input)
        {
            return false;
        }

        public virtual bool ProcessAttribute(InputTreeAttribute a, InputTreeNode i)
        {
            return false;
        }

        public virtual bool PostProcessing(InputTreeNode input)
        {
            return false;
        }

        public virtual bool PreProcessing(InputTreeNode input)
        {
            return false;
        }

        public void SetSchema(InputSchema schema)
        {
            this.m_schema = schema;
        }
    }
}