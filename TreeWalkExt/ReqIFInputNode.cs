using ReqIFSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
       
    public class ReqIFInputNode : InputTreeNode
    {
        private ReqIF root = null;
        public ReqIFInputNode(string URL)
        {
            var deserializer = new ReqIFDeserializer();
            root = deserializer.Deserialize(URL);
        }

        public override string GetName() { return "ReqIF"; }
        public override string getClassName() { return "ReqIF"; }
        public override bool MoveNextAtttribute(ref object context)
        {
            return false;
        }

        public override InputTreeNode GetCurrentChild(ref object context)
        {
            return new ReqIFSpecificationInputNode(this, ((ReqIFSpecificationEnumerator)context).Current) ;
        }

        public override bool MoveNextChild(ref object context)
        {
            if(context == null)
            {
                foreach(var Content in root.CoreContent)
                {
                    context = new ReqIFSpecificationEnumerator(Content.Specifications);
                    break;
                }
            }
            if(context != null)
            {
                if( ((ReqIFSpecificationEnumerator)context).MoveNext() ) {
                    return true;
                }
            }
            return false;
        }
    }

    internal class ReqIFSpecificationEnumerator:IEnumerator<Specification>
    {
        private List<Specification> specifications;
        private int index = -1;

        public ReqIFSpecificationEnumerator(List<Specification> specifications)
        {
            this.specifications = specifications;
        }

        public Specification Current { get { return this.specifications[index]; } }

        object IEnumerator.Current { get { return this.specifications[index]; } }

        public void Dispose() { }

        public bool MoveNext()
        {
            index++;
            if (index  >= specifications.Count) return false;
            return true;
        }

        public void Reset() {  }
    }
}
