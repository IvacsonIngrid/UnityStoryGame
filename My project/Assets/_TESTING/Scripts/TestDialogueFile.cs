using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDialogueFile : MonoBehaviour
{
    [SerializeField] private TextAsset fileToReload = null;
    void Start()
    {
        StartConversation();
    }

    void StartConversation()
    {
        List<string> lines = FileManager.ReadTextAsset(fileToReload);

        /*foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
                continue;
            
            Debug.Log($"Segments line '{line}'");
            DIALOGUE_LINE dlLine = DialogueParser.Parse(line);

            int i = 0;
            foreach (DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment in dlLine.dialogue.segments)
            {
                Debug.Log($"Segment [{i++}] = '{segment.dialogue}' [signal={segment.startSignal.ToString()}{(segment.signalDelay > 0 ? $" {segment.signalDelay}" : $"")}]");
            }
        }*/

        /*for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line))
                continue;

            DIALOGUE_LINE dl = DialogueParser.Parse(line);
            Debug.Log($"{dl.speaker.name} as [{(dl.speaker.castName != string.Empty ? dl.speaker.castName : dl.speaker.name)}]at {dl.speaker.castName}");

            List<(int l, string ex)> expr = dl.speaker.CastExpressions;
            for (int c = 0; c < expr.Count; c++)
            {
                Debug.Log($"[Layer[{expr[c].l}] = '{expr[c].ex}']");
            }
        }*/

        /*foreach(string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            DIALOGUE_LINE dl = DialogueParser.Parse(line);

            for (int i = 0; i < dl.commandData.commands.Count; i++)
            {
                DL_COMMAND_DATA.Command command = dl.commandData.commands[i];
                Debug.Log($"Command [{i}] '{command.name}' has arguments [{string.Join(", ", command.arguments)}]");
            }
        }*/

        
        DialogueSystem.instance.Say(lines);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            DialogueSystem.instance.dialogueContainer.Hide();

        else if (Input.GetKeyDown(KeyCode.UpArrow))
            DialogueSystem.instance.dialogueContainer.Show();
    }
}
