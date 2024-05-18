using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE.LogicalLines
{
    public class LogicalLineInput : ILogicalLine
    {
        public string keyword => "input";
        public IEnumerator Execute(DIALOGUE_LINE line)
        {
            //throw new System.NotImplementedException();

            string title = line.dialogueData.rawData;
            InputPanel panel = InputPanel.instance;
            panel.Show(title);

            while (panel.isWaitingOnUserInput)
                yield return null;
        }

        public bool Matches(DIALOGUE_LINE line)
        {
            //throw new System.NotImplementedException ();
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }
    }
}