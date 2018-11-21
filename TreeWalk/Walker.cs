using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWalk
{
    public class Walker
    {
        private InputSchema _schema;
        public InputTreeNode root_input;
        public OutputProcessor output;

        public InputSchema schema { get => GetSchema(); set => SetSchema(value); }

        public InputSchema GetSchema()
        {
            return _schema;
        }

        public void SetSchema(InputSchema value)
        {
            this._schema = value;
        }

        public virtual void Walk()
        {
            throw new NotImplementedException();
        }
    }
}
