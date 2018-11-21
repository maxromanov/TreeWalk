using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TreeWalk
{
    public class QuerySchema : InputSchema
    {
        private List<QueryRecord> queries = new List<QueryRecord>();
        private string _basePath;
        private string _inputSchemaURL = "";
        private InputSchema _inputSchema = null;

        public QuerySchema(string URL)
        {            
            _basePath = Path.GetFullPath(URL);
            var webClient = new System.Net.WebClient();
            var JSONStr = webClient.DownloadString(_basePath+"rules.json");             
            JObject json = JObject.Parse(JSONStr);

            JProperty inputSchemaProp = json.Property("input_schema");
            if(inputSchemaProp != null)
            {
                this._inputSchemaURL = inputSchemaProp.Value.ToString();                
                string ext = Path.GetExtension(this._inputSchemaURL);
                if(ext.Equals(".json"))
                {
                    _inputSchema = new JSONSchema(_inputSchemaURL);
                }
            }

            foreach(JProperty qr in ((JObject)json.Property("queries").Value).Properties())
            {
                QueryRecord qrObj = new QueryRecord();
                qrObj.name = qr.Name;
                qrObj.query = ((JObject)qr.Value).Property("query").Value.ToString();

                if(((JObject)qr.Value).Property("script") != null)
                   qrObj.script = ((JObject)qr.Value).Property("script").Value.ToString();

                if (((JObject)qr.Value).Property("query_type").Value.ToString() == "jsonpath") qrObj.query_type = QueryType.JSONPath;
                else if (((JObject)qr.Value).Property("query_type").Value.ToString() == "WildcardPattern") qrObj.query_type = QueryType.WildcardPattern;

                if (((JObject)qr.Value).Property("result") != null)
                    qrObj.query_result = ((JValue)((JObject)qr.Value).Property("result").Value).ToObject<bool>();



                queries.Add(qrObj);
            }
        }

        public IEnumerable<QueryRecord> getQueries()
        {
            return queries;
        }

        public override string getBasePath()
        {
            return _basePath;
        }

        public override bool checkNode(InputTreeNode input)
        {
            string src = input.getPath();
            foreach(QueryRecord rec in queries)
            {
                switch (rec.query_type)
                {
                    case QueryType.JSONPath:
                        break;
                    case QueryType.XPath:
                        break;
                    case QueryType.XQuery:
                        break;
                    case QueryType.SQL:
                        break;
                    case QueryType.WildcardPattern:
                        WildcardPattern pattern = new WildcardPattern(rec.query);
                        if (pattern.IsMatch(src))
                        {
                            return rec.query_result;
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        public override QueryRecord getFilter(InputTreeNode input)
        {
            string src = input.getPath();
            foreach (QueryRecord rec in queries)
            {
                switch (rec.query_type)
                {
                    case QueryType.JSONPath:
                        break;
                    case QueryType.XPath:
                        break;
                    case QueryType.XQuery:
                        break;
                    case QueryType.SQL:
                        break;
                    case QueryType.WildcardPattern:
                        WildcardPattern pattern = new WildcardPattern(rec.query);
                        if (pattern.IsMatch(src)) return rec;
                        break;
                    default:
                        break;
                }
            }
            return null;
        }

        public override InputSchema getInputSchema() { return _inputSchema; }
    }
}
