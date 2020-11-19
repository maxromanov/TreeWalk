using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TreeWalk
{
    #region Utils
    
    public class InputTreeAttribute
    {
        private string name;
        private object value;
        private Type attribyteType = typeof(System.Object);

        public string Name { get => name; set => name = value; }
        public object Value { get => value; set => this.value = value; }
        public Type AttribyteType { get => attribyteType; set => attribyteType = value; }
    }

    public class InputAttributeEnumerator : IEnumerator<InputTreeAttribute>
    {
        private InputTreeNode inputTreeNode;
        private Object context = null;

        public InputAttributeEnumerator(InputTreeNode inputTreeNode)
        {
            this.inputTreeNode = inputTreeNode;
        }

        public InputTreeAttribute Current { get { return inputTreeNode.GetCurrentAttribute(ref context); } }

        object IEnumerator.Current { get { return Current; } }

        public void Dispose() { }
        public bool MoveNext()
        {
            return inputTreeNode.MoveNextAtttribute(ref context);
        }
        public void Reset() { throw new NotSupportedException(); }
    }

    public class InputTreeNodeEnumerator : IEnumerator<InputTreeNode>
    {
        private InputTreeNode inputTreeNode;
        private Object context = null;
        private string childClassName = "";
        private readonly QueryType query_type;
        private string query = "";

        public InputTreeNodeEnumerator(InputTreeNode inputTreeNode)
        {
            this.inputTreeNode = inputTreeNode;
        }

        public InputTreeNodeEnumerator(InputTreeNode inputTreeNode, string childClassName) : this(inputTreeNode)
        {
            this.childClassName = childClassName;
        }

        public InputTreeNodeEnumerator(InputTreeNode inputTreeNode, QueryType query_type, string query) : this(inputTreeNode)
        {
            this.query_type = query_type;
            this.query = query;
        }

        public InputTreeNode Current { get { return inputTreeNode.GetCurrentChild(ref context); } }

        object IEnumerator.Current { get { return Current; } }

        public void Dispose() { }

        public bool MoveNext()
        {
            if (!childClassName.Equals("")) return inputTreeNode.MoveNextChild(ref context, childClassName);
            if (!query.Equals("")) return inputTreeNode.MoveNextChild(ref context, query_type, query);
            return inputTreeNode.MoveNextChild(ref context);
        }

        public void Reset() { throw new NotSupportedException(); }
    }

    public class InputAttributeCollection : IEnumerable<InputTreeAttribute>
    {
        private readonly InputTreeNode inputTreeNode;
        public InputAttributeCollection(InputTreeNode inputTreeNode)
        {
            this.inputTreeNode = inputTreeNode;
        }
        public IEnumerator<InputTreeAttribute> GetEnumerator()
        {
            return new InputAttributeEnumerator(inputTreeNode);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new InputAttributeEnumerator(inputTreeNode);
        }
    }

    public class InputTreeNodeCollection : IEnumerable<InputTreeNode>
    {
        private readonly InputTreeNode inputTreeNode;
        private string childClassName = "";
        private readonly QueryType query_type = QueryType.SQL;
        private string query = "";

        public InputTreeNodeCollection(InputTreeNode inputTreeNode)
        {
            this.inputTreeNode = inputTreeNode;
        }

        public InputTreeNodeCollection(InputTreeNode inputTreeNode, string childClassName) : this(inputTreeNode)
        {
            this.childClassName = childClassName;
        }

        public InputTreeNodeCollection(InputTreeNode inputTreeNode, QueryType query_type, string query) : this(inputTreeNode)
        {
            this.query_type = query_type;
            this.query = query;
        }

        public IEnumerator<InputTreeNode> GetEnumerator()
        {
            if (!childClassName.Equals("")) return new InputTreeNodeEnumerator(inputTreeNode, childClassName);
            else if (!query.Equals("")) return new InputTreeNodeEnumerator(inputTreeNode, query_type, query);
            return new InputTreeNodeEnumerator(inputTreeNode);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!childClassName.Equals("")) return new InputTreeNodeEnumerator(inputTreeNode, childClassName);
            else if (!query.Equals("")) return new InputTreeNodeEnumerator(inputTreeNode, query_type, query);
            return new InputTreeNodeEnumerator(inputTreeNode);
        }
    }
    #endregion

    public class InputTreeNode
    {        
        public InputAttributeCollection attributes = null;
        public InputTreeNodeCollection childs = null;
        private TreeNodeStyle nodeStyle = TreeNodeStyle.Node;
        private InputTreeNode parent = null;
        private string fileName = "";
        private InputSchema _schema = null;

        #region Constructors
        public InputTreeNode()
        {
            attributes = new InputAttributeCollection(this);
            childs = new InputTreeNodeCollection(this);
            fileName = Name;
        }
        
        public virtual InputTreeNode CreateChild(object childObj)
        {
            throw new NotImplementedException();
        }

        public InputTreeNode(InputTreeNode _parent)
        {
            attributes = new InputAttributeCollection(this);
            childs = new InputTreeNodeCollection(this);
            this.parent = _parent;
            this.Schema = _parent.Schema;
            fileName = Name;
        }
        #endregion

        #region Properties
        public string ClassName { get => getClassName();  set => setClassName(value); }
        public string Name { get => GetName(); set => SetName(value); }
        public InputTreeNode Parent { get => parent; set => parent = value; }
        public TreeNodeStyle NodeStyle { get => nodeStyle; set => nodeStyle = value; }
        public string FileName { get => fileName; set => fileName = value; }
        public InputSchema Schema { get => _schema; set => _schema = value; }
        #endregion

        #region Getters
        public virtual string GetName() { throw new NotImplementedException(); }
        public virtual string getClassName() { throw new NotImplementedException(); }
        public virtual bool HasProperty(string propertyName) { throw new NotImplementedException(); }
        public virtual string Property(string propertyName) { throw new NotImplementedException(); }
        public virtual string GetNameForClassName()
        {
            string name = GetName();
            return name.Substring(0, 1).ToUpper() + name.Substring(1);
        } 
        #endregion

        #region Editors
        public virtual void SetName(string value) { throw new NotImplementedException(); }
        protected virtual void setClassName(string a_value) { throw new NotImplementedException(); }        
        public void TakeAsArray()     {  this.NodeStyle = TreeNodeStyle.Array; }        
        public virtual void SetProperty(string propertyName, object propertyValue) { throw new NotImplementedException();}
        public virtual void SetProperty(string propertyName, PSObject propertyValue) { throw new NotImplementedException(); }
        public virtual void SetPropertyFromJSONString(string propertyName, string JSONValue) { throw new NotImplementedException();}        
        public virtual void DeleteProperty(string property_name) { throw new NotImplementedException(); }
        public virtual void Delete() { throw new NotImplementedException(); }
        public virtual void MarkToDelete() { throw new NotImplementedException(); }
        public virtual void DeleteMarked() { throw new NotImplementedException(); }
        public virtual InputTreeNode addChild(string v) { throw new NotImplementedException(); }
        public virtual InputTreeNode addChild(string inputPath, string inputID) { throw new NotImplementedException(); }
        public virtual void Save() { throw new NotImplementedException(); }
        public virtual void SaveChild() { throw new NotImplementedException(); }
        #endregion

        #region Iterators    
        public virtual bool MoveNextAtttribute(ref object context)
        {
            throw new NotImplementedException();
        }

        public virtual InputTreeAttribute GetCurrentAttribute(ref object context)
        {
            throw new NotImplementedException();
        }

        public virtual InputTreeNode GetCurrentChild(ref object context)
        {
            throw new NotImplementedException();
        }

        public virtual bool MoveNextChild(ref object context)
        {
            throw new NotImplementedException();
        }

        public InputTreeNodeCollection GetChildren(string childClassName)
        {
            return new InputTreeNodeCollection(this, childClassName);
        }

        public virtual bool MoveNextChild(ref object context, string childClassName)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<InputTreeNode> Query(QueryType query_type, string query)
        {
            return new InputTreeNodeCollection(this, query_type, query);
        }

        public virtual bool MoveNextChild(ref object context, QueryType query_type, string query)
        {
            throw new NotImplementedException();
        }

        #endregion

        public virtual bool HasPropertyValue(string v1, string v2)
        {
            return (this.HasProperty(v1) && this.Property(v1).Equals(v2));
        }

        public virtual InputTreeNode PropertyObject(string v)
        {
            throw new NotImplementedException();
        }

        public virtual string getPath()
        {
            throw new NotImplementedException();
        }

        public virtual InputTreeNode getNodeByPath(string inputPath)
        {
            throw new NotImplementedException();
        }

        public virtual InputTreeNode getLatestParent(string inputPath)
        {
            throw new NotImplementedException();
        }

        public virtual string getIDasString()
        {
            throw new NotImplementedException();
        }
        internal bool HasChilds()
        {
            object tmpcontext = null;
            return this.MoveNextChild(ref tmpcontext);
        }


    }

}
