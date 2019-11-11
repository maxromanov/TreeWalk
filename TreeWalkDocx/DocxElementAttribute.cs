using DocumentFormat.OpenXml;
using TreeWalk;

namespace TreeWalkDocx
{
    internal class DocxElementAttribute : InputTreeAttribute
    {
        private OpenXmlAttribute current;

        public DocxElementAttribute(OpenXmlAttribute current)
        {
            this.current = current;
            Name = current.LocalName;
            Value = current.Value;
        }

    }
}