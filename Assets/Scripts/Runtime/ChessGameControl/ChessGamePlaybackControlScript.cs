using Assets.Scripts.Runtime.Logic;
using Assets.Scripts.Runtime.Logic.Parser.TurnParser;
using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChessGamePlaybackControlScript : RealtimeComponent<ChessGameControlModel>, IRunningStateChangedSubscriber, IGameParsedSubscriber
{
    private event Action<ChessTurnSet> onChessTurnSetParsed;

    private SimulationDataScript simulationDataScript;
    private SimulationBoardLinkScript simulationBoardLinkScript;

    private bool isRunning;
    private List<string> parsedTurns;

    private void Awake()
    {
        simulationDataScript = GetComponent<SimulationDataScript>();
        simulationBoardLinkScript = GetComponent<SimulationBoardLinkScript>();
        parsedTurns = new List<string>();
    }

    private void Start()
    {
        EventActionBinder.BindSubscribersToAction<ITurnSetParsedSubscriber>((implementation) => onChessTurnSetParsed += implementation.HandleTurnSetParsedEvent);
    }

    public void HandleRunningStateChangedClient(bool value)
    {
        StopAllCoroutines();
    }

    public void HandleRunningStateChanged(bool value)
    {
        isRunning = value;

        if (value)
        {
            StartCoroutine(PlayFromCurrentMove());
        }
    }

    public void HandleGameParsed(List<string> value)
    {
        parsedTurns = value;
    }

    public IEnumerator PlayFromCurrentMove()
    {
        var moves = FindObjectsOfType<PieceMoveDataScript>().Cast<PieceMoveDataScript>()
            .Where(x => x.SequenceId > model.lastPlayedSequenceId)
            .OrderBy(x => x.SequenceId)
            .ToList();

        foreach (var move in moves)
        {
            if (!isRunning)
                yield break;

            DispatchChessTurnSetEvents(move.TurnIndex);

            var piecePlaybackScript = simulationBoardLinkScript.BoardApi.GetPieceByName(move.PieceName)
                .GetComponent<PiecePlaybackScript>();

            var destinationTile = simulationBoardLinkScript.BoardApi.GetTileByName(move.DestinationTileName);
            var destinationTileHighlightScript = destinationTile.GetComponent<BoardTileHighlightScript>();

            destinationTileHighlightScript.ShowHighlight();

            yield return StartCoroutine(piecePlaybackScript.HandleFloatToDestinationPosition(destinationTile.position));

            destinationTileHighlightScript.HideHighlight();

            model.lastPlayedSequenceId = move.SequenceId;

            yield return new WaitForSeconds(simulationDataScript.ClockData.SecondsBetweenMoves);
        }
    }

    private void DispatchChessTurnSetEvents(int turnNumber)
    {
        // Turn Number is 1-based, and index is 0-based.

        int current = turnNumber - 1;
        int prev = current - 1;
        int next = current + 1;

        var chessTurnSet = new ChessTurnSet
        {
            Current = ChessTurnParser.ResolveChessTurn(parsedTurns[current])
        };

        if (prev >= 0)
        {
            chessTurnSet.Previous = ChessTurnParser.ResolveChessTurn(parsedTurns[prev]);
        }

        if (next < parsedTurns.Count)
        {
            chessTurnSet.Next = ChessTurnParser.ResolveChessTurn(parsedTurns[next]);
        }

        onChessTurnSetParsed.Invoke(chessTurnSet);
    }
}
