using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalkTest
{
    public class ConfigConst
    {
        public static string WSDir = "C:\\vsagentm\\_work\\2\\s";
        public static string BASEDIR = WSDir + "\\TreeWalkTest\\";


        public static string AssemblyDir()
        {
            BASEDIR = System.Reflection.Assembly.GetAssembly(typeof(ConfigConst)).Location;
            BASEDIR = System.IO.Path.GetDirectoryName(BASEDIR)+"\\";
            return BASEDIR;
        }
        public static string MkPath(string relative)
        {
            return ConfigConst.BASEDIR + relative;
        }
    }
}
