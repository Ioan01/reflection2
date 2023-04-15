using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reflection2
{
    internal static class TypeHandler
    {
        private static Dictionary<string,NewType> types = new Dictionary<string,NewType>();
        public static void CreateType(string typename)
        {
            if (Type.GetType("typename", false) != null)
                throw new Exception("Cannot create reserved types.");

            if (types.ContainsKey(typename))
                throw new Exception($"Redefinition of {typename}.");

            types.Add(typename,new NewType(typename));
        }

        public static void RemoveProperty(string typename, string propertyName)
        {
            if (!types.ContainsKey(typename)) throw new Exception($"Type {typename} does not exist");

            types[typename].RemoveProperty(propertyName);

            InstanceHandler.RemoveInstancesProperty(typename,propertyName);

        }

        public static NewType GetNewType(string typename)
        {
            if (types.ContainsKey(typename))
                return types[typename];
            return null;
        }

        public static void AddProperty(string typename,string propertyName,string propertyType)
        {
            if (!types.ContainsKey(typename)) throw new Exception($"Type {typename} does not exist.");

            Property property;

            var primitive = Type.GetType(propertyType);
            if (primitive != null)
                property = new Property(primitive);
            else
            {
                if (!types.ContainsKey(typename))
                {
                    throw new Exception($"Cannot add property of type {propertyType} to {typename} : {propertyType} does not exist");
                }

                property = new Property(types[propertyType]);
            }


            types[typename].AddProperty(propertyName,property);

            InstanceHandler.AddIntancesProperty(typename,propertyName,property.GetDefaultValue());

        }

        public static void PrintTypes()
        {
            foreach (var (key, value) in types)
            {
                Console.WriteLine(value);
            }
        }


        public static void AddExpression(string typename, string name,string epxression)
        {
            if (!types.ContainsKey(typename))
                throw new Exception($"Type {typename} not found.");
            else types[typename].BuildExpression(name,epxression.Split(" "));
        }
    }
}
