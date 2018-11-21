using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    public class T4DirProcessor: OutputProcessor
    {
        public string OutputDirectory { get; private set; }
        public string CurrentFileName { get; set; }
    
        public T4DirProcessor(string baseDir)
        {
            this.OutputDirectory = baseDir;
            if (!this.OutputDirectory.EndsWith("\\")) this.OutputDirectory += "\\";
        }     
        
        public override bool ProcessNode(InputTreeNode input)
        {
            String templateNamespace = ((T4DirSchema)Schema).GetBaseDir().TrimEnd('\\').Split('\\').Last();            
            String templateName = "TreeWalk." + templateNamespace + "." + input.getClassName(); 
            if( ProcessTemplate(input, Schema, templateName) ) return true;
            return false;
        }

        public override bool PostProcessing(InputTreeNode input)
        {
            if(Directory.Exists(((T4DirSchema)Schema).GetBaseDir() + "post\\"))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(((T4DirSchema)Schema).GetBaseDir() + "post\\");
                foreach (FileInfo fInfo in directoryInfo.EnumerateFiles())
                {
                    if (fInfo.Extension.Equals(".tt"))
                    {
                        string templateNamespace = ((T4DirSchema)Schema).GetBaseDir().TrimEnd('\\').Split('\\').Last();
                        string templateName = "TreeWalk." + templateNamespace + ".post."+ fInfo.Name.Split('.').First();
                        ProcessTemplate(input, Schema, templateName);
                    }
                    else if (fInfo.Extension.Equals(".cs"))
                    {

                    }
                    else
                    {
                        string content = System.IO.File.ReadAllText(fInfo.FullName);
                        System.IO.File.WriteAllText(this.OutputDirectory + fInfo.Name, content);
                    }
                }
            }
            return false;
        }

        private bool ProcessTemplate(InputTreeNode input, InputSchema schema, string templateName)
        {
            var template = Assembly.GetExecutingAssembly().CreateInstance(templateName);
            if (template == null) return false;
            Type templateType = template.GetType();
            if (templateType.GetField("input") != null) templateType.GetField("input").SetValue(template, input);
            if (templateType.GetField("output") != null) templateType.GetField("output").SetValue(template, this);
            if (templateType.GetField("schema") != null) templateType.GetField("schema").SetValue(template, schema);


            String result = template.GetType().GetMethod("TransformText").Invoke(template, null).ToString();
            
            if (result.Equals("")) return false;

            bool AppendCurrent = false;      

            if (templateType.GetField("AppendCurrent") != null)
                    AppendCurrent = bool.Parse(templateType.GetField("AppendCurrent").GetValue(template).ToString());

            if(AppendCurrent)
            {
                System.IO.File.AppendAllText(CurrentFileName, result,Encoding.UTF8);
                return true;
            }
            String outputExt = template.GetType().GetField("Ext").GetValue(template).ToString();
            string NameOfFile = input.FileName;
            if (templateType.GetField("FileName") != null)
                NameOfFile = templateType.GetField("FileName").GetValue(template).ToString();

            CurrentFileName = this.OutputDirectory + NameOfFile + outputExt;
            bool Append = false;
            if (templateType.GetField("Append") != null)
                Append = bool.Parse(templateType.GetField("Append").GetValue(template).ToString());

            if(Append) System.IO.File.AppendAllText(CurrentFileName, result, Encoding.UTF8);
            else       System.IO.File.WriteAllText(CurrentFileName, result, Encoding.UTF8);

            return true;
        }

        public override bool PreProcessing(InputTreeNode input)
        {
            if (Directory.Exists(((T4DirSchema)Schema).GetBaseDir() + "init\\"))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(((T4DirSchema)Schema).GetBaseDir() + "init\\");
                foreach (FileInfo fInfo in directoryInfo.EnumerateFiles())
                {
                    if (fInfo.Extension.Equals(".tt"))
                    {
                        string templateNamespace = ((T4DirSchema)Schema).GetBaseDir().TrimEnd('\\').Split('\\').Last();
                        string templateName = "TreeWalk." + templateNamespace +".init."+ fInfo.Name.Split('.').First();
                        ProcessTemplate(input, Schema, templateName);
                    }
                    else if (fInfo.Extension.Equals(".cs"))
                    {

                    }
                    else
                    {
                        string content = System.IO.File.ReadAllText(fInfo.FullName);
                        System.IO.File.WriteAllText(this.OutputDirectory + fInfo.Name, content);
                    }
                }
            }
            return false;
        }
    }
}
