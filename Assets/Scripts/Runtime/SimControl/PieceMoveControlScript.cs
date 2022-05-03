using Assets.Scripts.Runtime.Logic;
using Assets.Scripts.Runtime.Logic.Resolvers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceMoveControlScript : MonoBehaviour
{
    private SimulationBoardLinkScript simulationBoardLinkScript;

    private PieceMovementResolver pieceMovementValidator;

    private void Awake()
    {
        simulationBoardLinkScript = GetComponent<SimulationBoardLinkScript>();
        pieceMovementValidator = new PieceMovementResolver(simulationBoardLinkScript.BoardApi);
    }

    public IEnumerator HandleTeamPieceMove(ChessPieceTeam team, ChessMove move)
    {
        var destinationHighlighter = simulationBoardLinkScript.BoardApi
            .GetTileByNotation(move.DestinationBoardPosition.Notation)
            .GetComponent<BoardTileHighlightScript>();

        destinationHighlighter.ShowHighlight();

        if (move.CaptureOnDestinationTile)
        {
            var pieceToCapture = simulationBoardLinkScript.BoardApi
                .GetTileByNotation(move.DestinationBoardPosition.Notation)
                .GetComponent<BoardTileScript>().Piece;

            yield return StartCoroutine(pieceToCapture.HandleMovementToGrave());
        }

        yield return StartCoroutine(GetPieceToMove(team, move).HandleMovement(move.DestinationBoardPosition.Notation));

        destinationHighlighter.HideHighlight();
    }

    private PieceScript GetPieceToMove(ChessPieceTeam team, ChessMove move)
    {
        var matchingPieces = simulationBoardLinkScript.BoardApi.GetAllPieces()
            .Where(x => x.Team == team && x.Type == move.PieceType && !x.IsCaptured)
            .ToList();

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