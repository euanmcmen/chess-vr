using Assets.Scripts.Runtime.Logic;
using Assets.Scripts.Runtime.Logic.Parser.GameParser;
using Assets.Scripts.Runtime.Logic.Parser.MoveParser;
using Assets.Scripts.Runtime.Logic.Parser.TurnParser;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameControlScript : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<ChessTurn> onChessTurnParsed;

    private SimulationDataScript simulationDataScript;
    private PieceMoveControlScript pieceMoveControlScript;
    private WaitForSeconds turnWaitForSeconds;

    private void Awake()
    {
        simulationDataScript = GetComponent<SimulationDataScript>();
        pieceMoveControlScript = GetComponent<PieceMoveControlScript>();
        turnWaitForSeconds = new WaitForSeconds(simulationDataScript.ClockData.SecondsBetweenTurns);
    }

    public IEnumerator HandleGame()
    {
        var turns = ChessGameParser.ResolveTurnsInGame(simulationDataScript.GameData.GamePGN);

        foreach (string turnNotation in turns)
        {
            yield return StartCoroutine(HandleTurn(turnNotation));
        }
    }

    private IEnumerator HandleTurn(string turnNotation)
    {
        var turn = ChessTurnParser.ResolveChessTurn(turnNotation);

        onChessTurnParsed.Invoke(turn);

        yield return StartCoroutine(HandleTeamMove(ChessPieceTeam.Light, turn.LightTeamMoveNotation));

        yield return StartCoroutine(HandleTeamMove(ChessPieceTeam.Dark, turn.DarkTeamMoveNotation));

        yield return turnWaitForSeconds;
    }

    public IEnumerator HandleTeamMove(ChessPieceTeam team, string notation)
    {
        var moves = ChessMoveParser.ResolveChessNotation(team, notation);

        if (moves == null)
        {
            Debug.LogWarningFormat("Unprocessable move: {0}", notation);
            yield break;
        }

        foreach (var move in moves)
        {
            yield return StartCoroutine(pieceMoveControlScript.HandleTeamPieceMove(team, move));
        }
    }
}