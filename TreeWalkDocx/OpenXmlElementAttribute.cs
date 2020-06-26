using DocumentFormat.OpenXml;
using TreeWalk;

namespace TreeWalkDocx
{
    internal class OpenXmlElementAttribute : InputTreeAttribute
    {
        private OpenXmlAttribute current;

        public OpenXmlElementAttribute(OpenXmlAttribute current)
        {
            this.current = current;
            Name = current.LocalName;
            Value = current.Value;
        }

    }
}