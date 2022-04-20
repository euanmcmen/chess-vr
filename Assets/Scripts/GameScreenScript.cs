using UnityEngine;

public class GameScreenScript : MonoBehaviour
{
    public void HandleTurnParsedEvent(ChessTurn chessTurn)
    {
        Debug.LogFormat("Turn handled: {0}. {1} {2}", chessTurn.TurnNumber, chessTurn.LightTeamMoveNotation, chessTurn.DarkTeamMoveNotation);
    }
}
