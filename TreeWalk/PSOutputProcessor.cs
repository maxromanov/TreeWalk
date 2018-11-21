using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    public class PSOutputProcessor : OutputProcessor
    {
        private string outputPath;
        private Runspace runspace;       
        

        public PSOutputProcessor(string output)
        {
            this.outputPath = output;            
        }

        public override string getOutputPath()
        {
            return this.outputPath;
        }

        private void ProcessPSScript(string script,InputTreeNode input)
        {
            if (!File.Exists(script)) return;
            Logging.log.DebugFormat("Start: {0}",script);
            string script_body = File.ReadAllText(script);

            runspace.SessionStateProxy.SetVariable("inputObj", input);
            runspace.SessionStateProxy.SetVariable("outputObj", this);

            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Output.DataReady += Output_DataReady;
            pipeline.Error.DataReady += Error_DataReady;
            pipeline.Commands.AddScript(script_body);
            try
            {
                pipeline.Invoke();
                
            }
            catch (Exception e)
            {
                Logging.log.Error("Powershell execution error at \""+script+"\"", e);
            }                  

        }

        private void Error_DataReady(object sender, EventArgs e)
        {
            PipelineReader<object> error = sender as PipelineReader<object>;
            if (error != null)
            {
                while (error.Count > 0)
                {
                    Logging.log.Error("PSERR:"+error.Read().ToString());
                }
            }
        }

        private void Output_DataReady(object sender, EventArgs e)
        {
            PipelineReader<PSObject> output = sender as PipelineReader<PSObject>;
            if (output != null) {
                while (output.Count > 0) {
                    char[] cc = { '\n', '\r' };
                    string buffer = output.Read().ToString().Trim(cc);
                    if(buffer.Length > 0)
                        Logging.Info("PSOUT:" + buffer);
                }
            }
        }

        public override bool ProcessNode(InputTreeNode input)
        {
            string script = "";
            if (this.currentFillter != null)
            {
                if (File.Exists(this.currentFillter.script)) script = this.currentFillter.script;
                else script = this.Schema.getBasePath() + this.currentFillter.script;
            }
            else {
                script = this.Schema.getBasePath() + input.FileName;
                if (Path.GetExtension(script).Equals("")) script += ".ps1";
                if (!File.Exists(script))
                {
                    script = this.Schema.getBasePath() + input.Name;
                    if (Path.GetExtension(script).Equals("")) script += ".ps1";
                    if (!File.Exists(script)) {
                        script = this.Schema.getBasePath() + input.ClassName;
                        if (Path.GetExtension(script).Equals("")) script += ".ps1";
                    }
                }
            }
            this.ProcessPSScript(script, input);
            return false;
        }

        public override bool PostProcessing(InputTreeNode input)
        {
            string script = this.Schema.getBasePath() + "post.ps1";
            runspace.SessionStateProxy.SetVariable("inputObj", input);
            runspace.SessionStateProxy.SetVariable("outputObj", this);

            if ( File.Exists(script) )  ProcessPSScript(script, input);
            this.runspace.Close();
            return false;
        }

        public override bool PreProcessing(InputTreeNode input)
        {
            this.runspace = RunspaceFactory.CreateRunspace();
            this.runspace.Open();           
            
            this.runspace.SessionStateProxy.Path.SetLocation(outputPath);
            runspace.SessionStateProxy.SetVariable("inputObj", input);
            runspace.SessionStateProxy.SetVariable("outputObj", this);

            string script = this.Schema.getBasePath() + "init.ps1";
            if (File.Exists(script)) ProcessPSScript(script, input);
            return false;
        }
    }
}
