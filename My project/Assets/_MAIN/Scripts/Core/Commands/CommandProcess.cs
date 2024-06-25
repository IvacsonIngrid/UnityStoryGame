using COMMANDS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommandProcess
{
    public Guid ID; // folyamat egyedi azonosit�ja
    public string processName;
    public Delegate command; // folyamat, amit aparancs futtat
    public CoroutineWrapper runningProcess; // parancs v�grehajt�s��rt felel
    public string[] args;

    public UnityEvent onTerminateAction; // esem�ny, ami a folyamat megszakit�sakor l�p �letbe

    // parancs inicializ�l�sa
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
