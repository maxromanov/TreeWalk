using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TreeWalk.XML
{
    public class XMLInputTextNode : XMLInputNodeBase
    {
        private XmlText t;

        public XMLInputTextNode(InputTreeNode parent, XmlText t):base(parent)
        {
            this.t = t;            
        }
        public override string GetName() => "!Text!";
        public override string getClassName() => "!XmlBodyText!";
        public override string getPath() => Parent.getPath() + "[text()=\"" + t.InnerText + "\"]";
        public override bool MoveNextAtttribute(ref object context) => false;
        public override string getIDasString() => Parent.getIDasString() + "[text()=\"" + t.InnerText + "\"]";
        public override bool MoveNextChild(ref object context, QueryType query_type, string query) => false;
        public override bool MoveNextChild(ref object context) => false;
        public override bool MoveNextChild(ref object context, string childClassName) => false;
    }
}
