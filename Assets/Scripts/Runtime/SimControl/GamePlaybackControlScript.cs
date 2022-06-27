using Assets.Scripts.Runtime.Logic;
using Assets.Scripts.Runtime.Logic.Parser.GameParser;
using Assets.Scripts.Runtime.Logic.Parser.TurnParser;
using Normal.Realtime;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GamePlaybackControlScript : RealtimeComponent<GameControlModel>, IRunningStateChangedSubscriber
{
    private event Action<ChessTurnSet> onChessTurnSetParsed;

    private SimulationDataScript simulationDataScript;
    private SimulationBoardLinkScript simulationBoardLinkScript;

    private bool isRunning;

    private void Awake()
    {
        simulationDataScript = GetComponent<SimulationDataScript>();
        simulationBoardLinkScript = GetComponent<SimulationBoardLinkScript>();
        
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

    public IEnumerator PlayFromCurrentMove()
    {
        var moves = FindObjectsOfType<PieceMoveDataScript>().Cast<PieceMoveDataScript>()
            .Where(x => x.SequenceId > model.lastPlayedSequenceId)
            .OrderBy(x => x.SequenceId)
            .ToList();

        Debug.LogFormat("{0} moves found.  Last move was {1}", moves.Count, model.lastPlayedSequenceId);

        foreach (var move in moves)
        {
            if (!isRunning)
                yield break;

            Debug.LogFormat("Sequence {0} - moving Piece {1} to tile {2}.", move.SequenceId, move.PieceName, move.DestinationTileName);

            DispatchChessTurnSetEvents(move.TurnIndex);

            var piecePlaybackScript = simulationBoardLinkScript.BoardApi.GetPieceByName(move.PieceName)
                .GetComponent<PiecePlaybackScript>();

            var destinationTile = simulationBoardLinkScript.BoardApi.GetTileByName(move.DestinationTileName);
            var destinationTileHighlightScript = destinationTile.GetComponent<BoardTileHighlightScript>();

            destinationTileHighlightScript.ShowHighlight();

            yield return StartCoroutine(piecePlaybackScript.HandleFloatToDestinationPosition(destinationTile.position));

            destinationTileHighlightScript.HideHighlight();

            model.lastPlayedSequenceId = move.SequenceId;
        }
    }

    private void DispatchChessTurnSetEvents(int turnNumber)
    {
        // Turn Number is 1-based, and index is 0-based.

        var turns = ChessGameParser.ResolveTurnsInGame(simulationDataScript.GameData.GamePGN);

        int current = turnNumber - 1;
        int prev = current - 1;
        int next = current + 1;

        var chessTurnSet = new ChessTurnSet
        {
            Current = ChessTurnParser.ResolveChessTurn(turns[current])
        };

        if (prev >= 0)
        {
            chessTurnSet.Previous = ChessTurnParser.ResolveChessTurn(turns[prev]);
        }

        if (next < turns.Count)
        {
            chessTurnSet.Next = ChessTurnParser.ResolveChessTurn(turns[next]);
        }

        onChessTurnSetParsed.Invoke(chessTurnSet);
    }
}
