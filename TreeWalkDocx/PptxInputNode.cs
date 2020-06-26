using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeWalk;

namespace TreeWalkDocx
{
    public class PptxInputNode : InputTreeNode
    {
        PresentationDocument file;
        readonly OpenXmlElement root;
        readonly OpenXmlElement curr;
                     
        public PptxInputNode(string URL)
        {
            FileName = URL;
            file = PresentationDocument.Open(FileName, false);
            root = file.PresentationPart.Presentation; 
        }

        public PptxInputNode(InputTreeNode _parent, OpenXmlElement current) : base(_parent)
        {
            this.curr = current;
        }

        public override string GetName()
        {
            OpenXmlElement o = root ?? curr;
            if(o != null)
            {                
                return o.LocalName;
            }
            return Path.GetFileName(FileName);
        }

        public override string getClassName()
        {
            OpenXmlElement o = root ?? curr;
            return o.GetType().Name;            
        }

        public override bool MoveNextAtttribute(ref object context)
        {
            OpenXmlElement o = root ?? curr;
            if (o != null)
            {
                if(context == null)    {
                    context = o.GetAttributes().GetEnumerator();
                }                
                return ((IEnumerator<OpenXmlAttribute>)context).MoveNext();
            }            
            else return base.MoveNextAtttribute(ref context);
        }

        public override InputTreeAttribute GetCurrentAttribute(ref object context)
        {
            OpenXmlElement o = root ?? curr;
            if (o != null)
            {
                return new OpenXmlElementAttribute(((IEnumerator<OpenXmlAttribute>)context).Current);
            }
            else  return base.GetCurrentAttribute(ref context);
        }

        public override InputTreeNode GetCurrentChild(ref object context)
        {
            OpenXmlElement o = root ?? curr;
            if (o != null)
            {
                return new DocxInputNode(this,((IEnumerator<OpenXmlElement>)context).Current);
            }
            else return base.GetCurrentChild(ref context);
        }

        public override bool MoveNextChild(ref object context)
        {
            OpenXmlElement o = root ?? curr;
            if (o != null)
            {
                if (context == null) context = o.ChildElements.GetEnumerator();
                bool has_child = ((IEnumerator<OpenXmlElement>)context).MoveNext();
                while(has_child)
                {
                    OpenXmlElement child = ((IEnumerator<OpenXmlElement>)context).Current;
                    if (!(child is DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties)) return true;
                    has_child = ((IEnumerator<OpenXmlElement>)context).MoveNext();
                }
                return false;
            }
            return base.MoveNextChild(ref context);
        }
    }
}
