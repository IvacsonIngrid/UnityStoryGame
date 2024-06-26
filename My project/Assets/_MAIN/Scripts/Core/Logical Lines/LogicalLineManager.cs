﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DIALOGUE.LogicalLines
{
    public class LogicalLineManager
    {
        private DialogueSystem dialogueSystem => DialogueSystem.instance;
        private List<ILogicalLine> logicalLines = new List<ILogicalLine>();
        public LogicalLineManager() => LoadLogicalLines(); // betölti az összes elérhető ILogicalLine implementációt

        // a tipusok létrehozása, hozzáadása
        private void LoadLogicalLines()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] lineTypes = assembly.GetTypes()
                                       .Where(t => typeof(ILogicalLine).IsAssignableFrom(t) && !t.IsInterface)
                                       .ToArray();

            foreach (Type lineType in lineTypes)
            {
                ILogicalLine line = (ILogicalLine)Activator.CreateInstance(lineType);
                logicalLines.Add(line);
            }
        }
        public bool TryGetLogic(DIALOGUE_LINE line, out Coroutine coroutine)
        {
            foreach (var logicalLine in logicalLines) // a betöltött implementációk elemein dolgozik
            {
                if (logicalLine.Matches(line))
                {
                    coroutine = dialogueSystem.StartCoroutine(logicalLine.Execute(line)); // végrehajtás elinditása
                    return true;
                }
            }
            
            coroutine = null;
            return false;
        }
    }
}