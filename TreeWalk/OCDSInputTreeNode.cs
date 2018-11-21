using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    public class OCDSInputTreeNode : JSONInputTreeNode
    {        
        public OCDSInputTreeNode(string inputFileURL):base(inputFileURL)
        {
            string rootSchemaURL = "http://standard.open-contracting.org/latest/en/release-package-schema.json";
            WebClient webClient = new WebClient();
            string JSONStr = webClient.DownloadString(rootSchemaURL);


            JObject rootSchema = JObject.Parse(JSONStr);
            string releaseSchemaRef = rootSchema.SelectToken("properties.releases.items.$ref").ToString();
            JSONStr = webClient.DownloadString(releaseSchemaRef);
            JObject releaseSchema = JObject.Parse(JSONStr);
            


            if(root != null)
            {
                JProperty extProperty = ((JObject)root).Property("extensions");
                
                if(extProperty != null)
                {
                    foreach(JToken extUrl in extProperty.Value.Children() )
                    {
                        try
                        {
                            Uri fNameUri = new Uri(extUrl.ToString());
                            string fName = Path.GetFileName(fNameUri.AbsolutePath);
                            if(fName.Equals("extension.json"))
                            {
                                fName = Path.GetDirectoryName(fNameUri.AbsolutePath) + "/release-schema.json";
                                UriBuilder builder = new UriBuilder(fNameUri);
                                builder.Path = fName;
                                JSONStr = webClient.DownloadString(builder.Uri);
                                JObject patch = JObject.Parse(JSONStr);
                                releaseSchema.Merge(patch);
                            }
                        }
                        catch(Exception e)
                        {
                            Logging.log.Error("OCDSInputTreeNode extension apply error:", e);
                        }
                    }
                }
            }

            ((JObject)rootSchema.SelectToken("properties.releases")).Property("items").Value = releaseSchema;

            this.Schema = new JSONSchema(rootSchema);
        }

        public OCDSInputTreeNode(InputTreeNode _parent, object context):base(_parent,(JToken)context)
        {
            
        }

        public override InputTreeNode CreateChild(object childObj)
        {
            return new OCDSInputTreeNode(this,childObj);
        }

        public static new OCDSInputTreeNode Create(string file_name)
        {
            if (!File.Exists(file_name))
            {
                string baseJson = "{  }";
                File.WriteAllText(file_name, baseJson);
            }
            return new OCDSInputTreeNode(file_name);
        }

        protected override string InitRootStr()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine(" \"$schema\" : \"http://standard.open-contracting.org/latest/en/release-package-schema.json\", ");
            sb.AppendLine(" \"version\" : \"1.1\", ");
            sb.AppendLine(" \"extensions\" : [ \"http://digitaltwins.info/schema/semantic/extension.json\" ], ");
            return sb.ToString();
        }
    }
}
