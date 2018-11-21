using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace TreeWalk
{
    public class TLLInputTreeNode : InputTreeNode
    {
        IGraph graph = new Graph();
        readonly bool isRoot = true;
        public TLLInputTreeNode(string URL)
        {            
            TurtleParser turtleParser = new TurtleParser();
            UriLoader.Load(graph, new Uri(URL), turtleParser);
        }

        protected override void setClassName(string a_value)
        {
            throw new NotImplementedException();
        }

        public override string getClassName()
        {
            if(isRoot)
            {
              //  return schema.GetRootClass();
               // return ExecSPARQLStr("SELECT * WHERE { ?s a owl:Ontology }");

            }
            throw new NotImplementedException();
        }

        private string ExecSPARQLStr(string commandText)
        {
            SparqlResultSet rs =(SparqlResultSet)ExecSPARQL(commandText);
            UriNode node = (UriNode)rs[0][0];
            string result = node.Uri.Segments.Last();
            return result;
        }

        private object ExecSPARQL(string commandText)
        {
            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlParameterizedString queryString = new SparqlParameterizedString();
            queryString.Namespaces.AddNamespace("owl", new Uri("http://www.w3.org/2002/07/owl#"));
            queryString.CommandText = commandText;
            SparqlQuery q = parser.ParseFromString(queryString.ToString());
            return graph.ExecuteQuery(q);
        }
    }
}
