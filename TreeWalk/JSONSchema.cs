using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace TreeWalk
{
    public class JSONClassDef :InputClassDef
    {

    }


    public class UndefinedTokenSchema : Exception
    {
        private string token_name = "";
        
        public UndefinedTokenSchema(string token_name) 
        {
            this.token_name = token_name;
        }

        public override string Message
        {
            get
            {
                return "Undefined token name :"+token_name;
            }
        }
    }

    public class JSONSchema : InputSchema
    {
        private JSchema schema = null;

        public JSONSchema(string URL)
        {
            var webClient = new System.Net.WebClient();
            var JSONStr = webClient.DownloadString(URL);
            JSchemaUrlResolver resolver = new JSchemaUrlResolver();
            schema = JSchema.Parse(JSONStr,resolver);
        }

        public JSONSchema(JObject root)
        {
            JSchemaUrlResolver resolver = new JSchemaUrlResolver();
            schema = JSchema.Parse(root.ToString(), resolver);
        }

        public override InputAttributeDef GetAttrDef(InputTreeAttribute attr, InputTreeNode input)
        {
            return base.GetAttrDef(attr, input);
        }

        public override string GetRootClass()
        {
            return base.GetRootClass();
        }

        public virtual string getPath(JToken current)
        {
            string result = "";
            string[] components = current.Path.Split('.');
            string inputPath = "";
            foreach(string component in components)
            {
                inputPath += "." + component;
                inputPath = inputPath.Trim('.');
                if (component.Contains("["))
                {
                    string prop_name = component.Substring(0, component.IndexOf('['));
                    string id_value = "?(";
                    JSchema prop_schema = this.getPropertySchema(prop_name,result.Trim('.'));
                    if( (prop_schema.Type == JSchemaType.Array) && (prop_schema.Items.Count == 1))
                    {                        
                        foreach(JSchema item in prop_schema.Items)
                        {
                            if (item.Type == JSchemaType.Object)
                            {
                                JObject c = (JObject)current.Root.SelectToken(inputPath);
                                foreach (string key_name in item.Required)
                                {
                                    if (c.Property(key_name) != null)
                                    {
                                        if (c.Property(key_name).Value.Type == JTokenType.Array) continue;
                                        string key_value = c.Property(key_name).Value.ToString();
                                        if (id_value != "?(") id_value += " && ";
                                        if (c.Property(key_name).Value.Type == JTokenType.Integer)
                                            id_value += "@." + key_name + "==" + key_value + "";
                                        else
                                            id_value += "@." + key_name + "=='" + key_value + "'";
                                    }
                                }
                            }
                            else if(item.Type == null && item.AnyOf.Count > 0)
                            {
                                foreach(JSchema variant in item.AnyOf)
                                {
                                    
                                    if(variant.Type == JSchemaType.Object && variant.Required.Count > 0)
                                    {
                                        JObject c = (JObject)current.Root.SelectToken(inputPath);
                                        string keys_variant_str = "";
                                        bool keys_found = true;
                                        foreach (string key_name in variant.Required)
                                        {
                                            if (c.Property(key_name) != null) {
                                                if (c.Property(key_name).Value.Type == JTokenType.Array) continue;
                                                string key_value = c.Property(key_name).Value.ToString();
                                                if (keys_variant_str != "") keys_variant_str += " && ";
                                                keys_variant_str += "@." + key_name + "==";
                                                if (c.Property(key_name).Value.Type == JTokenType.Integer)
                                                    keys_variant_str += key_value ;
                                                else keys_variant_str += "'" + key_value + "'";
                                            }
                                            else {
                                                keys_found = false;
                                                break;
                                            }
                                        }
                                        if(keys_found)
                                        {
                                            id_value += keys_variant_str;
                                            break;
                                        }

                                    }
                                }
                            }
                            else 
                            {
                                JToken c = current.Root.SelectToken(inputPath);
                                if(c.Type == JTokenType.String)
                                {
                                    id_value += "@ == '" + c.ToString() + "'";
                                }
                            }
                        }
                    }
                    
                    id_value += ")";
                    result += prop_name + "[" + id_value + "].";
                }
                else result += component + ".";
            }
            result = result.Trim('.');
            return result;
        }

        public JSchema getPropertySchema(string prop_name, string path)
        {
            if(path == "")
            {
                if (schema.Properties.ContainsKey(prop_name)) return schema.Properties[prop_name];
            }
            InputPath jp = new InputPath(path);
            JSchema cs = schema;
            foreach(InputPathSegment segment in jp.Segments)
            {
                if (cs.Type == JSchemaType.Object)
                {
                    if (!cs.Properties.ContainsKey(segment.Name)) return null;
                    cs = cs.Properties[segment.Name];
                }
                else if( cs.Type == JSchemaType.Array)
                {
                    bool schemaFound = false;
                    foreach(JSchema item in cs.Items)
                    {
                        if (item.Type == JSchemaType.Object)
                        {
                            if (item.Properties.ContainsKey(segment.Name))
                            {
                                cs = item.Properties[segment.Name];
                                schemaFound = true;
                                break;
                            }
                        } else if(item.AnyOf.Count > 0)
                        {
                            foreach(JSchema variant in item.AnyOf)
                                if(variant.Properties.ContainsKey(segment.Name))
                                {
                                    cs = variant.Properties[segment.Name];
                                    schemaFound = true;
                                    break;

                                }
                            if (schemaFound) break;
                        }
                    }
                    if (!schemaFound) return null;
                }
            }
            if (cs.Type == JSchemaType.Array && cs.Items.Count == 1)
                foreach (JSchema item in cs.Items)
                  if(item.Type == JSchemaType.Object) cs = item;
                  else if(item.Type == null && item.AnyOf.Count > 0)
                    {
                        foreach (JSchema variant in item.AnyOf)
                            if (variant.Properties.ContainsKey(prop_name))
                                return variant.Properties[prop_name];
                    }
            try
            {
                return cs.Properties[prop_name];
            }
            catch(Exception e)
            {
                throw new Exception("Can't find property schema  for  "+prop_name+" property at the position "+path, e);
            }
        }

        internal InputTreeNode getNodeByPath(JToken targetToken, string inputPath, InputTreeNode targetNode)
        {
            if (inputPath == "") return targetNode;

            JToken result = targetToken.Root.SelectToken(inputPath);
            if (result != null)
                    return new JSONInputTreeNode(targetNode, result);
            return null;
        }

        private bool IsRootProperty(string component)
        {
            return schema.Properties.ContainsKey(component);
        }

        public virtual InputTreeNode getLatestParent(JToken targetToken, string inputPath, JSONInputTreeNode targetNode)
        {
            if (inputPath == "") return targetNode;
            InputPath path = new InputPath(inputPath);
            string latestPath = "";
            JToken latestParent = targetToken;
            JToken obj = null;
            foreach(InputPathSegment segment in path.Segments)
            {
                latestPath += "." + segment.Name;
                latestPath = latestPath.Trim('.');
                if (segment.Query != "") latestPath += "[" + segment.Query + "]";
                obj = targetToken.Root.SelectToken(latestPath);
                if(obj == null) {
                    if (latestParent == targetToken) return targetNode;
                    return new JSONInputTreeNode(targetNode, latestParent);
                }
                latestParent = obj;
            }

            return targetNode;
        }

        internal string getIDasString(JToken i)
        {
            JToken objToid = i;
            if( (i.Type == JTokenType.Object) && (i == i.Root) )  return "__$";
            if ((i.Type == JTokenType.Property) && (((JProperty)i).Value.Type == JTokenType.Object)) objToid = ((JProperty)i).Value;

            if(objToid.Type == JTokenType.Object )
            {
                JToken parentProperty = objToid.Parent;
                while(parentProperty.Type != JTokenType.Property) {
                    parentProperty = parentProperty.Parent;
                    if (parentProperty == null) return "";
                }
                JToken parentObject = parentProperty.Parent;
                while(parentObject.Type != JTokenType.Object){
                    parentObject = parentObject.Parent;
                    if (parentObject == null) return "";
                }

                string prop_name = ((JProperty)parentProperty).Name;                
                JSchema prop_schema = this.getPropertySchema(prop_name, parentObject.Path );
                if(prop_schema == null) throw new UndefinedTokenSchema(prop_name);
                string id_value = "__";

                if ( prop_schema.Type == JSchemaType.Array && prop_schema.Items.Count == 1 )
                {
                    foreach (JSchema item in prop_schema.Items)
                    {
                        if (item.Type == JSchemaType.Object)
                        {
                            foreach (string key_name in item.Required)
                            {
                                if (id_value != "__") id_value += "&";
                                id_value += key_name + "='";
                                if (((JObject)objToid).Property(key_name) != null)
                                {
                                    if (((JObject)objToid).Property(key_name).Value.Type == JTokenType.Array) continue;
                                    string key_value = ((JObject)objToid).Property(key_name).Value.ToString();
                                    id_value += key_value;
                                }
                                id_value += "'";
                            }
                        }
                        else if (item.Type == null && item.AnyOf.Count > 0)
                        {
                            foreach(JSchema variant in item.AnyOf)
                            {
                                string key_variant_str = "";
                                bool keys_found = true;
                                foreach (string key_name in variant.Required)
                                {
                                    if (((JObject)objToid).Property(key_name) != null)   {
                                        if (((JObject)objToid).Property(key_name).Value.Type == JTokenType.Array) continue;
                                        string key_value = ((JObject)objToid).Property(key_name).Value.ToString();
                                        if (key_variant_str != "") key_variant_str += "&";
                                        key_variant_str += key_name + "='" + key_value + "'";
                                    }
                                    else {
                                        keys_found = false;
                                        break;
                                    }
                                }
                                if(keys_found)
                                {
                                    id_value += key_variant_str;
                                    break;
                                }
                            }
                        }
                        if ((i.Type == JTokenType.Property) && (((JProperty)i).Value.Type == JTokenType.Object))
                        {
                            id_value += "&property=" + ((JProperty)i).Name;
                        }
                        return id_value;
                    }
                }
                else if(prop_schema.Type == JSchemaType.Object && prop_schema.Required.Count > 0)
                {
                    foreach (string key_name in prop_schema.Required)
                    {
                        if (id_value != "__") id_value += "&";
                        id_value += key_name + "='";
                        if (((JObject)objToid).Property(key_name) != null)
                        {
                            if (((JObject)objToid).Property(key_name).Value.Type == JTokenType.Array) continue;
                            string key_value = ((JObject)objToid).Property(key_name).Value.ToString();

                            id_value += key_value;
                        }
                        id_value += "'";
                    }
                    return id_value;
                }

            }
            return "";
        }
    }
}