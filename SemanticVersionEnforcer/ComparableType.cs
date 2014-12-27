using System;

namespace SemanticVersionEnforcer
{
    internal class ComparableType : IComparable
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
            
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal);
        }
    }
}
