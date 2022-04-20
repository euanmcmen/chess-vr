using Assets.Scripts.MovementValidator;
using Assets.Scripts.Parser;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BoardScript : MonoBehaviour
{
    //public UnityEvent OnTurnFinished;

    //private PieceMovementValidator pieceMovementValidator;

    //private void Awake()
    //{
    //    pieceMovementValidator = new PieceMovementValidator(this);
    //}

    //#region Turn Handling

    //public void HandleTurnParsedEvent(ChessTurn chessTurn)
    //{
    //    Debug.LogFormat("Turn {0} - Light Move: '{1}' Dark Move: '{2}'", chessTurn.TurnNumber, chessTurn.LightTeamMoveNotation, chessTurn.DarkTeamMoveNotation);

    //    StartCoroutine(HandleTurn(chessTurn));
    //}

    //private IEnumerator HandleTurn(ChessTurn chessTurn)
    //{
    //    yield return StartCoroutine(HandleTeamMove(ChessPieceTeam.Light, chessTurn.LightTeamMoveNotation));

    //    yield return StartCoroutine(HandleTeamMove(ChessPieceTeam.Dark, chessTurn.DarkTeamMoveNotation));

    //    OnTurnFinished.Invoke();
    //}

    //private IEnumerator HandleTeamMove(ChessPieceTeam team, string notation)
    //{
    //    var moves = ChessMoveParser.ResolveChessNotation(team, notation);

    //    if (moves == null)
    //    {
    //        Debug.LogWarningFormat("Unprocessable move: {0}", notation);
    //        yield break;
    //    }

    //    foreach (var move in moves)
    //    {
    //        yield return StartCoroutine(HandleTeamPieceMove(team, move));
    //    }
    //}

    //private IEnumerator HandleTeamPieceMove(ChessPieceTeam team, ChessMove move)
    //{
    //    if (move.CaptureOnDestinationTile)
    //    {
    //        Destroy(GetBoardTileScriptByNotation(move.DestinationBoardPosition.Notation).Piece.gameObject);
    //    }

    //    yield return StartCoroutine(
    //        GetPieceToMove(team, move)
    //        .HandleMovement(move.DestinationBoardPosition.Notation));
    //}

    //private PieceScript GetPieceToMove(ChessPieceTeam team, ChessMove move)
    //{
    //    var matchingPieces = transform.GetComponentsInChildren<PieceScript>().Where(x => x.Team == team && x.Type == move.PieceType)
    //        .ToList();

    //    return move.DisambiguationOriginBoardPosition != null
    //        ? GetPieceToMoveFromDisambiguation(matchingPieces, move)
    //        : GetPieceToMoveFromValidation(matchingPieces, team, move);
    //}

    //private PieceScript GetPieceToMoveFromDisambiguation(List<PieceScript> matchingPieces, ChessMove move)
    //{
    //    return move.DisambiguationOriginBoardPosition.IsPartialNotation
    //        ? matchingPieces.Single(x => x.CurrentBoardPosition.ColumnLetter == move.DisambiguationOriginBoardPosition.ColumnLetter)
    //        : matchingPieces.Single(x => x.CurrentBoardPosition.Notation == move.DisambiguationOriginBoardPosition.Notation);
    //}

    //private PieceScript GetPieceToMoveFromValidation(List<PieceScript> matchingPieces, ChessPieceTeam team, ChessMove move)
    //{
    //    return matchingPieces.Single(x => pieceMovementValidator.IsMoveValid(team, x, move));
    //}
    //#endregion

    //// Tile
    //public GameObject GetTileByNotation(string notation)
    //{
    //    return GetBoardTileTransformByNotation(notation).gameObject;
    //}

    //// Tile Piece
    //public PieceScript GetPieceOnTileByNotation(string notation)
    //{
    //    return GetBoardTileScriptByNotation(notation).Piece;
    //}

    //// Tile Piece
    //public void SetPieceOnTileByNotation(string notation, PieceScript piece)
    //{
    //    GetBoardTileScriptByNotation(notation).Piece = piece;
    //}

    //private BoardTileScript GetBoardTileScriptByNotation(string notation)
    //{
    //    return GetBoardTileTransformByNotation(notation).GetComponent<BoardTileScript>();
    //}

    //private Transform GetBoardTileTransformByNotation(string notation)
    //{
    //    return transform.Find(notation.ToLower());
    //}
}