using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TreeWalk.XML;

namespace TreeWalk
{
    class InputTreeNodeCreator
    {
        public static Dictionary<string, string> extTypes = new Dictionary<string, string>()
        {
            {".ttl", "TLLInputTreeNode" },
            {".reqif","ReqIFInputNode" },
            {".archimate", "ArchimateModel" },
            {".docx", "DocxInputNode" },
            {".pptx", "PptxInputNode" }
        };



        public static InputTreeNode Create(string file_name)
        {
            string ext = Path.GetExtension(file_name);
            return Create(ext, file_name);
        }

        public static InputTreeNode Create(string ext, string file_name)
        {
            if (ext == ".xml") return new XMLInputNode(file_name);
            if (ext == ".json") return new JSONInputTreeNode(file_name);
            if (ext == ".ocds") return new OCDSInputTreeNode(file_name);

            if (extTypes.ContainsKey(ext))
            {

                string type_name = extTypes[ext];
                Type node_type = AppDomain.CurrentDomain.GetAssemblies()
                                   .SelectMany(x => x.GetTypes())
                                   .FirstOrDefault(x => x.Name == type_name);
                if (node_type != null)
                {
                    ConstructorInfo constructor = null;
                    foreach (ConstructorInfo cons in node_type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (cons.GetParameters().Length == 1 && cons.GetParameters()[0].ParameterType.Name.Equals("String"))
                        {
                            constructor = cons;
                            break;
                        }
                    }
                    if (constructor != null)
                    {
                        object[] fname = new object[1] { file_name };
                        return (InputTreeNode)constructor.Invoke(fname);
                    }
                }
            }
            return null;
        }
    }
}
