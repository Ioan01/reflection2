using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reflection2
{
    internal static class InstanceHandler
    {
        private static Dictionary<string, Instance?> instances = new Dictionary<string, Instance?>();
        private static Dictionary<string, List<Instance>> instancesByType = new Dictionary<string, List<Instance>>();


        public static void Instantiate(string varName,string typename, string[] values)
        {
            var newType = TypeHandler.GetNewType(typename);
            if (newType == null) throw new Exception($"Type {typename} does not exist.");

            var instance = newType.Instantiate(values);
            instance.FillIn();

            instances.Add(varName,instance);

            if (instancesByType.ContainsKey(typename) )
                instancesByType[typename].Add(instance);
            else instancesByType.Add(typename,new List<Instance>(new []{instance}));
        }

        public static void AddIntancesProperty(string typename, string propertyName,object val)
        {
            if (instancesByType.ContainsKey(typename))
                instancesByType[typename].ForEach(instance => instance.AddField(propertyName,val));
        }

        public static void RemoveInstancesProperty(string typename,string name)
        {
            if (instancesByType.ContainsKey(typename))
                instancesByType[typename].ForEach(instance => instance.RemoveProperty(name));
        }


        public static Instance? GetInstance(string name)
        {
            if (instances.ContainsKey(name)) return instances[name];
            return null;
        }

        public static void PrintAll()
        {
            foreach (var (key, value) in instances)
            {
                Console.WriteLine($"{key} : {value}");
            }
        }

        public static void PrintInstance(string substring)
        {
            if (!instances.ContainsKey(substring))
                throw new Exception($"{substring} not an instance of any type");

            Console.WriteLine(instances[substring]);
        }
    }
}
