﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Linq;

public static class GlobalVariables
{
    public static int SharedVariable = 0;
}

public class TagManager
{
    private static readonly Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>() // tagork és azok értéke
    {
        { "<mainChar>", () => "Avira" },
        { "<time>", () => DateTime.Now.ToString("hh:mm tt") },
        { "<playerLevel>", () => "15" },
        {  "<input>", () => InputPanel.instance.lastInput  },
        { "<tempVall>", () => "42" }
    };

    private static readonly Regex tagRegex = new Regex("<\\w+>"); // regulțris kif. ayonositțsa sy0vegben

    public static string Inject(string text, bool injectTags = true, bool injectVariables = true) // behelyettesit a szövegbe
    {
        if (injectTags)
            text = InjectTags(text);

        if (injectVariables)
            text = InjectVariables(text);

        return text;
    }
    private static string InjectTags(string value) // kicseréli megfelelő értékkel
    {
        if (tagRegex.IsMatch(value))
        {
            foreach (Match match in tagRegex.Matches(value))
            {
                if (tags.TryGetValue(match.Value, out var tagValueRequest))
                {
                    value = value.Replace(match.Value, tagValueRequest());
                }
            }
        }

        return value;
    }

    private static string InjectVariables(string value)
    {
        var matches = Regex.Matches(value, Variables.REGEX_VARIABLE_IDS);
        var matchesList = matches.Cast<Match>().ToList();

        for (int i = matchesList.Count - 1; i >= 0; i--)
        {
            var match = matchesList[i];
            string variableName = match.Value.TrimStart(Variables.VARIABLE_ID, '!');
            bool negate = match.Value.StartsWith('!');

            bool endsInIllegalCharacter = variableName.EndsWith(Variables.DATABASE_VARIABLE_RELATIONAL_ID);
            if (endsInIllegalCharacter)
                variableName = variableName.Substring(0, variableName.Length - 1);

            if (!Variables.TryGetValue(variableName, out object variableValue))
            {
                Debug.LogError($"Variable {variableName} not found in string");
                continue;
            }

            if (negate && variableValue is bool)
                variableValue = !(bool)variableValue;

            int lengthBeRemoved = match.Index + match.Length > value.Length ? value.Length - match.Index : match.Length;
            if (endsInIllegalCharacter)
                lengthBeRemoved -= 1;

            value = value.Remove(match.Index, lengthBeRemoved);
            value = value.Insert(match.Index, variableValue.ToString());
        }

        return value;
    }
}
