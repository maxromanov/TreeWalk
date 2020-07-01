using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace TreeWalk
{

    public class XMLInputAttribute : InputTreeAttribute
    {
        private readonly XmlElement current = null;
        private XmlAttribute attribute = null;

        public XMLInputAttribute(XmlElement e, XmlAttribute a = null )
        {
            current = e;
            attribute = a;
            if(attribute != null)
            {
                this.Name = attribute.Name;
                this.Value = attribute.Value.ToString();
                if(attribute.SchemaInfo != null)
                {
                    this.AttribyteType = typeof(string);
                }
            }
        }

    }

    public class XMLInputNode: InputTreeNode
    {
        protected XmlDocument document = null;
        protected XmlElement current = null;

        public XMLInputNode(string URL)
        {
            this.FileName = URL;
            this.document = new XmlDocument();


            XmlReaderSettings settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema
            };
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;            
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
            settings.XmlResolver = new XmlUrlResolver();
            settings.Schemas.XmlResolver = new XmlUrlResolver();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11  
                | SecurityProtocolType.Tls12  | SecurityProtocolType.Ssl3 ;


            Logging.log.Info("Loading " + URL);
            XmlReader reader = XmlReader.Create(URL, settings);            
            this.document.Load(reader);

            reader.Close();
            
            var nsmgr = new XmlNamespaceManager(this.document.NameTable);
            nsmgr.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");

            
            
                        
        }

        private void SchemaValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Logging.log.Warn("Schema validation varning " + args.Message);
            else
                Logging.log.Error("Schema validation error: " + args.Message);
        }

        private void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
            {                
                Logging.log.Warn(" Validation warning: " + args.Message);
                Logging.log.Warn(" Validation warning exception:", args.Exception);
            }
            else
            {
                Logging.log.Error("Validation error: " + args.Message);
                Logging.log.Warn(" Validation error exception:", args.Exception);
            }
        }


        public XMLInputNode(InputTreeNode _parent, object childObj) : base(_parent)
        {
            this.current = (XmlElement)childObj;
        }

        public override InputTreeNode CreateChild(object childObj)
        {
            return new XMLInputNode(this, childObj);
        }

        public override string getClassName()
        {
            XmlElement e = current ?? document.DocumentElement;
            return e.Name;
        }

        public override string GetName()
        {
            if (document == null) return "";
            XmlElement e = current ?? document.DocumentElement;
            if (e.HasAttribute("Name")) return e.Attributes["Name"].Value.ToString();
            if (e.HasAttribute("id")) return e.Attributes["id"].Value.ToString();
            return e.Name;
        }

        public override string getPath()
        {
            XmlElement e = current ?? document.DocumentElement;
            string path_text = "";
            string path = "";
            while(e.ParentNode is XmlElement)
            {
                path_text = "/x:" + e.LocalName;
                string id_filter = "";
                foreach (XmlAttribute a in e.Attributes)
                {
                   if(a.SchemaInfo != null)   {
                        if (a.SchemaInfo.SchemaType != null)      {
                            if (a.SchemaInfo.SchemaType.Datatype.TokenizedType == XmlTokenizedType.ID) {
                                if (id_filter == "") id_filter += "[";
                                else id_filter += " and ";
                                id_filter += "@" + a.LocalName + "='" + a.Value.ToString() + "'";
                            }
                            else if (a.SchemaInfo.SchemaAttribute.Use == XmlSchemaUse.Required )
                            {
                                if (id_filter == "") id_filter += "[";
                                else id_filter += " and ";
                                id_filter += "@" + a.LocalName + "='" + a.Value.ToString() + "'";
                            }
                        }
                    }
                }
                if (id_filter == "")
                {
                    foreach (XmlAttribute a in e.Attributes)
                    {
                        if (a.SchemaInfo != null)
                        {
                            if (a.SchemaInfo.SchemaType != null)
                            {
                                if (a.SchemaInfo.SchemaType.Datatype.TokenizedType == XmlTokenizedType.None)
                                {
                                    if (id_filter == "") id_filter += "[";
                                    else id_filter += " and ";
                                    id_filter += "[@" + a.LocalName + "='" + a.Value.ToString() + "']";                                    
                                }
                            }
                        }
                    }
                }
                if (id_filter != "") id_filter += "]";
                path = path_text + id_filter + path;
                e = (XmlElement)e.ParentNode;
            }
            // adding root
            path = "/x:" + e.LocalName + path;
            return path;
        }

        public override InputTreeNode getNodeByPath(string inputPath)
        {
            XmlElement e = current ?? document.DocumentElement;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(this.document.NameTable);
            nsmgr.AddNamespace("x", e.OwnerDocument.DocumentElement.NamespaceURI);
            foreach ( XmlAttribute attribute in this.document.DocumentElement.Attributes) {
                if(attribute.Prefix.Equals("xmlns"))
                    nsmgr.AddNamespace(attribute.LocalName, attribute.Value);                
            }
            

            XmlElement child = (XmlElement)e.SelectSingleNode(inputPath,nsmgr);
            if(child == null)
            {
                Logging.log.Warn("XMLInputNode.getNodeByPath return null for path : " + inputPath);
                return null;
            }
            return CreateChild(child);
        }

        public override string Property(string propertyName)
        {
            XmlElement e = current ?? document.DocumentElement;
            if (e.HasAttribute(propertyName)) return e.GetAttribute(propertyName);
            return "";
        }

        public override bool MoveNextAtttribute(ref object context)
        {
            XmlElement e = current ?? document.DocumentElement;
            if(context == null)
            {
                context = e.Attributes.GetEnumerator();
            }
            if(context != null)
            {
                return ((IEnumerator)context).MoveNext();
            }
            return base.MoveNextAtttribute(ref context);
        }

        public override InputTreeAttribute GetCurrentAttribute(ref object context)
        {
            XmlElement e = current ?? document.DocumentElement;
            if (context != null)
            {
                return new XMLInputAttribute(e, (XmlAttribute)((IEnumerator)context).Current);
            }
            return base.GetCurrentAttribute(ref context);
        }

        public override bool MoveNextChild(ref object context)
        {
            XmlElement e = current ?? document.DocumentElement;
            if (context == null)
            {
                context = e.ChildNodes.GetEnumerator();
            }
            if (context != null)
            {
                return ((IEnumerator)context).MoveNext();
            }
            return base.MoveNextChild(ref context);
        }

        public override InputTreeNode GetCurrentChild(ref object context)
        {
            XmlElement e = current ?? document.DocumentElement;
            if (context != null)
            {
                return new XMLInputNode(this, (XmlElement)((IEnumerator)context).Current);
            }
            return base.GetCurrentChild(ref context);
        }

        public override bool MoveNextChild(ref object context, string childClassName)
        {
            return base.MoveNextChild(ref context, childClassName);
        }

        public override bool MoveNextChild(ref object context, QueryType query_type, string query)
        {
            return base.MoveNextChild(ref context, query_type, query);
        }

        public override InputTreeNode getLatestParent(string inputPath)
        {
            XmlElement e = current ?? document.DocumentElement;
            if (inputPath == "") return this;
            InputPath path = new InputPath(inputPath,'/');
            string latestPath = "/";
            InputTreeNode parent = null;
            InputTreeNode obj = null;

            foreach (InputPathSegment segment in path.Segments)
            {
                latestPath += "/x:" + segment.Name;                
                if (segment.Query != "") latestPath += "[" + segment.Query + "]";
                obj = this.getNodeByPath(latestPath);
                if(obj == null)    return parent;
                parent = obj;
            }
            return base.getLatestParent(inputPath);
        }

        public override string getIDasString()
        {
            XmlElement e = current ?? document.DocumentElement;
            foreach (XmlAttribute a in e.Attributes)
            {
                if (a.SchemaInfo != null)
                {
                    if (a.SchemaInfo.SchemaType != null)    {
                        if (a.SchemaInfo.SchemaType.Datatype.TokenizedType == XmlTokenizedType.ID)
                        {
                            return "__" + a.LocalName + "='" + a.Value.ToString() + "'";
                        }
                    }
                }
            }


            foreach (XmlAttribute a in e.Attributes)
            {
                if (a.SchemaInfo != null)
                {
                    if (a.SchemaInfo.SchemaType != null)
                    {
                        if (a.SchemaInfo.SchemaType.Datatype.TokenizedType == XmlTokenizedType.None)
                        {
                            return "__" + a.LocalName + "='" + a.Value.ToString() + "'";
                        }
                    }
                }
            }

            return base.getIDasString();
        }

        public override InputTreeNode addChild(string path, string id)
        {            
            string path_diff = path.Substring(this.getPath().Length);
            InputPath r = new InputPath(path_diff, '/');

            XmlElement e = current ?? document.DocumentElement;
            XmlDocument doc = document ?? e.OwnerDocument;
            XmlElement child = null;
            foreach(InputPathSegment  s in r.Segments)
            {
                child = doc.CreateElement(s.Name,e.NamespaceURI);
                child = (XmlElement)e.AppendChild(child);
                if(s.Query != "")
                {
                    string [] keys = s.Query.Split(new[] { " and " }, StringSplitOptions.None);
                    foreach (string keystr in keys)
                    {
                        string[] key = keystr.Substring(1).Split('=');
                        XmlAttribute a = doc.CreateAttribute(key[0]);
                        a.Value = key[1].Trim('\'');
                        child.Attributes.Append(a);
                    }
                }
                e = child;
            }
            doc.Validate(ValidationCallBack, e);
            if (child != null) return new XMLInputNode(this, child);
            return null;
        }

        public override void SetProperty(string propertyName, object propertyValue)
        {
            XmlElement e = current ?? document.DocumentElement;

            if (e.ParentNode is XmlDocument )    {  // check for ID property of root
                if(e.HasAttribute(propertyName))  {
                    if( e.Attributes[propertyName].SchemaInfo != null )        {
                        if (e.Attributes[propertyName].SchemaInfo.SchemaType != null)   {
                            if (e.Attributes[propertyName].SchemaInfo.SchemaType.Datatype.TokenizedType == XmlTokenizedType.ID) return;
                        }
                    }
                }
            }
            
            if (e.HasAttribute(propertyName))
            {
                e.Attributes[propertyName].Value = propertyValue.ToString();
            }
            else
            {
                XmlAttribute a = e.OwnerDocument.CreateAttribute(propertyName);
                a.Value = propertyValue.ToString();
                e.Attributes.Append(a);
            }            
        }

        public override void Save()
        {
            XmlElement e = current ?? document.DocumentElement;
            e.OwnerDocument.Save(FileName);
        }
    }
}
