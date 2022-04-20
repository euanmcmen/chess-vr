using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AsyncUnityEventSO<T> : ScriptableObject
{
    public UnityEvent<T> OnEventStarted;

    public UnityEvent OnEventFinished;

    private int eventsInProgress;
}
