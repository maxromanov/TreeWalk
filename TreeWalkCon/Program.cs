using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeWalk;
using log4net.Core;

/*
 *  -i D:\Temp\UpWorkSpy\Scraper\Scraper\Output\tenders_20180313165501.ocds -t JSON -o D:\Temp\UpWorkSpy\Scraper\Scraper\OUTPUT\upwork_ocds.json -p DifAndMerge -s http://standard.open-contracting.org/latest/en/release-package-schema.json
 *  -i https://raw.githubusercontent.com/open-contracting/standard/1.1-dev/standard/schema/release-schema.json -t JSON -o D:\Temp\UpWorkSpy\Scraper\Scraper\OCDS\  -p T4Dir -s "C:\Users\DEV\Documents\visual studio 2017\Projects\DigitalTwins\TreeWalk\JS2PY"
 *  -i https://raw.githubusercontent.com/open-contracting/standard/1.1-dev/standard/schema/release-package-schema.json -t JSON -o D:\Temp\UpWorkSpy\Scraper\Scraper\OCDS\  -p T4Dir -s "C:\Users\DEV\Documents\visual studio 2017\Projects\DigitalTwins\TreeWalk\JS2PY"
 *  -i  D:\Temp\UpWorkSpy\Scraper\Scraper\OUTPUT\upwork_ocds.json -t JSON -o "C:\Users\DEV\Documents\visual studio 2017\Projects\DigitalTwins\TreeWalk\JSONPS" -p QueryPSScript -r Query
 */
namespace TreeWalkCon
{
    class Program
    {
        
        class Options
        {
            [Option('i', "input", Required = true, HelpText = "Input tree to be walked")]
            public string Input { get; set; }

            [Option('t', "input_type", Required = false, Default= "", HelpText = "Input tree type")]
            public string Input_type { get; set; }

            [Option('o',"output", Required = true, HelpText = "Ouput source")]
            public string Output { get; set; }

            [Option('p', "processor", Required = false, HelpText = "Output processor type", Default ="")]
            public string Processor { get; set; }

            [Option('s', "schema", HelpText ="Schema url")]
            public string Schema { get; set; }

            [Option('r', "runner", HelpText = "Run as tree or query",Default = "Tree")]
            public string runner_type { get; set; }

            [Option('l', "loglevel", HelpText ="Log4Net log level", Default = "WARN",Required = false)]
            public string log_level { get; set; }
        }

        static void Main(string[] args)
        { 
            CommandLine.Parser.Default.ParseArguments<Options>(args)
             .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts))
             .WithNotParsed<Options>((errs) => HandleParseError(errs));
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach(Error e in errs)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static int RunOptionsAndReturnExitCode(Options opts)
        {
            Level log_level = LogLevelMap.GetLogLevel(opts.log_level);
            Logging.SetLevel(log_level);
            Runner.Defaults();
            Runner.outputProcessorType = opts.Processor;
            Runner.runnerType = opts.runner_type;
            Runner.inputType = opts.Input_type;            
            try
            {
                Runner.Run(opts.Input, opts.Output, opts.Schema);
            }
            catch(Exception e)
            {
                Console.WriteLine("Walking Exception :{0}",e.Message);
                Console.Write(e.StackTrace);
                return -1;
            }
            return 0;
        }
    }
}
