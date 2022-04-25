using Assets.Scripts.Runtime.Logic;
using Assets.Scripts.Runtime.Logic.Parser.MoveParser;
using Assets.Scripts.Runtime.Logic.Resolvers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveControlScript : MonoBehaviour
{
    [SerializeField]
    private BoardApiScript boardApi;

    private PieceMovementResolver pieceMovementValidator;

    private void Awake()
    {
        pieceMovementValidator = new PieceMovementResolver(boardApi);
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
            yield return StartCoroutine(HandleTeamPieceMove(team, move));
        }
    }

    private IEnumerator HandleTeamPieceMove(ChessPieceTeam team, ChessMove move)
    {
        boardApi.ShowTileHighlightByNotation(move.DestinationBoardPosition.Notation);

        if (move.CaptureOnDestinationTile)
        {
            Destroy(boardApi.GetPieceOnTileByNotation(move.DestinationBoardPosition.Notation).gameObject);
        }

        yield return StartCoroutine(
            GetPieceToMove(team, move).HandleMovement(move.DestinationBoardPosition.Notation));

        boardApi.HideTileHighlightByNotation(move.DestinationBoardPosition.Notation);
    }

    private PieceScript GetPieceToMove(ChessPieceTeam team, ChessMove move)
    {
        var matchingPieces = boardApi.GetAllPieces().Where(x => x.Team == team && x.Type == move.PieceType).ToList();

        return move.DisambiguationOriginBoardPosition != null
            ? GetPieceToMoveFromDisambiguation(matchingPieces, move)
            : GetPieceToMoveFromResolver(matchingPieces, team, move);
    }

    private PieceScript GetPieceToMoveFromDisambiguation(List<PieceScript> matchingPieces, ChessMove move)
    {
        return move.DisambiguationOriginBoardPosition.IsPartialNotation
            ? matchingPieces.Single(x => x.CurrentBoardPosition.ColumnLetter == move.DisambiguationOriginBoardPosition.ColumnLetter)
            : matchingPieces.Single(x => x.CurrentBoardPosition.Notation == move.DisambiguationOriginBoardPosition.Notation);
    }

    private PieceScript GetPieceToMoveFromResolver(List<PieceScript> matchingPieces, ChessPieceTeam team, ChessMove move)
    {
        return matchingPieces.Single(x => pieceMovementValidator.ResolveMovingPiece(team, x, move));
    }
}