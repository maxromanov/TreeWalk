using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    public class QueryWalker : Walker
    {
        public QuerySchema S { get { return (QuerySchema)schema; } }

        public override void Walk()
        {
            Console.WriteLine("Start walking query");
            output.PreProcessing(root_input);
            Console.WriteLine("PreProcessing done");
            foreach ( QueryRecord r in S.getQueries() ) {
                foreach (InputTreeNode result in root_input.Query(r.query_type, r.query))
                {
                    Console.WriteLine("Processing {0}:{1}",result.getClassName(),result.GetName());
                    result.FileName = r.script;
                    output.ProcessNode(result);
                }
            }
            Console.WriteLine("records passed");
            output.PostProcessing(root_input);
            Console.WriteLine("Query walking finalized");
        }
    }
}
