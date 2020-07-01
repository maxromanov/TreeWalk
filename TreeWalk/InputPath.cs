using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TreeWalk
{
    class InputPathSegment
    {
        private string name = "";
        private string query = "";

        public string Name { get => name; set => name = value; }
        public string Query { get => query; set => query = value; }

        public override string ToString()
        {
            return name + "[" + query + "]";
        }
    }
    class InputPath
    {
        private List<InputPathSegment> segments = new List<InputPathSegment>();
        private char separator = '.';

        public List<InputPathSegment> Segments { get => segments; set => segments = value; }


        private void ParsePath(string path)
        {            
            List<string> components = new List<string>();
            int bracket_level = 0;
            string curr = "";
            foreach(var c in path)
                if(bracket_level > 0)       {
                    curr += c;
                    if (c == '[') bracket_level++;
                    if (c == ']') bracket_level--;
                }  else {                    
                    curr += c;
                    if (c == '[') bracket_level++;
                    if (c == separator)
                    {
                        if(!string.IsNullOrEmpty(curr.Trim(separator)))
                          components.Add(curr.Trim(separator));
                        curr = "";
                    }                    
                }
            components.Add(curr.Trim('.'));
                        
            foreach (string component in components)
            {
                InputPathSegment segment = new InputPathSegment();                
                int QueryPos = component.IndexOf('[');
                string current = component;
                if (QueryPos >= 0)       {
                    segment.Query = component.Substring(component.IndexOf('[')+1, component.LastIndexOf(']') - component.IndexOf('[') - 1);
                    current = component.Substring(0,component.IndexOf('['));
                }                                
                segment.Name = current;
                if (segment.Name.StartsWith("x:")) segment.Name = segment.Name.Substring(2);
                segments.Add(segment);
            }            
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
