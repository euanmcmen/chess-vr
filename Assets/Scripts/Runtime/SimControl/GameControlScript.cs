﻿using Assets.Scripts.Runtime.Logic;
using Assets.Scripts.Runtime.Logic.Parser.GameParser;
using Assets.Scripts.Runtime.Logic.Parser.MoveParser;
using Assets.Scripts.Runtime.Logic.Parser.TurnParser;
using Assets.Scripts.Runtime.Logic.Resolvers;
using Assets.Scripts.Runtime.Models;
using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameControlScript : RealtimeComponent<GameControlModel>, IRunningStateChangedSubscriber
{
    private event Action<ChessTurnSet> onChessTurnSetParsed;

    private SimulationDataScript simulationDataScript;
    private SimulationBoardLinkScript simulationBoardLinkScript;
    private PieceMovementResolver pieceMovementValidator;

    private bool isRunning;

    [SerializeField]
    private GameObject pieceMovementDataPrefab;

    private void Awake()
    {
        simulationDataScript = GetComponent<SimulationDataScript>();
        simulationBoardLinkScript = GetComponent<SimulationBoardLinkScript>();

        pieceMovementValidator = new PieceMovementResolver(simulationBoardLinkScript.BoardApi);

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

    public void CreateTurnData()
    {
        var turnNotations = ChessGameParser.ResolveTurnsInGame(simulationDataScript.GameData.GamePGN);

        foreach (var turn in turnNotations)
        {
            var resolvedTurn = ChessTurnParser.ResolveChessTurn(turn);
            var resolvedTurnMoveData = ResolveMoveDataForTurn(resolvedTurn);

            var moveList = new List<TurnMovePieceData>();

            if (resolvedTurnMoveData.LightTeamMove != null)
            {
                moveList.Add(resolvedTurnMoveData.LightTeamMove.CapturedPieceMoveData);
                moveList.AddRange(resolvedTurnMoveData.LightTeamMove.MovingPiecesData);
            }

            if (resolvedTurnMoveData.DarkTeamMove != null)
            {
                moveList.Add(resolvedTurnMoveData.DarkTeamMove.CapturedPieceMoveData);
                moveList.AddRange(resolvedTurnMoveData.DarkTeamMove.MovingPiecesData);
            }

            for (int i = 0; i < moveList.Count; i++)
            {
                if (moveList[i] == null)
                    continue;

                var instantiationOptions = new Realtime.InstantiateOptions()
                {
                    destroyWhenOwnerLeaves = false,
                    destroyWhenLastClientLeaves = true,
                    ownedByClient = false
                };

                Realtime.Instantiate(pieceMovementDataPrefab.name, instantiationOptions)
                    .GetComponent<PieceMoveDataScript>().SetupModel(resolvedTurn.TurnNumber, i, moveList[i]);
            }
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

            var piece = simulationBoardLinkScript.BoardApi.GetPieceByName(move.PieceName);
            var destinationTile = simulationBoardLinkScript.BoardApi.GetTileByName(move.DestinationTileName);

            var destinationTileHighlightScript = destinationTile.GetComponent<BoardTileHighlightScript>();

            destinationTileHighlightScript.ShowHighlight();

            yield return StartCoroutine(piece.PlayMovementToPosition(destinationTile.position));

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

    private TurnData ResolveMoveDataForTurn(ChessTurn turn)
    {
        var turnData = new TurnData();

        var lightTeamMoveData = ResolveMoveDataForTurnTeam(ChessPieceTeam.Light, turn.LightTeamMoveNotation);
        turnData.LightTeamMove = lightTeamMoveData;

        var darkTeamMoveData = ResolveMoveDataForTurnTeam(ChessPieceTeam.Dark, turn.DarkTeamMoveNotation);
        turnData.DarkTeamMove = darkTeamMoveData;

        return turnData;
    }

    private TurnMoveData ResolveMoveDataForTurnTeam(ChessPieceTeam team, string teamMoveNotation)
    {
        var teamMoves = ChessMoveParser.ResolveChessNotation(team, teamMoveNotation);
        if (teamMoves == null)
        {
            Debug.LogWarningFormat("Unprocessable move: {0} {1}", team, teamMoveNotation);
            return null;
        }

        return ResolveMoveDataForTurnTeamPieces(team, teamMoves);
    }

    private TurnMoveData ResolveMoveDataForTurnTeamPieces(ChessPieceTeam team, List<ChessMove> moves)
    {
        TurnMoveData result = new() { Team = team, MovingPiecesData = new List<TurnMovePieceData>() };

        var allActivePieces = simulationBoardLinkScript.BoardApi.GetAllActivePieces();

        if (moves.Count == 1)
        {
            // In the case of a single move:
            // a regular instruction - d4, Nf2, Bb7...
            // a capture instruction - exd4, Nxf2, Bxb7
            var onlyMove = moves[0];

            if (onlyMove.CaptureOnDestinationTile)
            {
                result.CapturedPieceMoveData = ResolveCaptureMoveDataForPiece(team, onlyMove, ref allActivePieces);
            }
        }

        //Identify the piece to move.
        // In the case of multiple moves, it will be a castle instruction such as O-O or O-O-O.
        // Otherwise it will be the movement part of a regular instruction.
        // There will only be one capture in the case of a regular instruction.
        foreach (var move in moves)
        {
            result.MovingPiecesData.Add(ResolveMoveDataForPiece(team, move, ref allActivePieces));
        }

        // Result can contain:
        // One capture move and list-of-one move. (Regular movement instruction)
        // Null capture move and list-of-two moves. (Castle movement instruction)
        return result;
    }

    private TurnMovePieceData ResolveCaptureMoveDataForPiece(ChessPieceTeam currentTeam, ChessMove move, ref List<PieceScript> allActivePieces)
    {
        var otherTeam = EnumHelper.GetOtherTeam(currentTeam);

        //Identify the piece to be captured.
        var pieceToCapture = allActivePieces
            .Where(x => x.Team == otherTeam)
            .Single(x => x.CurrentBoardPosition.Notation == move.DestinationBoardPosition.Notation);

        //Identify the piece's destination tile.
        var pieceToCaptureDestinationTileName = simulationBoardLinkScript.BoardApi.GetGraveBoardApiForTeam(otherTeam).GetNextTile().name;

        // Prepare results for capture.
        var result = new TurnMovePieceData
        {
            PieceName = pieceToCapture.name,
            DestinationTileName = pieceToCaptureDestinationTileName
        };

        // Finish and apply move update.
        ApplyPieceCapture(pieceToCapture, pieceToCaptureDestinationTileName);

        return result;
    }

    private TurnMovePieceData ResolveMoveDataForPiece(ChessPieceTeam currentTeam, ChessMove move, ref List<PieceScript> allActivePieces)
    {
        var matchingPieces = allActivePieces.Where(x => x.Team == currentTeam && x.Type == move.PieceType);

        var pieceToMove = move.DisambiguationOriginBoardPosition != null
            ? GetPieceToMoveFromDisambiguation(matchingPieces, move)
            : GetPieceToMoveFromResolver(matchingPieces, currentTeam, move);

        // Prepare results for move.
        var result = new TurnMovePieceData
        {
            PieceName = pieceToMove.name,
            DestinationTileName = move.DestinationBoardPosition.Notation
        };

        // Finish and apply movement update.
        ApplyPieceMove(pieceToMove, move.DestinationBoardPosition.Notation);

        return result;
    }

    private void ApplyPieceCapture(PieceScript pieceToCapture, string tileNameToMoveTo)
    {
        pieceToCapture.SetCaptured();
        pieceToCapture.SetPositionOnTile(tileNameToMoveTo);
    }

    private void ApplyPieceMove(PieceScript pieceToMove, string tileNameToMoveTo)
    {
        pieceToMove.SetPositionOnTile(tileNameToMoveTo);
    }

    private PieceScript GetPieceToMoveFromDisambiguation(IEnumerable<PieceScript> matchingPieces, ChessMove move)
    {
        return DisambiguationHasPartialNotationOnly(move)
            ? matchingPieces.Single(x => x.CurrentBoardPosition.ColumnLetter == move.DisambiguationOriginBoardPosition.ColumnLetter)
            : matchingPieces.Single(x => x.CurrentBoardPosition.Notation == move.DisambiguationOriginBoardPosition.Notation);
    }

    private PieceScript GetPieceToMoveFromResolver(IEnumerable<PieceScript> matchingPieces, ChessPieceTeam team, ChessMove move)
    {
        try
        {
            return matchingPieces.Single(x => pieceMovementValidator.ResolveMovingPiece(team, x, move));
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Could not resolve move.  Error: {0}; Team: {1}; Move: {2}", e.Message, team, move.DestinationBoardPosition.Notation);
            throw;
        }
    }

    private bool DisambiguationHasPartialNotationOnly(ChessMove move)
    {
        return move.DisambiguationOriginBoardPosition is DisambiguationChessBoardPosition;
    }
}