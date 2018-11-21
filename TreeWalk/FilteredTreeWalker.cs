using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    public class FilteredTreeWalker : TreeWalker
    {
        protected override void WalkNode(InputTreeNode input)
        {
            if (schema.checkNode(input))
            {
                output.currentFillter = schema.getFilter(input);
                base.WalkNode(input);
                output.currentFillter = null;
            }
            else
            {
                Logging.log.InfoFormat("Skip node {0}", input.Name);
            }
        }
    }
}
