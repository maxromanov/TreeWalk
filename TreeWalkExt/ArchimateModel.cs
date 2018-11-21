using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TreeWalk;

namespace TreeWalk
{
    public class ArchimateModel: XMLInputNode
    {
        public ArchimateModel(string URL):base(URL)
        {

        }

        public ArchimateModel(InputTreeNode _parent, object childObj) : base(_parent, childObj)
        {

        }

        public override string GetName()
        {
            XmlElement e = current ?? ( document?.DocumentElement );
            if(e != null)
            {
                return e.Attributes["name"].Value;
            }
            return "archimate:model";
        }

        public override InputTreeNode CreateChild(object childObj)
        {
            return new ArchimateModel(this, childObj);
        }

        public override string Property(string propertyName)
        {
            XmlElement e = current ?? (document?.DocumentElement);
            if (e != null)
            {
                foreach( XmlElement model_prop in e.GetElementsByTagName("property"))
                {
                    if (model_prop.GetAttribute("key").Equals(propertyName))
                        return model_prop.GetAttribute("value");
                }
            }
            return "";
        }
    }
}
