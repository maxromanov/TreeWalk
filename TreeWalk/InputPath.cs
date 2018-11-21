using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    class InputPathSegment
    {
        private string name = "";
        private string query = "";

        public string Name { get => name; set => name = value; }
        public string Query { get => query; set => query = value; }
    }
    class InputPath
    {
        private List<InputPathSegment> segments = new List<InputPathSegment>();
        private char separator = '.';

        public List<InputPathSegment> Segments { get => segments; set => segments = value; }


        private void ParsePath(string path)
        {
            string[] components = path.Split('[');
            bool isQueryStart = false;
            InputPathSegment segment = new InputPathSegment();
            bool first = true;
            foreach (string component in components)
            {
                isQueryStart = (component.IndexOf(']') >= 0);
                string current = component;
                if (isQueryStart)
                {
                    segment.Query = component.Substring(0, component.IndexOf(']'));
                    current = component.Substring(component.IndexOf(']') + 1);
                }
                current = current.Trim(separator);
                string[] steps = current.Split(separator);
                foreach (string step in steps)
                {
                    if (!first)
                    {
                        segments.Add(segment);
                        segment = new InputPathSegment();
                    }                    
                    segment.Name = step;
                    if (segment.Name.StartsWith("x:")) segment.Name = segment.Name.Substring(2);
                    first = false;
                }
            }
            if (segment.Name != "") segments.Add(segment);
        }

        public InputPath(string path)
        {
            ParsePath(path);
        }

        public InputPath(string path, char separator) 
        {
            this.separator = separator;
            ParsePath(path);
        }
    }
}
