using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Expressions;

namespace DIALOGUE.LogicalLines
{
    public class LogicalLineOperators : ILogicalLine
    {
        public string keyword => throw new System.NotImplementedException();

        public IEnumerator Execute(DIALOGUE_LINE line)
        {
            string trimitedLine = line.rawData.Trim();
            string[] parts = Regex.Split(trimitedLine, REGEX_ARITHMATIC);

            if (parts.Length < 3)
            {
                Debug.LogError($"Invalid command: {trimitedLine}");
                yield break;
            }

            string variable = parts[0].Trim().TrimStart(Variables.VARIABLE_ID);
            string op = parts[1].Trim();
            string[] remainingParts = new string[parts.Length - 2];

            Array.Copy(parts, 2, remainingParts, 0, parts.Length - 2);

            object value = CalculateValue(remainingParts);

            if (value == null)
                yield break;

            ProcessOperator(variable, op, value);
        }

        private void ProcessOperator(string variable, string op, object value)
        {
            if (Variables.TryGetValue(variable, out object currentValue))
            {
                ProcessOperatorOnVariable(variable, op, value, currentValue);
            }
            else if (op == "=")
            {
                Variables.CreateVariable(variable, value);
            }
        }

        private void ProcessOperatorOnVariable(string variable, string op, object value, object currentValue)
        {
            switch (op)
            {
                case "=":
                    Variables.TrySetValue(variable, value);
                    break;
                case "+=":
                    Variables.TrySetValue(variable, ConcatenateOrAdd(value, currentValue));
                    break;
                case "-=":
                    Variables.TrySetValue(variable, Convert.ToDouble(currentValue) - Convert.ToDouble(value));
                    break;
                case "*=":
                    Variables.TrySetValue(variable, Convert.ToDouble(currentValue) * Convert.ToDouble(value));
                    break;
                case "/=":
                    Variables.TrySetValue(variable, Convert.ToDouble(currentValue) / Convert.ToDouble(value));
                    break;
                default:
                    Debug.LogError($"Invalid operator: {op}");
                    break;
            }
        }

        private object ConcatenateOrAdd(object value, object currentValue)
        {
            if (value is string)
                return currentValue.ToString() + value;

            return Convert.ToDouble(currentValue) + Convert.ToDouble(value);
        }

        public bool Matches(DIALOGUE_LINE line)
        {
            Match match = Regex.Match(line.rawData.Trim(), REGEX_OPERATOR_LINE);
            return match.Success;
        }
    }
}
