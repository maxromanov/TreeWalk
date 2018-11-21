using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    public class DifAndMerge : OutputProcessor
    {
        public InputTreeNode target = null;
        private InputTreeNode difference = null;

        private InputTreeNode inserts = null;
        private InputTreeNode updates = null;
        private InputTreeNode deletes = null;

        private OutputProcessorType processorType = OutputProcessorType.DifAndMerge;

        public DifAndMerge(string OutputFile,OutputProcessorType procType)
        {
            this.processorType = procType;
            string ext = Path.GetExtension(OutputFile);

            target = InputTreeNode.Create(OutputFile); 

            if (processorType == OutputProcessorType.DifAndMerge)
            {
                string differenceName = Path.ChangeExtension(OutputFile, "diff.json");
                difference = JSONInputTreeNode.Create(differenceName);
                inserts = difference.PropertyObject("inserts");
                if (inserts == null) inserts = difference.addChild("inserts");
                updates = difference.PropertyObject("updates");
                if (updates == null) updates = difference.addChild("updates");
                deletes = difference.PropertyObject("deletes");
                if (deletes == null) deletes = difference.addChild("deletes");
            }
        }

        public override bool ProcessAttribute(InputTreeAttribute a, InputTreeNode i)
        {
            string path = i.getPath();
            InputTreeNode t = target.getNodeByPath(path);
            if(t != null)
            { 
                if (t.Property(a.Name) == a.Value.ToString()) return false;
                if (processorType == OutputProcessorType.DifAndMerge)
                {
                    string old = t.Property(a.Name);
                    string id = t.getIDasString();
                    InputTreeNode log = updates.addChild(id + "&Property=" + a.Name + "&TIME=" + DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                    log.SetProperty("path", path);
                    log.SetProperty("name", a.Name);
                    log.SetProperty("old", old);
                    log.SetProperty("new", a.Value);
                }
                t.SetProperty(a.Name, a.Value);
            }
            return false;
        }

        public override bool ProcessNode(InputTreeNode input)
        {            
            string inputPath = input.getPath();
            InputTreeNode targetPos = target.getNodeByPath(inputPath);
            if(targetPos == null)
            {
                InputTreeNode parent = target.getLatestParent(inputPath);
                string input_id = "";


                if (input.NodeStyle == TreeNodeStyle.Node) input_id = input.getIDasString();
                    else if (input.NodeStyle == TreeNodeStyle.Array && input.HasChilds())
                         input_id = parent.getIDasString() + "&array=" + input.Name;
                    else if (input.NodeStyle == TreeNodeStyle.Value)
                         input_id = parent.getIDasString() + "&value='" + input.ToString() + "'";
                    if (input_id == "") return false;
                
                
                if (processorType == OutputProcessorType.DifAndMerge)
                {
                    InputTreeNode log = inserts.addChild(input_id);
                    log.SetProperty("inputPath", inputPath);
                    log.SetProperty("parentPath", parent.getPath());
                    log.SetProperty("inputID", input_id);
                    if (input.NodeStyle == TreeNodeStyle.Array) log.SetProperty("array", true);
                }
                targetPos = parent.addChild(inputPath, input_id);                
            }
            return false;
        }

        public override bool PostProcessing(InputTreeNode input)
        {
            target.Save();
            if (processorType == OutputProcessorType.DifAndMerge)
                    difference.Save();
            return false;
        }
    }
}
