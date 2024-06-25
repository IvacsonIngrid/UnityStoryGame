using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DIALOGUE
{
    public class DL_COMMAND_DATA
    {
        public List<Command> commands;
        private const char COMMANDSPLITTER_IO = ',';
        private const char ARGUMENTSCONTAINER_IO = '(';
        private const string WAITCOMMAND_ID = "[wait]";

        // parancs felépjtése
        public struct Command
        {
            public string name;
            public string[] arguments;
            public bool waitForCompletion; // végrehajtás után kell-e várni
        }

        // inicializálás
        public DL_COMMAND_DATA(string rawCommands)
        {
            commands = RipCommands(rawCommands);
        }

        // parancsokra bontja a nyers anyagot
        private List<Command> RipCommands(string rawCommands)
        {
            string[] data = rawCommands.Split(COMMANDSPLITTER_IO, System.StringSplitOptions.RemoveEmptyEntries); // vessző alapján tagol
            List<Command> result = new List<Command>();

            foreach (string cmd in data)
            {
                Command command = new Command(); // minden tagolt rész átalakul Command struktúrává, úgy kerül a listába
                int index = cmd.IndexOf(ARGUMENTSCONTAINER_IO);
                command.name = cmd.Substring(0, index).Trim();

                if (command.name.ToLower().StartsWith(WAITCOMMAND_ID)) // kell-e várni a parancs lefutása után
                {
                    command.name = command.name.Substring(WAITCOMMAND_ID.Length);
                    command.waitForCompletion = true;
                }
                else
                    command.waitForCompletion = false;

                command.arguments = GetArgs(cmd.Substring(index + 1, cmd.Length - index - 2)); // paraméterek lekérdezése
                result.Add(command);
            }
            return result;
        }

        // paramétereket dolgozza fel
        private string[] GetArgs(string args)
        {
            List<string> argList = new List<string>(); // lista a paramétereknek
            StringBuilder currentArg = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == '"') // lehetséges argumentum jön
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (!inQuotes && args[i] == ' ') // lehetséges argumentum jön
                {
                    argList.Add(currentArg.ToString());
                    currentArg.Clear();
                    continue;
                }

                currentArg.Append(args[i]); // argumentumok gyűjtése
            }

            if (currentArg.Length > 0)
                argList.Add(currentArg.ToString());

            return argList.ToArray();
        }
    }
}