using Newtonsoft.Json.Schema;
using System;

namespace TreeWalk
{
    public class InputSchema
    {
        public virtual InputClassDef GetNodeDef(InputTreeNode input)
        {
            throw new System.NotImplementedException(); 
        }

        public virtual InputAttributeDef GetAttrDef(InputTreeAttribute attr, InputTreeNode input)
        {
            throw new System.NotImplementedException();
        }

        public virtual QueryRecord getFilter(InputTreeNode input)
        {
            throw new NotImplementedException();
        }

        public virtual bool checkNode(InputTreeNode input) { return true; }

        public virtual string getBasePath() { throw new NotImplementedException(); }

        public virtual string GetRootClass() { throw new NotImplementedException(); }

        public virtual InputSchema getInputSchema() { throw new NotImplementedException(); }

        public static InputSchema Create(string uri)
        {
            Uri full = new Uri(uri);
            string path = full.GetLeftPart(UriPartial.Path);
            return null;
        }
                
    }

    public class InputAttributeDef
    {
    }

    public class InputClassDef
    {
        public string ClassName { get; set; }
    }


}