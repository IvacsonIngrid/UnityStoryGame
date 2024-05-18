using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

public class TagManager
{
    private readonly Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>();
    private readonly Regex tagRegex = new Regex("<\\w+>");

    public TagManager() 
    {
        InitializeTags();
    }

    private void InitializeTags()
    {
        tags["<mainChar>"] = () => "Avira";
        tags["<time>"] = () => DateTime.Now.ToString("hh:mm tt");
        tags["<playerLevel>"] = () => "15";
        tags["<input>"] = () => InputPanel.instance.lastInput;
        tags["<tempVall>"] = () => "42";
    }

    public string Inject(string text)
    {
        if (tagRegex.IsMatch(text))
        {
            foreach (Match match in tagRegex.Matches(text))
            {
                if (tags.TryGetValue(match.Value, out var tagValueRequest))
                {
                    text = text.Replace(match.Value, tagValueRequest());
                }
            }
        }

        return text;
    }
}
