//using Assets.Scripts.Runtime.Logic;
//using Assets.Scripts.Runtime.Logic.Resolvers;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class PieceMoveControlScript : MonoBehaviour, IRunningStateChangedSubscriber
//{
//    private SimulationBoardLinkScript simulationBoardLinkScript;

//    private PieceMovementResolver pieceMovementValidator;

//    private void Awake()
//    {
//        simulationBoardLinkScript = GetComponent<SimulationBoardLinkScript>();
//        pieceMovementValidator = new PieceMovementResolver(simulationBoardLinkScript.BoardApi);
//    }

//    public void HandleRunningStateChangedClient(bool value)
//    {
//        //if (!value)
//        //{
//        //    StopAllCoroutines();
//        //}
//    }

//    public void HandleRunningStateChanged(bool value)
//    {
//        //StopAllCoroutines();
//    }

//    public NetworkMoveInfo GetPiecesToMove(ChessPieceTeam team, ChessMove move)
//    {
//        var result = new NetworkMoveInfo { DestinationMovementNotation = move.DestinationBoardPosition.Notation };

//        var allActivePieces = simulationBoardLinkScript.BoardApi.GetAllActivePieces();

//        if (move.CaptureOnDestinationTile)
//        {
//            var pieceToCapture = allActivePieces
//                .Where(x => x.Team != team)
//                .Single(x => x.CurrentBoardPosition.Notation == move.DestinationBoardPosition.Notation);

//            result.PieceToCaptureId = pieceToCapture.GetComponent<NetworkIdentityScript>().Id;
//        }

//        var matchingPieces = allActivePieces.Where(x => x.Team == team && x.Type == move.PieceType);

//        var pieceToMove = move.DisambiguationOriginBoardPosition != null
//            ? GetPieceToMoveFromDisambiguation(matchingPieces, move)
//            : GetPieceToMoveFromResolver(matchingPieces, team, move);

//        result.PieceToMoveId = pieceToMove.GetComponent<NetworkIdentityScript>().Id;

//        return result;
//    }

//    public void UpdatePiecePositionsInMove(NetworkMoveInfo detailedMoveInfo)
//    {
//        if (detailedMoveInfo.PieceToCaptureId != null)
//        {
//            simulationBoardLinkScript.BoardApi.GetByNetworkIdentityId(detailedMoveInfo.PieceToCaptureId)
//                .GetComponent<PieceScript>().SetPositionOnGraveTile();
//        }

//        simulationBoardLinkScript.BoardApi.GetByNetworkIdentityId(detailedMoveInfo.PieceToMoveId)
//                .GetComponent<PieceScript>().SetPositionOnTile(detailedMoveInfo.DestinationMovementNotation);
//    }

//    public IEnumerator PlayPieceMoves(NetworkMoveInfo detailedMoveInfo)
//    {
//        var destinationHighlighter = simulationBoardLinkScript.BoardApi
//            .GetTileByNotation(detailedMoveInfo.DestinationMovementNotation)
//            .GetComponent<BoardTileHighlightScript>();

//        destinationHighlighter.ShowHighlight();

//        if (detailedMoveInfo.PieceToCaptureId != null)
//        {
//            yield return StartCoroutine(simulationBoardLinkScript.BoardApi.GetByNetworkIdentityId(detailedMoveInfo.PieceToCaptureId)
//                .GetComponent<PieceScript>().PlayMovement());
//        }

//        yield return StartCoroutine(simulationBoardLinkScript.BoardApi.GetByNetworkIdentityId(detailedMoveInfo.PieceToMoveId)
//                .GetComponent<PieceScript>().PlayMovement());

//        destinationHighlighter.HideHighlight();
//    }

//    //public IEnumerator HandleTeamPieceMove(ChessPieceTeam team, ChessMove move)
//    //{
//    //    var destinationHighlighter = simulationBoardLinkScript.BoardApi
//    //        .GetTileByNotation(move.DestinationBoardPosition.Notation)
//    //        .GetComponent<BoardTileHighlightScript>();

//    //    destinationHighlighter.ShowHighlight();

//    //    if (move.CaptureOnDestinationTile)
//    //    {
//    //        //var pieceToCapture = simulationBoardLinkScript.BoardApi
//    //        //    .GetTileByNotation(move.DestinationBoardPosition.Notation)
//    //        //    .GetComponent<BoardTileScript>().Piece;

//    //        var pieceToCapture = simulationBoardLinkScript.BoardApi.GetAllActivePieces()
//    //            .Where(x => x.Team != team)
//    //            .Single(x => x.CurrentBoardPosition.Notation == move.DestinationBoardPosition.Notation);

//    //        yield return StartCoroutine(pieceToCapture.HandleMovementToGrave());
//    //    }

//    //    var pieceToMove = GetPieceToMove(team, move);

//    //    yield return StartCoroutine(pieceToMove.HandleMovement(move.DestinationBoardPosition.Notation));

//    //    destinationHighlighter.HideHighlight();
//    //}

//    //private PieceScript GetPieceToMove(ChessPieceTeam team, ChessMove move)
//    //{
//    //    var matchingPieces = simulationBoardLinkScript.BoardApi.GetAllActivePieces()
//    //        .Where(x => x.Team == team && x.Type == move.PieceType && !x.IsCaptured)
//    //        .ToList();

//    //    return move.DisambiguationOriginBoardPosition != null
//    //        ? GetPieceToMoveFromDisambiguation(matchingPieces, move)
//    //        : GetPieceToMoveFromResolver(matchingPieces, team, move);
//    //}

//    private PieceScript GetPieceToMoveFromDisambiguation(IEnumerable<PieceScript> matchingPieces, ChessMove move)
//    {
//        return DisambiguationHasPartialNotationOnly(move)
//            ? matchingPieces.Single(x => x.CurrentBoardPosition.ColumnLetter == move.DisambiguationOriginBoardPosition.ColumnLetter)
//            : matchingPieces.Single(x => x.CurrentBoardPosition.Notation == move.DisambiguationOriginBoardPosition.Notation);
//    }

//    private PieceScript GetPieceToMoveFromResolver(IEnumerable<PieceScript> matchingPieces, ChessPieceTeam team, ChessMove move)
//    {
//        try
//        {
//            return matchingPieces.Single(x => pieceMovementValidator.ResolveMovingPiece(team, x, move));
//        }
//        catch(Exception e)
//        {
//            Debug.LogErrorFormat("Could not resolve move.  Error: {0}; Team: {1}; Move: {2}", e.Message, team, move.DestinationBoardPosition.Notation);
//            throw;
//        }
//    }

//    private bool DisambiguationHasPartialNotationOnly(ChessMove move)
//    {
//        return move.DisambiguationOriginBoardPosition is DisambiguationChessBoardPosition;
//    }
//}