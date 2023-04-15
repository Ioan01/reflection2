using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace reflection2
{
    internal class Instance
    {
        private NewType type;

        private Dictionary<string,object> fields = new Dictionary<string, object>();

        


        public Instance(NewType type)
        {
            this.type = type;
        }

        public void AddField(string name, object value)
        {
            fields.Add(name, value);
        }

        public object GetField(string name)
        {
            if (fields.ContainsKey(name)) return fields[name];
            throw new Exception($"Cannot access {name} in {type.Name}");
        }

        public void RemoveProperty(string name)
        {
            fields.Remove(name);
        }

        public void SetField(string fieldName, object value)
        {
            if (!fields.ContainsKey(fieldName))
                throw new Exception($"Cannot access field {fieldName} in {type.Name}");

            var field = fields[fieldName];

            if (field.GetType() != value.GetType())
                throw new Exception($"Mismatching types in variable {fieldName} in {type.Name}");

            if (field is Instance)
            {
                if (((field as Instance)!).type == ((value as Instance)!).type)
                {
                    fields[fieldName] = value;
                }
                else throw new Exception($"Mismatching types in variable {fieldName} in {type.Name}");
            }

            else fields[fieldName] = value;

        }

        public override string ToString()
        {
            var str = fields.Aggregate("{", (pair, valuePair) => pair + $"{valuePair.Key} : {valuePair.Value},");

            str += type.Rules.Aggregate("", (acc, rule) => acc += $"{rule.Key} : {rule.Value.compute(this)},");

            str = str.Remove(str.Length - 1);

            str += "}";
            return str;
        }

        public void FillIn()
        {
            foreach (var (pName, property) in type.Properties.Where(pair => !pair.Value.IsPrimitive))
            {
                fields.Add(pName,new Instance(property.NewType));
            }
        }

        public double ApplyRule(string rule)
        {
            return type.Rules[rule].compute(this);
        }
    }
}
