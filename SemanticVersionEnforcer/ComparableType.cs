using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SemanticVersionEnforcer
{
    class ComparableType : IComparable
    {
        public Type Type { get; set; }
        public ComparableType(Type type)
        {
            Type = type;
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        public int CompareTo(object other)
        {
            
            return this.ToString().CompareTo(other.ToString());
        }
    }
}
