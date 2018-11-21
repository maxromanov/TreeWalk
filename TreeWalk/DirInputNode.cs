using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    public class DirInputNode : InputTreeNode
    {
        private FileSystemInfo current = null;

        public DirInputNode(string v)
        {
            this.current = new FileInfo(v);
            this.FileName = v;
        }

        public DirInputNode(InputTreeNode _parent , object childObj):base(_parent)
        {
            this.current = (FileSystemInfo) childObj;
            this.FileName = this.current.Name;
        }

        public override InputTreeNode CreateChild(object childObj)
        {
            return new DirInputNode(this,childObj);
        }

        public override string GetName()
        {
            if( current != null)
            {
                if(current.Attributes.HasFlag(FileAttributes.Directory))
                {
                    string[] segments = current.FullName.Split(Path.DirectorySeparatorChar);
                    return segments.Last( s => s != "");
                }
                return current.Name;
            }
            return "";
        }

        public override string getClassName()
        {
            if (current == null) return "";
            if (Parent == null) return "root";
            if (current.Attributes.HasFlag(FileAttributes.Directory)) return "dir";
            return current.Extension.Trim('.');
        }

        public override bool MoveNextAtttribute(ref object context)
        {
            return false;
        }

        public override bool MoveNextChild(ref object context)
        {
            if(context == null)
            {
                if (current.Attributes.HasFlag(FileAttributes.Directory))
                {
                    DirectoryInfo dir = new DirectoryInfo(current.FullName);
                    context = dir.EnumerateFiles().GetEnumerator();                    
                }
            }
            if (context != null)
            {
                bool next_step = false;

                if (!context.GetType().IsGenericType) return false;
                if (context.GetType().GenericTypeArguments.Length != 1) return false;

                if( context.GetType().GenericTypeArguments[0].Name == "FileInfo")
                {
                    next_step = ((IEnumerator<FileInfo>)context).MoveNext();
                    if( ! next_step)
                    {
                        DirectoryInfo dir = new DirectoryInfo(current.FullName);
                        context = dir.EnumerateDirectories().GetEnumerator();
                    }
                }
                if(context.GetType().GenericTypeArguments[0].Name == "DirectoryInfo")
                {
                    next_step = ((IEnumerator<DirectoryInfo>)context).MoveNext();
                }
                return next_step;
            }
            return false;
        }

        public override InputTreeNode GetCurrentChild(ref object context)
        {
            if (!context.GetType().IsGenericType) return null;
            if (context.GetType().GenericTypeArguments.Length != 1) return null;

            if (context.GetType().GenericTypeArguments[0].Name == "FileInfo")
            {
                return CreateChild(((IEnumerator<FileInfo>)context).Current);
            }
            if (context.GetType().GenericTypeArguments[0].Name == "DirectoryInfo")
            {
                return CreateChild(((IEnumerator<DirectoryInfo>)context).Current);
            }
            return null;
        }

        public override string getPath()
        {
            if (this.Parent == null) return "";
            string rel_path = Parent.getPath();

            if (current.Attributes.HasFlag(FileAttributes.Directory))
                return rel_path == "" ? this.Name : Parent.getPath() + "\\" + this.Name;
            return rel_path == "" ? this.Name : Parent.getPath() + "\\" + this.Name;
        }

        public override string Property(string propertyName)
        {
            if (propertyName == "FullName") return current.FullName;
            return base.Property(propertyName);
        }
    }
}
