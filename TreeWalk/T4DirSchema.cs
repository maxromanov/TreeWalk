using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    public class T4DirClassDef : InputClassDef
    {
        public string TemplatePath { get; set; }
        public string Extention { get; internal set; }
    }

    public class T4DirSchema: InputSchema
    {
        private string _baseDir;

        public T4DirSchema(string baseDir)
        {
            _baseDir = baseDir;
            if (!this._baseDir.EndsWith("\\")) this._baseDir += "\\";
        }

        public string GetBaseDir() { return _baseDir; }
        public override InputClassDef GetNodeDef(InputTreeNode input)
        {
            string l_classTemplatePath = _baseDir  + input.ClassName + ".tt";
            if( File.Exists(l_classTemplatePath))
            {
                T4DirClassDef l_def = new T4DirClassDef();
                l_def.ClassName = input.ClassName;
                l_def.TemplatePath = l_classTemplatePath;
                return l_def;
            }
            throw new System.ArgumentOutOfRangeException();
        }


    }
}
