using log4net;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    public enum RunnerType { Tree, Query, FilteredTree }

    public enum InputTreeType
    {
        None,
        FileDir,
        Xml,
        ODataURL,
        TTLUrl,
        JSON,
        OCDS,
        ReqIF
    }

    public enum TreeNodeStyle
    {
        Node = 0,
        Array = 1,
        Value = 2
    }

    public enum QueryType   {
        JSONPath,
        XPath,
        XQuery,
        SQL,
        WildcardPattern
    }

    public enum OutputProcessorType
    {
        None,
        ODataUrl,
        FileDir,
        XmlFile,
        T4Dir,
        DifAndMerge,
        Merge,
        QueryPSScript,
        PSScript        
    }


    /// <summary>
    /// for getting the log level that belongs to a string
    /// </summary>
    public static class LogLevelMap
    {
        static LevelMap levelMap = new LevelMap();

        static LogLevelMap()
        {
            foreach (FieldInfo fieldInfo in typeof(Level).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (fieldInfo.FieldType == typeof(Level))
                {
                    levelMap.Add((Level)fieldInfo.GetValue(null));
                }
            }
        }

        public static Level GetLogLevel(string logLevel)
        {
            if (string.IsNullOrWhiteSpace(logLevel))
            {
                return null;
            }
            else
            {
                return levelMap[logLevel];
            }
        }
    }

    public static class Logging
    {
        public static log4net.ILog log = log4net.LogManager.GetLogger("TreeWalkLog");


        static Logging()
        {
            string path_to_config = Assembly.GetAssembly(typeof(TreeWalk.Logging)).Location;
            FileInfo logConfig = new FileInfo( Path.ChangeExtension(path_to_config,"log4net.config") );
            log4net.Config.XmlConfigurator.Configure(logConfig);
            log = log4net.LogManager.GetLogger("TreeWalkLog");
        }
       

        public static void Info(string v)
        {
            log.Info(v);
        }

        public static void SetLevel(Level target)
        {
            ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root.Level = target;
            ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).RaiseConfigurationChanged(EventArgs.Empty);
        }

        public static void SetLevel(string target)
        {
            Level l = LogLevelMap.GetLogLevel(target);
            SetLevel(l);
        }
    }
}
