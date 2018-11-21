using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    public class TreeWalker : Walker
    {

        public override void Walk()
        {
            if( root_input == null ) { throw new InvalidOperationException(" Cant start walking without root input node!");  }
            output.PreProcessing(root_input);
            WalkNode(root_input);
            output.PostProcessing(root_input);
        }

        protected virtual void WalkNode(InputTreeNode input)
        {
            Logging.log.InfoFormat("Walk node {0}::{1}",input.getClassName(),input.GetName());
            output.ProcessNode(input);
            Logging.log.DebugFormat("Processing atributes on node {0}::{1}", input.getClassName(), input.GetName());
            foreach (InputTreeAttribute attribute in input.attributes)
            {
                output.ProcessAttribute(attribute, input);
            }
            Logging.log.DebugFormat("Processing childs of node {0}::{1}", input.getClassName(), input.GetName());
            foreach (InputTreeNode childNode in input.childs )
            {
                WalkNode(childNode);
            }
            Logging.log.DebugFormat("Done with walk node {0}::{1}", input.getClassName(), input.GetName());
        }
    }
}
