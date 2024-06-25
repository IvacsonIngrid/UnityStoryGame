using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using UnityEngine.Events;
using Unity.VisualScripting;
using CHARACTERS;

namespace COMMANDS
{
    public class CommandManager : MonoBehaviour
    {
        private const char SUB_COMMAND_IDENTIFIER = '.'; // alparancsokat elválasztó karakter
        public const string DATABASE_CHARACTERS_BASE = "characters";
        public const string DATABASE_CHARACTERS_SPRITE = "characters_sprite";
        public static CommandManager instance { get; private set; } // Singleton példány
        private CommandDatabase database; // parancsok adatbázisa
        private Dictionary<string, CommandDatabase> subDatabase = new Dictionary<string, CommandDatabase>(); // alparancsok tárolására

        private List<CommandProcess> activeProcess = new List<CommandProcess>(); // aktiv parancsok listája

        private CommandProcess topProcess => activeProcess.Last();

        private void Awake() // inicializálás
        {
            if (instance == null)
            {
                instance = this;

                database = new CommandDatabase();

                Assembly assembly = Assembly.GetExecutingAssembly();
                Type[] extensionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CMD_DatabaseExtension))).ToArray();

                foreach (Type extension in extensionTypes)
                {
                    MethodInfo extendMethod = extension.GetMethod("Extend");
                    extendMethod.Invoke(null, new object[] { database });
                }
            }
            else
                DestroyImmediate(gameObject);
        }

        public CoroutineWrapper Execute(string commandName, params string[] args)
        {
            if (commandName.Contains(SUB_COMMAND_IDENTIFIER)) // ha van alparancs is, akkor tovább adja, máshol van lekezelve
                return ExecuteSubCommand(commandName, args);
            
            Delegate command = database.GetCommand(commandName); // parancs azonositása

            if (command == null)
                return null;

            return StartProcess(commandName, command, args); // parancs végrehajtása
        }

        private CoroutineWrapper ExecuteSubCommand(string commandName, string[] args)
        {
            string[] parts = commandName.Split(SUB_COMMAND_IDENTIFIER);
            string databaseName = string.Join(SUB_COMMAND_IDENTIFIER, parts.Take(parts.Length - 1));
            string subCommandName = parts.Last();

            if (subDatabase.ContainsKey(databaseName)) // ellenőrzi, hogy az alparancs része-e az aladatbázisnak
            {
                Delegate command = subDatabase[databaseName].GetCommand(subCommandName); // alparancs lekérése
                if (command != null)
                {
                    return StartProcess(commandName, command, args); // végrehajtás
                }
                else
                {
                    Debug.LogError($"No command called '{subCommandName}' was found in sub database '{databaseName}'");
                    return null;
                }
            }

            string characterName = databaseName;
            // megprobálja karakróter parancsként futtatni
            if (CharacterManager.instance.HasCharacter(databaseName))
            {
                List<string> newArgs = new List<string>(args);
                newArgs.Insert(0, characterName);
                args = newArgs.ToArray();

                return ExecuteCharacterCommand(subCommandName, args);
            }

            Debug.LogError($"No sub database called '{databaseName}' exists! Command '{subCommandName}' could not be run.");
            return null;
        }

        // karakterparancs végrehajtása
        private CoroutineWrapper ExecuteCharacterCommand(string commandName, params string[] args)
        {
            Delegate command = null;

            CommandDatabase db = subDatabase[DATABASE_CHARACTERS_BASE];
            if(db.HasCommand(commandName))
            {
                command = db.GetCommand(commandName);
                return StartProcess(commandName, command, args);
            }

            CharacterConfigData characterConfigData = CharacterManager.instance.GetCharacterConfig(args[0]);
            switch (characterConfigData.characterType)
            {
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    db = subDatabase[DATABASE_CHARACTERS_SPRITE];
                    break;
            }

            command = db.GetCommand(commandName);

            if (command != null)
                return StartProcess(commandName, command, args);

            Debug.LogError($"Command Manager was unable to execute command '{commandName}' on character '{args[0]}'. The character name or command may be invalid.");
            return null;
        }

        // új folyamat inditása, hozzáadni az aktivak listájához
        private CoroutineWrapper StartProcess(string commandName, Delegate command, string[] args)
        {
            System.Guid processID = System.Guid.NewGuid();
            CommandProcess cmd = new CommandProcess(processID, commandName, command, null, args, null);
            activeProcess.Add(cmd);

            Coroutine co = StartCoroutine(RunningProcess(cmd));
            cmd.runningProcess = new CoroutineWrapper(this, co);
            
            return cmd.runningProcess;
        }

        // jelenlegi aktiv folyamat megszakitása
        public void StopCurrentProcess()
        {
            if (topProcess != null)
                KillProcess(topProcess);
        }

        // minden aktiv folyamat leállitása, az aktivak listájának kiüritése
        public void StopAllProcesses()
        {
            foreach (var c in activeProcess)
            {
                if (c.runningProcess != null && !c.runningProcess.IsDone)
                    c.runningProcess.Stop();

                c.onTerminateAction?.Invoke();
            }

            activeProcess.Clear();
        }

        private IEnumerator RunningProcess(CommandProcess process) // adott parancs futtatása
        {
            yield return WaitingForProcessToComplete(process.command, process.args);

            KillProcess(process);
        }

        public void KillProcess(CommandProcess cmd) // adott parancs megszakitása
        {
            activeProcess.Remove(cmd);

            if (cmd.runningProcess != null && !cmd.runningProcess.IsDone)
                cmd.runningProcess.Stop();

            cmd.onTerminateAction?.Invoke();
        }

        private IEnumerator WaitingForProcessToComplete(Delegate command, string[] args)
        {
            if (command is Action)
                command.DynamicInvoke();

            else if (command is Action<string>)
                command.DynamicInvoke(args.Length == 0 ? string.Empty : args[0]);

            else if (command is Action<string[]>)
                command.DynamicInvoke((object)args);

            else if (command is Func<IEnumerator>)
                yield return ((Func<IEnumerator>)command)();

            else if (command is Func<string, IEnumerator>)
                yield return ((Func<string, IEnumerator>)command)(args.Length == 0 ? string.Empty : args[0]);

            else if (command is Func<string[], IEnumerator>)
                yield return ((Func<string[], IEnumerator>)command)(args);
        }

        // befejező lépés hozzáadása a jelenlegi folyamathoz
        public void AddTerminationActionToCurrentProcess(UnityAction action)
        {
            CommandProcess process = topProcess;

            if (process == null)
                return;

            process.onTerminateAction = new UnityEvent();
            process.onTerminateAction.AddListener(action);
        }

        // alparancsok számára adatbáis létrehozása
        public CommandDatabase CreateSubDatabase(string name)
        {
            name = name.ToLower();

            if (subDatabase.TryGetValue(name, out CommandDatabase db))
            {
                Debug.LogWarning($"A database by the name of '{name}' already exists!");
                return db;
            }

            CommandDatabase newDatabase = new CommandDatabase();
            subDatabase.Add(name, newDatabase);

            return newDatabase;
        }
    }
}