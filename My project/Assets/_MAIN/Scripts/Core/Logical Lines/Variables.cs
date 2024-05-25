using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Variables
{
    private const string DEFAULT_DATABASE_NAME = "Default";
    public const char DATABASE_VARIABLE_RELATIONAL_ID = '.';
    public static readonly string REGEX_VARIABLE_IDS = @"[!]?\$[a-zA-Z0-9_.]+";
    public const char VARIABLE_ID = '$';
    
    public class Database
    {
        public string name;
        public Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
        public Database(string name)
        {
            this.name = name;
            variables = new Dictionary<string, Variable>();
        }
    }

    public abstract class Variable
    {
        public abstract object Get();
        public abstract void Set(object value);
    }

    public class Variable<T> : Variable
    {
        private T value;
        private Func<T> get;
        private Action<T> set;

        public Variable(T defaultValue = default, Func<T> getter = null, Action<T> setter = null)
        {
            value = defaultValue;

            if (getter == null)
                this.get = () => value;
            else
                this.get = getter;

            if (setter == null)
                this.set = newValue => value = newValue;
            else
                this.set = setter;
        }

        public override object Get() => get();
        public override void Set(object newValue) => set((T)newValue);
    }

    private static Dictionary<string, Database> databases = new Dictionary<string, Database>() { { DEFAULT_DATABASE_NAME, new Database(DEFAULT_DATABASE_NAME) } };
    private static Database defaultDatabase => databases[DEFAULT_DATABASE_NAME];

    public static bool CreateDatabase(string name)
    {
        if (!databases.ContainsKey(name))
        {
            databases[name] = new Database(name);
            return true;
        }

        return false;
    }

    public static Database GetDatabase(string name)
    {
        if (name == string.Empty)
            return defaultDatabase;

        if (!databases.ContainsKey(name))
            CreateDatabase(name);

        return databases[name];
    }

    public static void PrintAllDatabase()
    {
        foreach (KeyValuePair<string, Database> keyValuePair in databases)
        {
            Debug.Log($"Database: '<color=#2AF35D>{keyValuePair.Key}</color>'");
        }
    }

    public static bool CreateVariable<T>(string name, T value, Func<T> getter = null, Action<T> setter = null)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if (db.variables.ContainsKey(variableName))
            return false;

        db.variables[variableName] = new Variable<T>(value, getter, setter);

        return true;
    }

    public static bool TryGetValue(string name, out object variable)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);
    
        if (!db.variables.ContainsKey(variableName))
        {
            variable = null;
            return false;
        }

        variable = db.variables[variableName].Get();

        return true;
    }

    public static bool TrySetValue<T>(string name, T value)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if (!db.variables.ContainsKey(variableName))
            return false;

        db.variables[variableName].Set(value);
        return true;
    }

    private static (string[], Database, string) ExtractInfo(string name)
    {
        string[] components = name.Split(DATABASE_VARIABLE_RELATIONAL_ID);
        Database db = components.Length > 1 ? GetDatabase(components[0]) : defaultDatabase;
        string variableName = components.Length > 1 ? components[1] : components[0];

        return (components, db, variableName);
    }

    public static void PrintAllVariables(Database database = null)
    {
        if (database != null)
        {
            PrintAllDatabaseVariables(database);
            return;
        }

        foreach (var d in databases)
        {
            PrintAllDatabaseVariables(d.Value);
        }    
    }

    private static void PrintAllDatabaseVariables(Database database)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"Database <color=#2AF35D>{database.name}</color>");

        foreach (KeyValuePair<string, Variable> variablePair in database.variables)
        {
            string variableName = variablePair.Key;
            object variableValue = variablePair.Value.Get();
            builder.AppendLine($"Variable - value: <color=#2AF3B0>{variableName}</color> = <color=#27DFDF>{variableValue}</color>");
        }

        Debug.Log(builder.ToString());
    }

    public static bool HasVariable(string name)
    {
        string[] parts = name.Split(DATABASE_VARIABLE_RELATIONAL_ID);
        Database db = parts.Length > 1 ? GetDatabase(parts[0]) : defaultDatabase;
        string variableName = parts.Length > 1 ? parts[1] : parts[0];

        return db.variables.ContainsKey(variableName);
    }

    public static void RemoveVariable(string name)
    {
        (string[] components, Database db, string variableName) = ExtractInfo(name);

        if (db.variables.ContainsKey(variableName))
            db.variables.Remove(variableName);
    }

    public static void RemoveAllVariables()
    {
        databases.Clear();
        databases[DEFAULT_DATABASE_NAME] = new Database(DEFAULT_DATABASE_NAME);
    }
}
