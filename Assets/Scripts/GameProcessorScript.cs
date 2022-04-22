using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProcessorScript : MonoBehaviour
{
    [SerializeField]
    private ChessClockSO clockData;

    [SerializeField]
    private ChessGameSO gameData;

    private int currentTurnNumber;
    private List<string> turns;

    private GameProcessorEventScript gameProcessorEventScript;

    private void Awake()
    {
        currentTurnNumber = 0;

        gameProcessorEventScript = GetComponent<GameProcessorEventScript>();

        turns = ChessGameParser.ResolveTurnsInGame(gameData.GamePGN);
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(clockData.SecondsBetweenTurns);
        StartCoroutine(HandleCurrentTurn());
    }

    public IEnumerator HandleCurrentTurn()
    {
        var turn = ChessTurnParser.ResolveChessTurn(turns[currentTurnNumber]);

        currentTurnNumber = turn.TurnNumber;

        gameProcessorEventScript.DispatchEvent(turn);

        yield return new WaitUntil(() => gameProcessorEventScript.IsFinished);

        if (currentTurnNumber == turns.Count)
        {
            yield break;
        }

        yield return new WaitForSeconds(clockData.SecondsBetweenTurns);

        StartCoroutine(HandleCurrentTurn());
    }
}
