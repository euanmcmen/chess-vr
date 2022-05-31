using Assets.Scripts.Runtime.Logic;
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
    private WaitForSeconds turnWaitForSeconds;

    private bool isRunning;

    [SerializeField]
    private GameObject pieceMovementDataPrefab;

    private void Awake()
    {
        simulationDataScript = GetComponent<SimulationDataScript>();
        simulationBoardLinkScript = GetComponent<SimulationBoardLinkScript>();
        //pieceMoveControlScript = GetComponent<PieceMoveControlScript>();
        turnWaitForSeconds = new WaitForSeconds(simulationDataScript.ClockData.SecondsBetweenTurns);

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
        //TODO - Create a DGO for each turn resolved using the chess game parser.
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

                Realtime.Instantiate(pieceMovementDataPrefab.name, new Realtime.InstantiateOptions())
                    .GetComponent<PieceMoveDataScript>().SetupModel(resolvedTurn.TurnNumber, i, moveList[i]);
            }
        }
    }

    public IEnumerator PlayFromCurrentMove()
    {
        //TODO 29/05 - Find all piece move data objects, and order them by sequence id.
        // Find the next object to play, play the move, and update the last played sequence id.

        var moves = FindObjectsOfType<PieceMoveDataScript>().Cast<PieceMoveDataScript>()
            .Where(x => x.SequenceId > 0)
            .OrderBy(x => x.SequenceId)
            .ToList();

        Debug.LogFormat("{0} moves found.", moves.Count);

        foreach (var move in moves)
        {
            DispatchChessTurnSetEvents(move.TurnIndex);

            Debug.LogFormat("Sequence {0} - moving Piece {1} to tile {2}.",move.SequenceId, move.PieceName, move.DestinationTileName);

            var piece = simulationBoardLinkScript.BoardApi.GetPieceByName(move.PieceName);
            var destinationTile = simulationBoardLinkScript.BoardApi.GetTileByName(move.DestinationTileName);
            
            yield return StartCoroutine(piece.PlayMovementToPosition(destinationTile.position));
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

    //private void HandleChessTurnEvents(ChessTurnSet chessTurnSet)
    //{
    //    onChessTurnSetParsed.Invoke(chessTurnSet);
    //}

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

/*
 
    //public IEnumerator StartGameFromCurrentTurn()
    //{
    //    var turns = ChessGameParser.ResolveTurnsInGame(simulationDataScript.GameData.GamePGN);

    //    for (int i = model.currentTurnIndex; i < turns.Count; i++)
    //    {
    //        //yield return new WaitUntil(() => isRunning);
    //        if (!isRunning)
    //            yield break;

    //        int prev = i - 1;
    //        int next = i + 1;

    //        var chessTurnSet = new ChessTurnSet
    //        {
    //            Current = ChessTurnParser.ResolveChessTurn(turns[i])
    //        };

    //        if (prev >= 0)
    //        {
    //            chessTurnSet.Previous = ChessTurnParser.ResolveChessTurn(turns[prev]);
    //        }

    //        if (next < turns.Count)
    //        {
    //            chessTurnSet.Next = ChessTurnParser.ResolveChessTurn(turns[next]);
    //        }

    //        Debug.LogFormat("Current Turn Progress Index: {0}", model.currentTurnProgressIndex);

    //        if (model.currentTurnProgressIndex == 0)
    //        {
    //            HandleChessTurnEvents(chessTurnSet);
    //            model.currentTurnProgressIndex = 1;
    //        }

    //        if (!isRunning)
    //            yield break;

    //        if (model.currentTurnProgressIndex == 1)
    //        {
    //            ResolvePieceMovesForTurn(chessTurnSet.Current);
    //            model.currentTurnProgressIndex = 2;
    //        }

    //        if (!isRunning)
    //            yield break;

    //        if (model.currentTurnProgressIndex == 2)
    //        {

    //            //TODO - A piece's movement can take a while.
    //            // When a piece has moved, it should clear its HasMoved state.  It shouldn't wait for the end of the turn.
    //            // When a piece has started moving, it should take priority when resuming movement.
    //            // MovementInProgress -> Captures -> NewMoves.

    //            yield return StartCoroutine(PlayPieceMovesForTurn());
    //            model.currentTurnProgressIndex = 3;
    //        }

    //        if (!isRunning)
    //            yield break;

    //        if (model.currentTurnProgressIndex == 3)
    //        {
    //            //simulationBoardLinkScript.BoardApi.ResetAllPiecesForCurrentTurn();
    //            model.currentTurnTeamIndex = 0;
    //            model.currentTurnIndex++;
    //            model.currentTurnProgressIndex = 0;
    //        }

    //        yield return turnWaitForSeconds;
    //    }
    //}

    //private TurnMoveData ResolveMoveDataForTurnTeam(ChessPieceTeam team, string teamMoveNotation)
    //{
    //    var result = new List<TurnMoveData>();

    //    var teamMoves = ChessMoveParser.ResolveChessNotation(team, teamMoveNotation);
    //    if (teamMoves == null)
    //    {
    //        Debug.LogWarningFormat("Unprocessable move: {0} {1}", team, teamMoveNotation);
    //    }
    //    else
    //    {
    //        return ResolveMoveDataForTurnTeamPieces(team, teamMoves);

    //        //for (int i = 0; i < teamMoves.Count; i++)
    //        //{
    //        //    var turnMoveData = ResolveMoveDataForTurnTeamPieces(team, teamMoves[i], i);
    //        //    result.Add(turnMoveData);
    //        //}
    //    }

    //    return result;
    //}


    //private void ResolvePieceMovesForTurn(ChessTurn turn)
    //{
    //    var lightTeamMoves = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Light, turn.LightTeamMoveNotation);
    //    if (lightTeamMoves == null)
    //    {
    //        Debug.LogWarningFormat("Unprocessable move: {0} {1}", ChessPieceTeam.Light, turn.LightTeamMoveNotation);
    //    }
    //    else
    //    {
    //        for (int i = 0; i < lightTeamMoves.Count; i++)
    //        {
    //            ResolvePiecesToMove(ChessPieceTeam.Light, lightTeamMoves[i], i);
    //        }
    //    }

    //    var darkTeamMoves = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Dark, turn.DarkTeamMoveNotation);
    //    if (darkTeamMoves == null)
    //    {
    //        Debug.LogWarningFormat("Unprocessable move: {0} {1}", ChessPieceTeam.Dark, turn.DarkTeamMoveNotation);
    //    }
    //    else
    //    {
    //        for (int i = 0; i < darkTeamMoves.Count; i++)
    //        {
    //            ResolvePiecesToMove(ChessPieceTeam.Dark, darkTeamMoves[i], i);
    //        }
    //    }
    //}

    //public void ResolvePiecesToMove(ChessPieceTeam team, ChessMove move, int index)
    //{
    //    var allActivePieces = simulationBoardLinkScript.BoardApi.GetAllActivePieces();

    //    if (move.CaptureOnDestinationTile)
    //    {
    //        //Identify the piece to be captured.
    //        var pieceToCapture = allActivePieces
    //            .Where(x => x.Team != team)
    //            .Single(x => x.CurrentBoardPosition.Notation == move.DestinationBoardPosition.Notation);

    //        // Configure the piece so it can be identified through the network.
    //        pieceToCapture.GetComponent<PieceTurnScript>().SetCapturedOnThisTurn();

    //        // Apply the move update.  End of network sync stuff.

    //        pieceToCapture.SetPositionOnGraveTile();

    //        Debug.LogFormat("{0} {1} will be captured on this turn.", pieceToCapture.Team, pieceToCapture.name);
    //    }

    //    //Identify the piece to move.
    //    var matchingPieces = allActivePieces.Where(x => x.Team == team && x.Type == move.PieceType);

    //    var pieceToMove = move.DisambiguationOriginBoardPosition != null
    //        ? GetPieceToMoveFromDisambiguation(matchingPieces, move)
    //        : GetPieceToMoveFromResolver(matchingPieces, team, move);

    //    // Configure the piece so it can be identified through the network.
    //    var pieceTurnScript = pieceToMove.GetComponent<PieceTurnScript>();
    //    pieceTurnScript.SetMovesOnThisTurn(index);

    //    // Apply the move update.  End of network sync.
    //    pieceToMove.SetPositionOnTile(move.DestinationBoardPosition.Notation);

    //    Debug.LogFormat("{0} {1} will move on this turn.", pieceToMove.Team, pieceToMove.name);
    //}

    //private IEnumerator PlayPieceMovesForTurn()
    //{
    //    //TODO - Split the movement parts into a function, and add highlighting for the tile.
    //    //    var destinationHighlighter = simulationBoardLinkScript.BoardApi
    //    //        .GetTileByNotation(move.DestinationBoardPosition.Notation)
    //    //        .GetComponent<BoardTileHighlightScript>();
    //    //      destinationHighlighter.HideHighlight();

    //    var darkPieces = simulationBoardLinkScript.BoardApi.GetAllPiecesForTeam(ChessPieceTeam.Dark);
    //    var lightPieces = simulationBoardLinkScript.BoardApi.GetAllPiecesForTeam(ChessPieceTeam.Light);

    //    //Find all pieces which have started to move, and process those.
    //    var piecesAlreadyMoving = simulationBoardLinkScript.BoardApi.GetAllPieces()
    //        .Select(x => x.GetComponent<PieceTurnScript>())
    //        .Where(x => x.MovementStarted);

    //    foreach (var piece in piecesAlreadyMoving)
    //    {
    //        yield return StartCoroutine(piece.GetComponent<PieceScript>().PlayMovement());
    //    }

    //    // Find all black pieces with ShouldBeCaptured this turn, and move those.
    //    var darkPieceToCapture = darkPieces.SingleOrDefault(x => x.GetComponent<PieceTurnScript>().ShouldBeCapturedOnThisTurn);
    //    if (darkPieceToCapture != null)
    //    {
    //        yield return StartCoroutine(darkPieceToCapture.GetComponent<PieceScript>().PlayMovement());
    //    }

    //    // Find all light pieces with ShouldMoveThisTurn, and sort ascending by their index.  Move those.
    //    var lightPiecesToMove = lightPieces
    //        .Select(x => x.GetComponent<PieceTurnScript>())
    //        .Where(x => x.ShouldMoveOnThisTurn)
    //        .OrderBy(x => x.InTurnMoveIndex);
    //    foreach(var lightPiece in lightPiecesToMove)
    //    {
    //        yield return StartCoroutine(lightPiece.GetComponent<PieceScript>().PlayMovement());
    //    }

    //    // Find all light pieces with ShouldBeCaptured this turn, and move those.
    //    var lightPieceToCapture = lightPieces.SingleOrDefault(x => x.GetComponent<PieceTurnScript>().ShouldBeCapturedOnThisTurn);
    //    if (lightPieceToCapture != null)
    //    {
    //        yield return StartCoroutine(lightPieceToCapture.GetComponent<PieceScript>().PlayMovement());
    //    }

    //    // Find all dark pieces with ShouldMoveThisTurn, and sort ascending by their index.  Move those.
    //    var darkPiecesToMove = darkPieces
    //        .Select(x => x.GetComponent<PieceTurnScript>())
    //        .Where(x => x.ShouldMoveOnThisTurn)
    //        .OrderBy(x => x.InTurnMoveIndex);
    //    foreach (var darkPiece in darkPiecesToMove)
    //    {
    //        yield return StartCoroutine(darkPiece.GetComponent<PieceScript>().PlayMovement());
    //    }
    //}

    //public void UpdatePiecePositionsInMove(NetworkMoveInfo detailedMoveInfo)
    //{
    //    if (detailedMoveInfo.PieceToCaptureId != null)
    //    {
    //        simulationBoardLinkScript.BoardApi.GetByNetworkIdentityId(detailedMoveInfo.PieceToCaptureId)
    //            .GetComponent<PieceScript>().SetPositionOnGraveTile();
    //    }

    //    simulationBoardLinkScript.BoardApi.GetByNetworkIdentityId(detailedMoveInfo.PieceToMoveId)
    //            .GetComponent<PieceScript>().SetPositionOnTile(detailedMoveInfo.DestinationMovementNotation);
    //}

    //public IEnumerator PlayPieceMoves(NetworkMoveInfo detailedMoveInfo)
    //{
    //    var destinationHighlighter = simulationBoardLinkScript.BoardApi
    //        .GetTileByNotation(detailedMoveInfo.DestinationMovementNotation)
    //        .GetComponent<BoardTileHighlightScript>();

    //    destinationHighlighter.ShowHighlight();

    //    if (detailedMoveInfo.PieceToCaptureId != null)
    //    {
    //        yield return StartCoroutine(simulationBoardLinkScript.BoardApi.GetByNetworkIdentityId(detailedMoveInfo.PieceToCaptureId)
    //            .GetComponent<PieceScript>().PlayMovement());
    //    }

    //    yield return StartCoroutine(simulationBoardLinkScript.BoardApi.GetByNetworkIdentityId(detailedMoveInfo.PieceToMoveId)
    //            .GetComponent<PieceScript>().PlayMovement());

    //    destinationHighlighter.HideHighlight();
    //}

    //public IEnumerator HandleTeamPieceMove(ChessPieceTeam team, ChessMove move)
    //{
    //    var destinationHighlighter = simulationBoardLinkScript.BoardApi
    //        .GetTileByNotation(move.DestinationBoardPosition.Notation)
    //        .GetComponent<BoardTileHighlightScript>();

    //    destinationHighlighter.ShowHighlight();

    //    if (move.CaptureOnDestinationTile)
    //    {
    //        //var pieceToCapture = simulationBoardLinkScript.BoardApi
    //        //    .GetTileByNotation(move.DestinationBoardPosition.Notation)
    //        //    .GetComponent<BoardTileScript>().Piece;

    //        var pieceToCapture = simulationBoardLinkScript.BoardApi.GetAllActivePieces()
    //            .Where(x => x.Team != team)
    //            .Single(x => x.CurrentBoardPosition.Notation == move.DestinationBoardPosition.Notation);

    //        yield return StartCoroutine(pieceToCapture.HandleMovementToGrave());
    //    }

    //    var pieceToMove = GetPieceToMove(team, move);

    //    yield return StartCoroutine(pieceToMove.HandleMovement(move.DestinationBoardPosition.Notation));

    //    destinationHighlighter.HideHighlight();
    //}

    //private PieceScript GetPieceToMove(ChessPieceTeam team, ChessMove move)
    //{
    //    var matchingPieces = simulationBoardLinkScript.BoardApi.GetAllActivePieces()
    //        .Where(x => x.Team == team && x.Type == move.PieceType && !x.IsCaptured)
    //        .ToList();

    //    return move.DisambiguationOriginBoardPosition != null
    //        ? GetPieceToMoveFromDisambiguation(matchingPieces, move)
    //        : GetPieceToMoveFromResolver(matchingPieces, team, move);
    //}


 */