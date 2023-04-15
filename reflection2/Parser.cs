using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace reflection2
{
    internal static class Parser
    {
        private static readonly Regex createRegex = new Regex("create type (\\w+)",RegexOptions.Compiled);
        private static readonly Regex addRegex = new Regex("add to (\\w+) (\\w+) (\\w+)",RegexOptions.Compiled);
        private static readonly Regex ruleRegex = new Regex("add rule to (\\w+) (\\w+)\\s*=\\s*([\\w+\\-*\\/*%. \\d]+)", RegexOptions.Compiled);
        private static readonly Regex removeRegex = new Regex("remove from (\\w+) (\\w+)", RegexOptions.Compiled);


        private static readonly Regex instantiationRegex = new Regex("(\\w+)\\s+(\\w+)", RegexOptions.Compiled);
        private static readonly Regex setRegex = new Regex("(\\w+).([\\w.]+)\\s*=\\s*(\"*\\w+.\\w*\"*)", RegexOptions.Compiled);

        private static int lineNumber = 1;


        private static Dictionary<string, string> _typeAliases = new Dictionary<string, string>()
        {
            {"string","System.String"},
            {"number","System.Double"},
            {"integer","System.Int32"}
        };

        private static Match Match(Regex regex, string line)
        {
            var match = regex.Match(line);

            if (match.Success)
            {
                return match;
            }

            throw new Exception($"Invalid syntax at {Parser.lineNumber} : {line}");
        }




        public static void InterpretScript(string script)
        {

            lineNumber = 1;
            if (string.IsNullOrEmpty(script)) {  return; }
            
            var lines = script.Split('\n').Select(l=>l.Trim());

            
            
            foreach (var line in lines)
            {
                InterpetLine(line);
                Parser.lineNumber++;
            }

        }

        private static void InterpetLine(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            if (line.StartsWith("create"))
            {
                CreateType(line);
            }
            else if (line.StartsWith("add"))
            {
                if (line.Contains("rule"))
                    CreateRule(line);
                else AddProperty(line);
            }
            else if (line.StartsWith("remove"))
            {
                RemoveProperty(line);
            }
            else if (line.StartsWith("print"))
            {
                if (line.Contains("types"))
                    TypeHandler.PrintTypes();
                else if (line.Contains("instances"))
                    InstanceHandler.PrintAll();
                else InstanceHandler.PrintInstance(line.Substring(line.IndexOf(' ') + 1));
            }
            else if (line.Contains('.'))
            {
                SetProperty(line);
            }
            else Instantiate(line);
        }

        private static void Instantiate(string line)
        {
            var match = Match(instantiationRegex, line);

            InstanceHandler.Instantiate(match.Groups[2].Value, match.Groups[1].Value,null);
        }

        private static void SetProperty(string line)
        {
            double doubleValue = 0;

            var match = Match(setRegex, line);

            var root = match.Groups[1].Value;

            var property = match.Groups[2].Value;

            var value = match.Groups[3].Value;

            var instance = InstanceHandler.GetInstance(root);
            if (instance == null)
            {
                throw new Exception($"{root} not declared. : {line}");
            }


            while (property.Contains('.'))
            {
                instance = (Instance)instance.GetField(property.Substring(0, property.IndexOf('.')));
                property = property.Substring(property.IndexOf(".")+1);
            }


            if (value.Contains("\""))
            {
                instance.SetField(property,value.Substring(1,value.Length-2));
            }
            else if (Double.TryParse(value, out doubleValue))
            {
                instance.SetField(property, doubleValue);

            }
            else
            {
                var childInstance = InstanceHandler.GetInstance(value);
                if (childInstance == null)
                {
                    throw new Exception($"{value} not declared. : {line}");
                }
                instance.SetField(property,childInstance);
            }


        }

        private static void RemoveProperty(string line)
        {
            var match = Match(removeRegex, line);
            TypeHandler.RemoveProperty(match.Groups[1].Value, match.Groups[2].Value);
        }

        private static void AddProperty(string line)
        {
            var match = Match(addRegex, line);


            var type = match.Groups[2].Value;

            if (_typeAliases.ContainsKey(type))
                type = _typeAliases[type];

            TypeHandler.AddProperty(match.Groups[1].Value, match.Groups[3].Value, type);
        }

        
        private static void CreateRule(string line)
        {
            var match = Match(ruleRegex, line);
            TypeHandler.AddExpression(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
        }

        private static void CreateType(string line)
        {
            var match = Match(createRegex, line);
            TypeHandler.CreateType(match.Groups[1].Value);

        }
    }
}
