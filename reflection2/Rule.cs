using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace reflection2
{
    internal class Rule
    {
        private readonly string[] operators;

        public Rule(string[] epxression)
        {
            
            operators = epxression;
        }

        public double compute(Instance instance)
        {
            double accumulator = 0;

            Type type = instance.GetType();

            string lastOperand = "+";


            // orders of operations is not respected :(

            for (int i = 0; i < operators.Length; i+=2)
            {
                

                var fieldName = operators[i];

                double operand = 0;


                if (!Char.IsDigit(fieldName[0]))
                    operand = WalkTree(fieldName, instance);
                else operand = Double.Parse(fieldName);


                switch (lastOperand)
                {
                    case "+":
                        accumulator += operand;
                        break;
                    case "-":
                        accumulator -= operand;
                        break;
                    case "*":
                        accumulator *= operand;
                        break;
                    case "/":
                        accumulator /= operand;
                        break;
                    case "%":
                        accumulator %= operand;
                        break;
                }

                if (i + 1 < operators.Length)
                    lastOperand = operators[i + 1];





            }


            return accumulator;
        }

        private double WalkTree(string fieldName, Instance instance)
        {
            while (fieldName.Contains('.'))
            {
                var variable = fieldName.Substring(0, fieldName.IndexOf('.'));

                fieldName = fieldName.Substring(fieldName.IndexOf(".")+1);

                if (!fieldName.Contains('.'))
                {
                    return System.Convert.ToDouble((instance.GetField(variable) as Instance).GetField(fieldName));
                }
                else instance = instance.GetField(variable) as Instance;
            }

            return (double)instance.GetField(fieldName);
        }
    }
}
