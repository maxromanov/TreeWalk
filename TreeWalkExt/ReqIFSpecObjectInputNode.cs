using ReqIFSharp;
using System.Collections.Generic;

namespace TreeWalk
{
    public class ReqIFSpecObjectInputNode : InputTreeNode
    {
        private SpecHierarchy current;

        public ReqIFSpecObjectInputNode(InputTreeNode _parent, object context):base(_parent) {            
            this.current = (SpecHierarchy)context;
            this.FileName = this.Name;
        }
        public override string getClassName() { return current.Object.Type.LongName; }
        public override string GetName()
        {
            if (current != null && current.Object != null && current.Object.LongName != null)  return current.Object.LongName;
            return "SpecObject";
        }

        public override string Property(string propertyName)
        {
            foreach(var Val in current.Object.Values )  {
                if (Val.AttributeDefinition.LongName.Equals(propertyName))
                {
                    if (Val.AttributeDefinition is ReqIFSharp.AttributeDefinitionEnumeration)
                        return ((List<ReqIFSharp.EnumValue>)Val.ObjectValue)[0].LongName;
                    return Val.ObjectValue.ToString();
                }
            }
            return "";
        }


        public override bool MoveNextAtttribute(ref object context) { return false; }

        public override InputTreeNode GetCurrentChild(ref object context)
        {
            return new ReqIFSpecObjectInputNode(this, ((ReqIFSpecHierarchyEnumerator)context).Current);
        }

        public override bool MoveNextChild(ref object context)
        {
            if (context == null)
            {
                context = new ReqIFSpecHierarchyEnumerator(current.Children);
            }
            if (context != null)
            {
                return ((ReqIFSpecHierarchyEnumerator)context).MoveNext();
            }
            return false;
        }
    }
}