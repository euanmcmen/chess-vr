using Assets.Scripts.Runtime.Logic;
using Assets.Scripts.Runtime.Logic.Parser.GameParser;
using Assets.Scripts.Runtime.Logic.Parser.TurnParser;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameControlScript : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<ChessTurn> onChessTurnParsed;

    private SimulationDataScript simulationDataScript;
    private TurnControlScript turnControlScript;

    private void Awake()
    {
        simulationDataScript = GetComponent<SimulationDataScript>();
        turnControlScript = GetComponent<TurnControlScript>();
    }

    public IEnumerator HandleGame()
    {
        var turnWaitForSeconds = new WaitForSeconds(simulationDataScript.ClockData.SecondsBetweenTurns);
        var turns = ChessGameParser.ResolveTurnsInGame(simulationDataScript.GameData.GamePGN);

        //var currentTurnIndex = 0;
        //while (currentTurnIndex < turns.Count)
        //{

        //}

        //for (int i = 0; i < turns.Count; i++)
        //{

        //}

        foreach (string turnNotation in turns)
        {
            var turn = ChessTurnParser.ResolveChessTurn(turnNotation);

            onChessTurnParsed.Invoke(turn);

            yield return StartCoroutine(turnControlScript.HandleTurn(turn));

            yield return turnWaitForSeconds;
        }

        //var turn = ChessTurnParser.ResolveCh
        //        essTurn(turns[currentTurnNumber]);

        //currentTurnNumber = turn.TurnNumber;

        //gameProcessorEventScript.DispatchEvent(turn);

        //yield return new WaitUntil(() => gameProcessorEventScript.IsFinished);

        //if (currentTurnNumber == turns.Count)
        //{
        //    yield break;
        //}

        //yield return new WaitForSeconds(clockData.SecondsBetweenTurns);

        //StartCoroutine(HandleCurrentTurn());
    }
}