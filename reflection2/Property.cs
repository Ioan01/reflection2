using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reflection2
{
    internal class Property
    {
        public bool IsPrimitive { get; set; }

        public NewType NewType { get; set; }

        public Type OldType { get; set; }

        public Property(NewType newType)
        {
            NewType = newType;
            IsPrimitive = false;
        }

        public Property(Type oldType)
        {
            OldType = oldType;
            IsPrimitive = true;
        }

        public override string ToString()
        {
            return IsPrimitive ? OldType.Name : NewType.Name;
        }

        public object GetDefaultValue()
        {
            if (IsPrimitive)
            {
                if (OldType.IsValueType)
                {
                    return Activator.CreateInstance(OldType);
                }

                return "";
            }
            else return new Instance(NewType);
        }
    }
}
