using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    public class JSONInputTreeNode : InputTreeNode
    {
        protected JToken root = null;
        protected string rootName = "root";
        protected JToken current = null;
        protected static List<string> pathesToDelete = new List<string>();
        
        public JSONInputTreeNode(string URL)
        {
            var webClient = new System.Net.WebClient();
            var JSONStr = webClient.DownloadString(URL);
            root = JToken.Parse(JSONStr);
            if (root.Type == JTokenType.Array) TakeAsArray();
            FileName = URL;
            rootName = URL;
        }

        public JSONInputTreeNode(InputTreeNode _parent,JToken child):base(_parent)
        {
            current = child;
            FileName = Name;
            if (current.Type == JTokenType.Array) TakeAsArray();
            if (current.Type == JTokenType.Property)
            {
                if (((JProperty)current).Value.Type == JTokenType.Array) TakeAsArray();
            }
            if (current.Type == JTokenType.String) this.NodeStyle = TreeNodeStyle.Value;
        }

        public override InputTreeAttribute GetCurrentAttribute(ref object context)
        {
            if (context != null)
            {
                return new JSONInputTreeAttribute(this, ((IEnumerator<JToken>)context).Current);
            }
            return null;
        }

        public override InputTreeNode CreateChild(object childObj)
        {
            return new JSONInputTreeNode(this,(JToken)childObj);
        }

        public override InputTreeNode GetCurrentChild(ref object context)
        {
            if(context != null)
            {
                return CreateChild(((IEnumerator<JToken>)context).Current);
            }
            return null;
        }

        public override bool MoveNextAtttribute(ref object context)
        {
            JToken o = current == null ? root : current;
            if(context == null)
            {
                switch(o.Type)
                {
                    case JTokenType.Object:
                        context = ((JObject)o).Children().GetEnumerator();
                        break;
                    case JTokenType.Property:
                        JProperty p = ((JProperty)o);
                        if (p.Value.Type == JTokenType.Object) context = ((JObject)p.Value).Children().GetEnumerator();
                        break;
                    default:
                        context = null;
                        return false;
                }
            }
            
            if(context != null)
            {
                while(((IEnumerator<JToken>)context).MoveNext())
                {
                    JToken c = ((IEnumerator<JToken>)context).Current;
                    if(c.Type == JTokenType.Property)
                    {
                        JProperty p = ((JProperty)c);
                        if (p.Value.Type != JTokenType.Object && p.Value.Type != JTokenType.Array) return true;
                    }                    
                }
            }
            context = null;
            return false;
        }

        public override bool MoveNextChild(ref object context)
        {
            JToken o = current ?? root;

            if (context == null)
            {
                switch (o.Type)
                {
                    case JTokenType.Object:
                        context = ((JObject)o).Children().GetEnumerator();
                        break;
                    case JTokenType.Property:
                        JProperty p = ((JProperty)o);
                        if (p.Value.Type == JTokenType.Object) context = ((JObject)p.Value).Children().GetEnumerator();
                        else if (p.Value.Type == JTokenType.Array) context = ((JArray)p.Value).GetEnumerator();
                        break;
                    case JTokenType.Array:
                        context = ((JArray)o).GetEnumerator();
                        break;
                    default:
                        context = null;
                        return false;
                }
            }
            if (context != null)
            {
                while (((IEnumerator<JToken>)context).MoveNext())
                {
                    JToken c = ((IEnumerator<JToken>)context).Current;
                    switch (o.Type)
                    {
                        case JTokenType.Object:
                            if (c.Type == JTokenType.Property && c.HasValues)
                            {
                                JToken v = ((JProperty)c).Value;
                                if (v.Type == JTokenType.Object || v.Type == JTokenType.Array) return true;
                            }
                            break;
                        case JTokenType.Array:
                            return true;
                        case JTokenType.Property:
                            JProperty p = ((JProperty)o);
                            if (p.Value.Type == JTokenType.Object)
                            {
                                JToken v = ((JProperty)c).Value;
                                if (v.Type == JTokenType.Object || v.Type == JTokenType.Array) return true;
                            }
                            else if (p.Value.Type == JTokenType.Array) return true;
                            break;
                        default:
                            break;
                    }
                }
            }
            context = null;
            return false;
        }

        public override bool MoveNextChild(ref object context, string childClassName)
        {
            if (context == null)
            {
                if (current == null)
                {
                    if( ((JObject)root).Property(childClassName) != null)
                    {
                        JProperty child  = ((JObject)root).Property(childClassName);
                        if (child.Value.Type == JTokenType.Object) context = child.Value.Children().GetEnumerator();
                    }
                }
                else
                {
                    if(current.Type == JTokenType.Property)
                    {
                        JObject propObj = current.Children<JObject>().First<JObject>();
                        if(propObj.Property(childClassName) != null)
                        {
                            JProperty child = propObj.Property(childClassName);
                            if (child.Value.Type == JTokenType.Object) context = child.Value.Children().GetEnumerator();
                        } 
                    }
                }
            }
            if (context != null)
            {
                while (((IEnumerator<JToken>)context).MoveNext())
                {
                    JToken currentToken = ((IEnumerator<JToken>)context).Current;
                    if (currentToken.HasValues)
                    {
                        foreach (JToken child in currentToken.Children())
                            if (
                                 (child.Type == JTokenType.Object) ||
                                 (child.Type == JTokenType.Array)
                                )
                                return true;
                    }
                }
            }
            context = null;
            return false;
        }

        public override bool MoveNextChild(ref object context, QueryType query_type, string query)
        {
            if (context == null)
            {
                if (query_type == QueryType.JSONPath)
                {
                    JToken o = current ?? root;
                    context = o.SelectTokens(query).GetEnumerator();                    
                }
            }
            if(context != null) return ((IEnumerator<JToken>)context).MoveNext();
            return false;

        }

        public override string getClassName()
        {
            if (current == null) return "schema";
            if (current.Type == JTokenType.Property)
            {
                if(this.Parent != null)
                {
                    if(this.Parent.NodeStyle == TreeNodeStyle.Array)
                    {
                        return this.Parent.getClassName();
                    }
                }
                
                if (current.HasValues)
                {
                    foreach (JToken child in current.Children())
                        if (
                             (child.Type == JTokenType.Object) ||
                             (child.Type == JTokenType.Array) 
                            )
                            return ((JProperty)current).Name;
                }
            }
            return current.Type.ToString();
        }

        public override string GetName()
        {
            if (current == null) return rootName;
            if (current.Type == JTokenType.Property) return ((JProperty)current).Name;
            if (current.Type == JTokenType.String) return current.ToString();
            return "unknown";
        }

        public override bool HasProperty(string propertyName)
        {
            if(current == null)
            {
                return (((JObject)root).Property(propertyName) != null);
            }
            if(current.Type == JTokenType.Property)
            {
                if(current.HasValues)
                {
                    if(current.Children<JObject>().First<JObject>() != null)
                    {
                        return (current.Children<JObject>().First<JObject>().Property(propertyName) != null);
                    }
                }
            }
            return (((JObject)current).Property(propertyName) != null);
        }

        public override void SetProperty(string propertyName, object propertyValue)
        {
            if(current != null)
            {
                if(current.Type == JTokenType.Object)
                {
                    if(((JObject)current).Property(propertyName) != null)
                    {
                        ((JObject)current).Property(propertyName).Value = new JValue(propertyValue);
                    }
                    else
                    {
                        ((JObject)current).Add(propertyName, new JValue(propertyValue));
                    }
                }
            }
        }

        public override void SetProperty(string propertyName, PSObject propertyValue)
        {
            JObject val = JObject.FromObject(propertyValue);
            if (current != null)
            {
                if (current.Type == JTokenType.Object)
                {
                    if (((JObject)current).Property(propertyName) != null)
                    {
                        ((JObject)current).Property(propertyName).Value = val;
                    }
                    else
                    {
                        ((JObject)current).Add(propertyName, val);
                    }
                }
            }
        }

        public override void SetPropertyFromJSONString(string propertyName, string JSONValue)
        {
            JObject val = JObject.Parse(JSONValue);
            if (current != null)
            {
                if (current.Type == JTokenType.Object)
                {
                    if (((JObject)current).Property(propertyName) != null)
                    {
                        ((JObject)current).Property(propertyName).Value = val;
                    }
                    else
                    {
                        ((JObject)current).Add(propertyName, val);
                    }
                }
            }
        }

        public override string Property(string propertyName)
        {
            JToken o = current ?? root;
            switch(o.Type)
            {
                case JTokenType.Object:
                    if(((JObject)o).Property(propertyName) != null ) return ((JObject)o).Property(propertyName).Value.ToString();
                    break;
                case JTokenType.Property:
                    JProperty p = ((JProperty)o);
                    if( p.Value.Type == JTokenType.Object)
                        if (((JObject)p.Value).Property(propertyName) != null)
                            return ((JObject)p.Value).Property(propertyName).Value.ToString();
                    break;
                default:
                    break;
            }
            return "";
        }

        public override void SetName(string value)
        {
           if(current == null)
            {
                rootName = value;
                return;
            }
            throw new NotImplementedException();
        }

        public override InputTreeNode PropertyObject(string v)
        {
            if (this.current != null)
            {
                if (current.Type == JTokenType.Object)
                {
                    JProperty prop = ((JObject)current).Property(v);
                    if (prop != null)
                    {
                        return CreateChild(prop);
                    }
                }
                else if (current.Type == JTokenType.Property)
                {
                    JToken propertyValue = ((JProperty)current).Value;
                    if(propertyValue.Type == JTokenType.Object)
                    {

                        JProperty prop = ((JObject)propertyValue).Property(v);
                        if (prop != null)
                        {
                            return CreateChild(prop);
                        }
                    }
                }
                return null;
            }
            return base.PropertyObject(v);
        }

        public override InputTreeNode addChild(string ClassName)
        {
            JObject empty = new JObject();
            JProperty insertProperty = null;
            if(current.Type == JTokenType.Property)
            {
                insertProperty = (JProperty)current;
                if(!insertProperty.HasValues) insertProperty.Value = new JObject();
                if(insertProperty.Value.Type == JTokenType.Object)
                {
                    ((JObject)insertProperty.Value).Add(ClassName, empty);
                    return new JSONInputTreeNode(this, empty);                    
                }
                return null;
            }
            else if (current.Type == JTokenType.Object)
            {
                insertProperty = ((JObject)current).Property(ClassName);
            }            
            if (insertProperty == null)
            {
                ((JObject)current).Add(ClassName, empty);
                insertProperty = ((JObject)current).Property(ClassName);
            }
            else insertProperty.Add(empty);
            return new JSONInputTreeNode(this, empty);
        }

        public override string getPath()
        {
            if (current != null) return ((JSONSchema)Schema).getPath(current);
            if (root != null) return ((JSONSchema)Schema).getPath(root);
            return base.getPath();
        }

        public override InputTreeNode getNodeByPath(string inputPath)
        {
            if (current != null)  return ((JSONSchema)Schema).getNodeByPath(current,inputPath,this);
            if (root != null) return ((JSONSchema)Schema).getNodeByPath(root,inputPath,this);
            return base.getNodeByPath(inputPath);
        }

        public override InputTreeNode getLatestParent(string inputPath)
        {
            if (current != null) return ((JSONSchema)Schema).getLatestParent(current, inputPath, this);
            if (root != null) return ((JSONSchema)Schema).getLatestParent(root, inputPath, this);
            return base.getNodeByPath(inputPath);
        }

        public override string getIDasString()
        {
            return ((JSONSchema)Schema).getIDasString(current ?? root);  
        }

        public override InputTreeNode addChild(string path, string id)
        {
            InputPath insertPath = new InputPath(path);
            InputPath targetPath = new InputPath(this.getPath());
            IEnumerator<InputPathSegment> targetSegmentIterator = targetPath.Segments.GetEnumerator();
            bool followTargetPath = targetSegmentIterator.MoveNext();
            JToken tokenToAddChild = current ?? root;

            foreach(InputPathSegment insertSegment in insertPath.Segments )
            {
                if(followTargetPath)
                {
                    if (insertSegment.Name != targetSegmentIterator.Current.Name) return null;
                    followTargetPath = targetSegmentIterator.MoveNext();
                    continue;
                }
                if(tokenToAddChild.Type == JTokenType.Object)
                {
                    JProperty propToAddChild = ((JObject)tokenToAddChild).Property(insertSegment.Name);
                    if (propToAddChild == null)
                    {
                        JSchema propertySchema = ((JSONSchema)Schema).getPropertySchema(insertSegment.Name, tokenToAddChild.Path);
                        JToken empty = null;
                        if (propertySchema.Type == JSchemaType.Object) empty = new JObject();
                        else if (propertySchema.Type == JSchemaType.Array) empty = new JArray();                  
                        ((JObject)tokenToAddChild).Add(insertSegment.Name, empty);
                        propToAddChild = ((JObject)tokenToAddChild).Property(insertSegment.Name);
                    }
                    tokenToAddChild = propToAddChild.Value;
                }
            }

            string id_trunc = id.Substring(2);
            string[] id_pairs = id_trunc.Split('&');
            Dictionary<string, string> ids = new Dictionary<string, string>();
            bool emptyArray = false;
            bool addValue = false;
            string val = "";
            bool addPropery = false;
            string addPropName = "";
            
            foreach (string pair in id_pairs)
            {
                string[] parsed_pair = pair.Split('=');
#pragma warning disable CS0642 // Possible mistaken empty statement
                if (pair == "$") ; // do nothing!
#pragma warning restore CS0642 // Possible mistaken empty statement
                else if (parsed_pair[0] == "array") emptyArray = true;
                else if (parsed_pair[0] == "value") { addValue = true; val = parsed_pair[1].Trim('\''); }
                else if (parsed_pair[0] == "property") { addPropery = true; addPropName = parsed_pair[1].Trim('\''); }
                else ids.Add(parsed_pair[0], parsed_pair[1].Trim('\''));
            }


            if (tokenToAddChild.Type == JTokenType.Array && !emptyArray && !addValue )
            {
                JObject item = new JObject();
                ((JArray)tokenToAddChild).Add(item);
                tokenToAddChild = item;
            }
            if(tokenToAddChild.Type == JTokenType.Array && addValue)
            {
                ((JArray)tokenToAddChild).Add(val);
                return null;
            }

            if (tokenToAddChild.Type == JTokenType.Object && !addPropery )
            {                
                foreach (KeyValuePair<string,string> pair in ids)
                {
                    if (pair.Value.Equals("")) continue;
                    JSchema id_schema = ((JSONSchema)Schema).getPropertySchema(pair.Key, path);
                    if (id_schema != null && id_schema.Type == JSchemaType.Integer)
                        ((JObject)tokenToAddChild).Add(pair.Key, new JValue(long.Parse(pair.Value)));
                    else
                        ((JObject)tokenToAddChild).Add(pair.Key, new JValue(pair.Value));
                }
            }            
            return new JSONInputTreeNode(this,tokenToAddChild);
        }

        public override string ToString()
        {
            if (current != null && current.Type == JTokenType.String) return current.ToString(); 
            return base.ToString();
        }

        protected virtual string InitRootStr()
        {
            return "{";
        }

        public override void SaveChild()
        {
            if (current == null) throw new InvalidOperationException("Can't save root as child! Use Save instead!");

            string currentPath = this.getPath();
            InputPath curPath = new InputPath(currentPath);
            string begin = InitRootStr();
            string end = "}";
            
            foreach(InputPathSegment segment in curPath.Segments)
            {               
                begin += " \""+segment.Name + "\" : ";
                if (!segment.Query.Equals(""))
                {
                    begin += " [ ";
                    end = " ] " + end;
                }

                if (segment == curPath.Segments.Last()) break;

                if(!segment.Query.Equals(""))  {
                    begin += " { ";
                    string id_request = segment.Query.Substring(2).TrimEnd(')');
                    string[] pairs = id_request.Split('&');                    
                    foreach(string idfield in pairs)
                    {
                        if (idfield.Equals("")) continue;
                        string idfld = idfield.Trim().Substring(2);
                        int eqPos = idfld.IndexOf('=');
                        string id_name = idfld.Substring(0, eqPos);
                        string id_value = idfld.Substring(eqPos + 2).Trim('\'');
                        begin += " \"" + id_name + "\" : \"" + id_value + "\", ";

                    }
                    end = " } " + end;
                }
                else
                {
                    begin += " { ";
                    end = " } "+end;                    
                }
                
            }
            string buff = "", content = "";

            if (current.Type == JTokenType.Property) buff = ((JProperty)current).Value.ToString(); 
            else buff = current.ToString();

            content = begin + " " + buff + " " + end;
            JObject temp = JObject.Parse(content);
            content = temp.ToString();
            System.IO.File.WriteAllText(FileName, content);
            return;
        }

        public override void Save()
        {
            if(current != null)
            {
                if(current.Type == JTokenType.Property)
                {
                    string buff = ((JProperty)current).Value.ToString();
                    System.IO.File.WriteAllText(FileName, buff);
                    return;

                }
                string obj_buffer = current.ToString();
                System.IO.File.WriteAllText(FileName, obj_buffer);
                return;
            } 
            string buffer = root.ToString();
            System.IO.File.WriteAllText(FileName, buffer);
        }

        public static new JSONInputTreeNode Create(string file_name)
        {
            if (!File.Exists(file_name))
            {
                string baseJson = "{}";
                File.WriteAllText(file_name, baseJson);
            }
            return new JSONInputTreeNode(file_name);
        }

        public override void DeleteProperty(string property_name)
        {
            JToken o = current ?? root;
            if(o.Type == JTokenType.Property)
            {
                JProperty p = (JProperty)o;
                if(p.Value.Type == JTokenType.Object)
                {
                    JObject c = (JObject)p.Value;
                    if(c.Property(property_name) != null)
                    {
                        c.Remove(property_name);
                        return;
                    }
                }
            }
            else if(o.Type == JTokenType.Object)
            {
                JObject c = (JObject)o;
                if (c.Property(property_name) != null)
                {
                    c.Remove(property_name);
                    return;
                }
            }
        }

        public override void Delete()
        {
            JToken o = current ?? root;
            if( o.Type == JTokenType.Property)
            {
                JToken p = o.Parent;
                ((JObject)p).Remove(((JProperty)o).Name);
                
            }
            else
            {
                o.Remove();
            }            
        }

        public override void MarkToDelete()
        {
            string path = this.getPath();
            pathesToDelete.Add(path);
        }

        public override void DeleteMarked()
        {
            JToken o = current ?? root;
            if (current == null) o = o.Root;
            foreach (string path in pathesToDelete)
            {
               Console.WriteLine("Deleting object on path: " + path);
               foreach( JToken token in o.SelectTokens(path)) 
                if (token != null)
                {
                        JToken p = token.Parent;
                        string prop_name = "";
                        if (p != null)
                        {
                            if (p.Type == JTokenType.Property)
                            {
                                prop_name = ((JProperty)p).Name;
                                p = p.Parent;
                                ((JObject)p).Remove(prop_name);

                            }
                            else if (p.Type == JTokenType.Array)
                            {
                                ((JArray)p).Remove(token);

                            }
                            else ((JObject)token).Remove();
                        }
                        else ((JObject)token).Remove();
                        break;
                }
            }

            pathesToDelete.Clear();
        }
    }
}
