using UnityEngine;
using UnityEngine.Events;

public class GameScreenScript : MonoBehaviour
{
    public UnityEvent OnTurnFinished;

    public void HandleTurnParsedEvent(ChessTurn chessTurn)
    {
        Debug.LogFormat("Turn handled: {0}. {1} {2}", chessTurn.TurnNumber, chessTurn.LightTeamMoveNotation, chessTurn.DarkTeamMoveNotation);

        OnTurnFinished.Invoke();
    }
}
