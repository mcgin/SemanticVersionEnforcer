using System;

namespace SemanticVersionEnforcer.Core
{
    internal class ComparableType : IComparable
    {
        public ComparableType(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }

        public int CompareTo(object other)
        {
            return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }
}