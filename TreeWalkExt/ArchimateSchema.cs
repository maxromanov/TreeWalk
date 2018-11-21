using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TreeWalk
{
    public class ArchimateSchema : InputSchema
    {
        XmlDocument model = null;

        public ArchimateSchema(string modelURL)
        {
            model = new XmlDocument();
            model.Load(modelURL);
        }
    }
}
