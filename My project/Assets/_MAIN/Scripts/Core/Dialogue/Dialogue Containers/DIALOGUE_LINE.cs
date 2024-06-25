using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    public class DIALOGUE_LINE
    {
        public string rawData { get; private set; } = string.Empty; // nyers adatok
        public DL_SPEAKER_DATA speakerData;
        public DL_DIALOGUE_DATA dialogueData;
        public DL_COMMAND_DATA commandData;

        // van-e: beszélő, dialogus adat, parancs szintű adat
        public bool hasSpeaker => speakerData != null; // speaker != string.Empty;
        public bool hasDialogue => dialogueData != null; //dialogue.hasDialogue;
        public bool hasCommands => commandData != null; //commands != string.Empty;

        // inicializálás - null érték adás, ha valamelyik hiányzik
        public DIALOGUE_LINE(string rawLine, string speaker, string dialogue, string commands)
        {
            rawData = rawLine;
            this.speakerData = (string.IsNullOrWhiteSpace(speaker) ? null : new DL_SPEAKER_DATA(speaker));
            this.dialogueData = (string.IsNullOrWhiteSpace(dialogue) ? null : new DL_DIALOGUE_DATA(dialogue));
            this.commandData = (string.IsNullOrWhiteSpace(commands) ? null : new DL_COMMAND_DATA(commands));
        }
    }
}