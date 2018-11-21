using ReqIFSharp;
using System.Collections;
using System.Collections.Generic;

namespace TreeWalk
{
    public class ReqIFSpecificationInputNode : InputTreeNode
    {
        
        private Specification current = null;

        public ReqIFSpecificationInputNode(InputTreeNode _parent, object context):base(_parent)  {
            this.current = (Specification)context;
            this.FileName = this.Name;
        }

        public override string GetName() {
            if (current != null && current.LongName != null) return current.LongName;
            return "Specification";
        }

        public override string getClassName()  {
            if (current != null && current.Type != null && current.Type.LongName != null) return current.Type.LongName;
            return "Specification";
        }

        public override string Property(string propertyName)
        {
            foreach (var Val in current.Values)
            {
                if (Val.AttributeDefinition.LongName.Equals(propertyName)) return Val.ObjectValue.ToString();
            }
            return "";
        }

        public override bool MoveNextAtttribute(ref object context) { return false; }

        public override InputTreeNode GetCurrentChild(ref object context)  {
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

    public class ReqIFSpecHierarchyEnumerator: IEnumerator<SpecHierarchy>
    {
        private List<SpecHierarchy> children;
        int Index = -1;

        public ReqIFSpecHierarchyEnumerator(List<SpecHierarchy> children)
        {
            this.children = children;
        }

        public SpecHierarchy Current { get { return children[Index]; } }

        object IEnumerator.Current { get { return children[Index]; } }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            Index++;
            if (Index >= children.Count) return false;
            return true;
        }

        public void Reset()
        {
        }
    }
}