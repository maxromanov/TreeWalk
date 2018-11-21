using Newtonsoft.Json.Linq;
using System;

namespace TreeWalk
{
    public class JSONInputTreeAttribute : InputTreeAttribute
    {
        private JSONInputTreeNode o;
        private JProperty prop;

        public JSONInputTreeAttribute(JSONInputTreeNode objectNode, JToken property)
        {
            this.o = objectNode;
            this.prop = (JProperty)property;
            this.Name = this.prop.Name;
            switch(this.prop.Value.Type)
            {
                case JTokenType.Boolean:
                    this.Value = this.prop.Value.ToObject<Boolean>();
                    this.AttribyteType = typeof(Boolean);
                    break;
                case JTokenType.String:
                    this.Value = this.prop.Value.ToObject<String>();
                    this.AttribyteType = typeof(String);
                    break;
                case JTokenType.Integer:
                    this.Value = this.prop.Value.ToObject<long>();
                    this.AttribyteType = typeof(long);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}