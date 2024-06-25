using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class CoroutineWrapper
{
    private MonoBehaviour owner; // a leállitáshoz kell
    private Coroutine coroutine; // futó példány

    public bool IsDone = false; // jelzi, mikor fejeződött be a coroutine
    public CoroutineWrapper(MonoBehaviour owner, Coroutine coroutine) // új példány létrehozása
    {
        this.owner = owner;
        this.coroutine = coroutine;
    }

    public void Stop() // coroutine leáll, jelzés beállitása
    {
        owner.StopCoroutine(coroutine);
        IsDone = true;
    }
}