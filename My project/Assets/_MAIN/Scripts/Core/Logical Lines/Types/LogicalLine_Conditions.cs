using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Conditions;

namespace DIALOGUE.LogicalLines
{
    public class LogicalLine_Conditions : ILogicalLine
    {
        // sz�ks�ges kulcsszavak
        public string keyword => "if";
        private const string ELSE = "else";
        private readonly string[] CONTAINERS = new string[] { "(", ")" };

        // adatok kinyer�se, feldolgoz�sa, 2 r�szre bont�sa, logikai �llit�s ki�rt�kel�se
        public IEnumerator Execute(DIALOGUE_LINE line)
        {
            string rawCondition = ExtractCondition(line.rawData.Trim());
            bool conditionResult = EvaluateCondition(rawCondition);

            Conversation currentConversation = DialogueSystem.instance.conversationManager.conversation;
            int currentProgress = DialogueSystem.instance.conversationManager.conversationProgress;

            EncapsulatedData ifData = RipEncapsulatedData(currentConversation, currentProgress, false);
            EncapsulatedData elseData = new EncapsulatedData();

            if (ifData.endingIndex + 1 < currentConversation.Count)
            {
                string nextLine = currentConversation.GetLines()[ifData.endingIndex + 1].Trim();
                if (nextLine == ELSE)
                {
                    elseData = RipEncapsulatedData(currentConversation, ifData.endingIndex + 1, false);
                    ifData.endingIndex = elseData.endingIndex;
                }
            }

            currentConversation.SetProgress(ifData.endingIndex);

            EncapsulatedData selData = conditionResult ? ifData : elseData;
            if (!selData.isNull && selData.lines.Count > 0)
            {
                Conversation newConversation = new Conversation(selData.lines);
                DialogueSystem.instance.conversationManager.EnqueuePriority(newConversation);
            }

            yield return null;
        }

        // val�ban egyezik-e a keresett kulcssz�val?
        public bool Matches(DIALOGUE_LINE line)
        {
            return line.rawData.Trim().StartsWith(keyword);
        }

        // blokkok meghat�roz�sa
        private string ExtractCondition(string line)
        {
            int startIndex = line.IndexOf(CONTAINERS[0]) + 1;
            int endIndex = line.IndexOf(CONTAINERS[1]);

            return line.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}