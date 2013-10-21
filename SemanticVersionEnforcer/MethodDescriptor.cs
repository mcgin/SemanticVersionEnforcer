using System;
using System.Reflection;

namespace SemanticVersionEnforcer
{
    public class MethodDescriptor
    {
       
        public Type Type { get; set; }
        public ParameterInfo ReturnType { get; set; }
        public string Name { get; set; }
        public ParameterInfo[] Parameters { get; set; }


        public override string ToString()
        {
            return Type.ToString() + ", " + ReturnType.ToString() + ", " + Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MethodDescriptor) obj);
        }
        protected bool Equals(MethodDescriptor other)
        {
            
            return Equals(Type.ToString(), other.Type.ToString()) && 
                Equals(ReturnType.ToString(), other.ReturnType.ToString()) && 
                string.Equals(Name, other.Name) && 
                ParamsEqual(other.Parameters);
        }

        private bool ParamsEqual(ParameterInfo[] otherParams)
        {
            if (otherParams.Length != Parameters.Length) {return false;}
            for (int i = 0; i < Parameters.Length; i++)
            {
                if (!otherParams[i].ParameterType.ToString().Equals(Parameters[i].ParameterType.ToString()))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Type != null ? Type.ToString().GetHashCode() : 0); 
                hashCode = (hashCode * 397) ^ (ReturnType != null ? ReturnType.ToString().GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                foreach (ParameterInfo p in Parameters)
                {
                    hashCode = (hashCode * 397) ^ (p != null ? p.ParameterType.ToString().GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (p != null ? p.Position.GetHashCode() : 0);
                }
                return hashCode;
            }
        }

    }
}