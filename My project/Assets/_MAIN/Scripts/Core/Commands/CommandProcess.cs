using COMMANDS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommandProcess
{
    public Guid ID; // folyamat egyedi azonositója
    public string processName;
    public Delegate command; // folyamat, amit aparancs futtat
    public CoroutineWrapper runningProcess; // parancs végrehajtásáért felel
    public string[] args;

    public UnityEvent onTerminateAction; // esemény, ami a folyamat megszakitásakor lép életbe

    // parancs inicializálása
    public CommandProcess(Guid id, string processName, Delegate command, CoroutineWrapper runningProcess, string[] args, UnityEvent onTerminateAction = null)
    {
        ID = id;
        this.processName = processName;
        this.command = command;
        this.runningProcess = runningProcess;
        this.args = args;
        this.onTerminateAction = onTerminateAction;
    }
}
