using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameProcessorEventScript : MonoBehaviour
{
    public UnityEvent<ChessTurn> OnTurnParsed;

    private int eventsInProgress;

    public bool IsFinished => eventsInProgress == 0;

    // Start is called before the first frame update
    void Awake()
    {
    }

    public void DispatchEvent(ChessTurn chessTurn)
    {
        eventsInProgress = OnTurnParsed.GetPersistentEventCount();

        OnTurnParsed.Invoke(chessTurn);
    }

    public void HandleEventFinished()
    {
        eventsInProgress--;
    }
}
