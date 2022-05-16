﻿using Assets.Scripts.Runtime.Logic;
using Assets.Scripts.Runtime.Logic.Parser.GameParser;
using Assets.Scripts.Runtime.Logic.Parser.MoveParser;
using Assets.Scripts.Runtime.Logic.Parser.TurnParser;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameControlScript : MonoBehaviour, IRunningStateChangedSubscriber
{
    private event Action<ChessTurnSet> onChessTurnSetParsed;

    private SimulationDataScript simulationDataScript;
    private PieceMoveControlScript pieceMoveControlScript;
    private WaitForSeconds turnWaitForSeconds;

    private bool isRunning;

    private void Awake()
    {
        simulationDataScript = GetComponent<SimulationDataScript>();
        pieceMoveControlScript = GetComponent<PieceMoveControlScript>();
        turnWaitForSeconds = new WaitForSeconds(simulationDataScript.ClockData.SecondsBetweenTurns);

        EventActionBinder.BindSubscribersToAction<ITurnSetParsedSubscriber>((implementation) => onChessTurnSetParsed += implementation.HandleTurnSetParsedEvent);
    }

    public void HandleRunningStateChanged(bool isRunning)
    {
        this.isRunning = isRunning;
    }

    public IEnumerator HandleGame()
    {
        var turns = ChessGameParser.ResolveTurnsInGame(simulationDataScript.GameData.GamePGN);

        for (int i = 0; i < turns.Count; i++)
        {
            yield return new WaitUntil(() => isRunning);

            int prev = i - 1;
            int next = i + 1;

            var chessTurnSet = new ChessTurnSet
            {
                Current = ChessTurnParser.ResolveChessTurn(turns[i])
            };

            if (prev >= 0)
            {
                chessTurnSet.Previous = ChessTurnParser.ResolveChessTurn(turns[prev]);
            }

            if (next < turns.Count)
            {
                chessTurnSet.Next = ChessTurnParser.ResolveChessTurn(turns[next]);
            }

            HandleChessTurnEvents(chessTurnSet);

            yield return StartCoroutine(HandleCurrentTurn(chessTurnSet.Current));
        }
    }

    private void HandleChessTurnEvents(ChessTurnSet chessTurnSet)
    {
        onChessTurnSetParsed.Invoke(chessTurnSet);
    }

    private IEnumerator HandleCurrentTurn(ChessTurn turn)
    {
        yield return StartCoroutine(HandleTeamMove(ChessPieceTeam.Light, turn.LightTeamMoveNotation));

        yield return StartCoroutine(HandleTeamMove(ChessPieceTeam.Dark, turn.DarkTeamMoveNotation));

        yield return turnWaitForSeconds;
    }

    private IEnumerator HandleTeamMove(ChessPieceTeam team, string notation)
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