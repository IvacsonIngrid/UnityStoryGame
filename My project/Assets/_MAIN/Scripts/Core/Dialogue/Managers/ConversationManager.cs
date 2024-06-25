using CHARACTERS;
using COMMANDS;
using DIALOGUE.LogicalLines;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DIALOGUE
{
    public class ConversationManager
    {
        private DialogueSystem dialogueSystem => DialogueSystem.instance;   
        private Coroutine process = null; // akt. párbeszéd kezelésére
        public bool isRunning => process != null; // jelenleg fut-e párbeszéd

        public TextArchitect architect = null; // párbeszéd szövegének megjelenitéséért

        private bool userPrompt = false; // felhasználói bemenetet jelez

        private LogicalLineManager logicalLineManager;
        public Conversation conversation => (conversationQueue.IsEmpty() ? null : conversationQueue.top); // jelenlegi párbeszéd

        public int conversationProgress => (conversationQueue.IsEmpty() ? -1 : conversationQueue.top.GetProgress()); // akt. párbeszéd előrehaladása
        private ConversationQueue conversationQueue; //  párbeszédeket kezeli

        public bool allowUserPrompt = true; // engedélyezett-e a felh. bemenet
        public ConversationManager(TextArchitect architect)
        {
            this.architect = architect;
            dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next;

            logicalLineManager = new LogicalLineManager();
            conversationQueue = new ConversationQueue();
        }

        public void Enqueue(Conversation conversation) => conversationQueue.Enqueue(conversation); // párbeszéd hozzáadása sorhoz
        public void EnqueuePriority(Conversation conversation) => conversationQueue.EnqueuePriority(conversation); // sor elejéhez ad hozzá

        private void OnUserPrompt_Next() // felh. bemenet kezelése
        {
            if (allowUserPrompt)
                userPrompt = true;
        }
        
        public Coroutine StartConversation(Conversation conversation) // párbeszéd indul
        {
            StopConversation();

            conversationQueue.Clear();

            Enqueue(conversation);

            process = dialogueSystem.StartCoroutine(RunningConversation());

            return process;
        }

        public void StopConversation() // párbeszéd leáll
        {
            if (!isRunning)
                return;

            dialogueSystem.StopCoroutine(process);
            process = null;
        }

        IEnumerator RunningConversation() // párbeszéd futását kezeli
        {
            while (!conversationQueue.IsEmpty())
            {
                Conversation currentConversation = conversation;

                if (currentConversation.HasReachedEnd())
                {
                    conversationQueue.Dequeue();
                    continue;
                }

                string rawLine = currentConversation.CurrentLine();

                //ne legyen szokoz
                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    TryAdvanceConversation(currentConversation);
                    continue;
                }

                DIALOGUE_LINE line = DialogueParser.Parse(rawLine);

                if (logicalLineManager.TryGetLogic(line, out Coroutine logic))
                {
                    yield return logic;
                }
                else
                {
                    //dialogus megmuatatasa
                    if (line.hasDialogue)
                        yield return Line_RunDialogue(line);

                    //run commands
                    if (line.hasCommands)
                        yield return Line_RunCommands(line);

                    //wait for user input if we had dialogue line
                    if (line.hasDialogue)
                    {
                        //wait for user input
                        yield return WaitForUserInput();
                        CommandManager.instance.StopAllProcesses();
                        dialogueSystem.OnSystemPrompt_Clear();
                    }
                }

                TryAdvanceConversation(currentConversation);
            }

            process = null;
        }

        private void TryAdvanceConversation(Conversation conversation)
        {
            conversation.IncrementProgress();

            if (conversation != conversationQueue.top)
                return;

            if (conversation.HasReachedEnd())
                conversationQueue.Dequeue();
        }

        IEnumerator Line_RunDialogue(DIALOGUE_LINE line) // párbeszéd sorának kezelése
        {
            //the speaker
            /*if (line.hasDialogue)
                HandleSpeakerLogic(line.speakerData);*/

            //the speaker
            if (line.hasSpeaker)
                HandleSpeakerLogic(line.speakerData);

            if (!dialogueSystem.dialogueContainer.isVisible)
                dialogueSystem.dialogueContainer.Show();

            //build dialogue
            yield return BuildLineSegments(line.dialogueData);
        }

        private void HandleSpeakerLogic(DL_SPEAKER_DATA speakerData) // beszélő logikájának kezelése
        {
            bool characterMustBeCreated = (speakerData.makeCharacterEnter || speakerData.isCastingPosition || speakerData.isCastingExpressions);

            Character character = CharacterManager.instance.GetCharacter(speakerData.name, createIfDoesNotExist: characterMustBeCreated);

            if (speakerData.makeCharacterEnter && (!character.isVisible && !character.isRevealing))
                character.Show();

            //neve
            dialogueSystem.ShowSpeakerName(TagManager.Inject(speakerData.displayname));

            //karakter szövegének tipusa
            DialogueSystem.instance.ApplySpeakerDataToDialogueContainer(speakerData.name);

            if (speakerData.isCastingPosition)
                character.MoveToPosition(speakerData.castPosition);

            //kifejezések
            if(speakerData.isCastingExpressions)
            {
                foreach (var exp in speakerData.CastExpressions)
                    character.OnReceiveCastingExpression(exp.layer, exp.expression);
            }
        }

        IEnumerator Line_RunCommands(DIALOGUE_LINE line) // parancsok futtatása
        {
            List<DL_COMMAND_DATA.Command> commands = line.commandData.commands;

            foreach(DL_COMMAND_DATA.Command command in commands)
            {
                if (command.waitForCompletion || command.name == "wait")
                {
                    CoroutineWrapper cw = CommandManager.instance.Execute(command.name, command.arguments);
                    while(!cw.IsDone)
                    {
                        if (userPrompt)
                        {
                            CommandManager.instance.StopCurrentProcess();
                            userPrompt = false;
                        }
                        yield return null;
                    }
                }
                else
                    CommandManager.instance.Execute(command.name, command.arguments);
            }

            yield return null;
        }

        IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line) //  szegmensek összeállitása
        {
            for (int i = 0; i < line.segments.Count; i++)
            {
                DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment = line.segments[i];

                yield return WaitForDialogueSegmentSignalToBeTriggered(segment);

                yield return BuildDialogue(segment.dialogue, segment.appendText);
            }
        }

        public bool isWaitingOnAutoTimer { get; private set; } = false;

        IEnumerator WaitForDialogueSegmentSignalToBeTriggered(DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment)
        {
            switch(segment.startSignal)
            {
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.C:
                    yield return WaitForUserInput();
                    dialogueSystem.OnSystemPrompt_Clear();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WC:
                    isWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    isWaitingOnAutoTimer = false;
                    dialogueSystem.OnSystemPrompt_Clear();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WA:
                    isWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    isWaitingOnAutoTimer = false;
                    break;
                default:
                    break;
            }
        }

        IEnumerator BuildDialogue(string dialogue, bool append = false) // párbeszéd összeállitása
        {
            dialogue = TagManager.Inject(dialogue);

            //build the dialogue
            if (!append)
                architect.Build(dialogue);
            else
                architect.Append(dialogue);

            //wait for the dialogue to complete
            while (architect.isBuilding)
            {
                if (userPrompt)
                {
                    if (!architect.hurryUp)
                        architect.hurryUp = true;
                    else
                        architect.ForceComplete();

                    userPrompt = false;
                }
                
                yield return null;
            }
        }

        IEnumerator WaitForUserInput() // felh. bemenetre vár
        {
            dialogueSystem.prompt.Show();

            while (!userPrompt)
                yield return null;

            dialogueSystem.prompt.Hide();

            userPrompt = false;
        }
    }
}