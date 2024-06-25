using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    public class CommandDatabase
    {
        private Dictionary<string, Delegate> database = new Dictionary<string, Delegate>(); // tárolja a parancs nevét és függvényreferenciát

        // létezik-e az adott parancs az adatbázisban
        public bool HasCommand(string commandName) => database.ContainsKey(commandName.ToLower());

        // új parancs hozzáadása az adatbázishoz
        public void AddCommand(string commandName, Delegate command)
        {
            commandName = commandName.ToLower();
            
            if (!HasCommand(commandName)) // ellenőrzi, ne legyen még a része
            {
                database.Add(commandName, command); // hozzáadas
            }
            else
                Debug.Log($"Command already exists in the database '{commandName}'"); // lekezeli, jelzi, ha már a része
        }

        // függvényreferencia lekérése
        public Delegate GetCommand(string commandName)
        {
            commandName = commandName.ToLower();

            if (!HasCommand(commandName))
            {
                Debug.Log($"Command '{commandName}' does not exist in the database!"); // ha nem találja - hibakezelés
                return null;
            }

            return database[commandName];
        }
    }
}