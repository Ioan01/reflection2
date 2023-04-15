using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reflection2
{
    internal class Constructor
    {

        public string Typename { get; }

        private readonly Dictionary<string,Type> types = new Dictionary<string, Type>();


        public Instance Create(NewType type,string[] values)
        {
            if (values == null)
                values = new string[0];

            int index = 0;

            Instance newInstance = new Instance(type);


            foreach (var (name,_type) in types)
            {
                if (index < values.Length)
                    switch (Type.GetTypeCode(_type))
                    {
                        case TypeCode.Int32:
                            newInstance.AddField(name, Convert.ToInt32(values[index]));
                            break;
                        case TypeCode.Char:
                            newInstance.AddField(name, values[0]);
                            break;
                        case TypeCode.Boolean:
                            newInstance.AddField(name,Convert.ToBoolean(values));
                            break;
                        case TypeCode.Double:
                            newInstance.AddField(name,Convert.ToDouble(values));
                            break;
                        case TypeCode.String:
                            newInstance.AddField(name, values[index]);
                            break;
                    }
                else
                {
                    if (_type.IsValueType)
                        newInstance.AddField(name,Activator.CreateInstance(_type));
                    else newInstance.AddField(name,"");
                }
                index++;
            }


            return newInstance;
        }

        public void AddField(string name,Type type)
        {
            types.Add(name,type);
        }

        public void RemoveField(string name)
        {
            types.Remove(name);
        }
    }
}
