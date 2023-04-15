using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace reflection2
{
    internal class NewType
    {
        public Dictionary<string, Property> Properties { get; } = new Dictionary<string, Property>();

        private Constructor _constructor = new Constructor();

        public Dictionary<string, Rule> Rules { get; } = new Dictionary<string, Rule>();

        public void AddProperty(string name, Property property)
        {
            if (Properties.ContainsKey(name))
                throw new Exception("");

            Properties.Add(name,property);

            if (property.IsPrimitive)
                _constructor.AddField(name,property.OldType);
        }

        public void RemoveProperty(string propertyName)
        {
            if (!Properties.ContainsKey(propertyName)) throw new Exception($"{Name} : Property {propertyName} does not exist.");

            Properties.Remove(propertyName);

            _constructor.RemoveField(propertyName);
        }

       
        public NewType(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public Instance Instantiate(string[] values)
        {
            var instance = _constructor.Create(this, values);
            return instance;
        }

        public override string ToString()
        {
            return Properties.Aggregate($"{{typeName :\"{Name}\", properties : {{", (s, pair) => s + $"{pair.Key} : {pair.Value},") + "}}";
        }

        public void BuildExpression(string ruleName,string[] operations)
        {
            Rules.Add(ruleName,new Rule(operations));

        }
    }
}
