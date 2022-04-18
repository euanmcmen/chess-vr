using Assets.Scripts.LineOfSight;
using Assets.Scripts.Parser;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameProcessor : MonoBehaviour
{
    [SerializeField]
    private BoardScript board;

    [SerializeField]
    private ChessClockSO clockData;

    [SerializeField]
    private ChessGameSO gameData;

    private int turnIndex;
    private List<string> turns;

    public float GetPieceMovementTime()
    {
        return clockData.PieceMovementCompletesAfterSeconds;
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        turnIndex = 0;
        turns = ChessGameParser.ResolveTurnsInGame(gameData.GamePGN);

        yield return null;

        yield return new WaitForSeconds(clockData.SecondsBetweenTurns);

        StartCoroutine(TurnLoop());
    }

    private IEnumerator TurnLoop()
    {
        var turn = ChessTurnParser.ResolveChessTurn(turns[turnIndex]);

        Debug.LogFormat("Turn {0} - Light Move: '{1}' Dark Move: '{2}'", turn.TurnNumber, turn.LightTeamMoveNotation, turn.DarkTeamMoveNotation);

        yield return StartCoroutine(HandleTeamMove(ChessPieceTeam.Light, turn.LightTeamMoveNotation));

        yield return StartCoroutine(HandleTeamMove(ChessPieceTeam.Dark, turn.DarkTeamMoveNotation));

        turnIndex = turn.TurnNumber;

        if (turnIndex == turns.Count)
        {
            yield break;
        }

        yield return new WaitForSeconds(clockData.SecondsBetweenTurns);

        StartCoroutine(TurnLoop());
    }

    private IEnumerator HandleTeamMove(ChessPieceTeam team, string notation)
    {
        var moves = ChessMoveParser.ResolveChessNotation(team, notation);

        if (moves == null)
        {
            Debug.LogWarningFormat("Unprocessable move: {0}", notation);
            yield break;
        }

        var pieces = board.transform.GetComponentsInChildren<PieceScript>();

        foreach (var move in moves)
        {
            PieceScript piece = null;

            // If we have a fully disambiguated move, use the piece on that origin tile.
            // A partially disambiguated move contains only the letter.
            if (move.DisambiguationOriginBoardPosition != null)
            {
                if (!move.DisambiguationOriginBoardPosition.IsPartialNotation)
                {
                    piece = pieces.Single(x => TeamAndTypeMatch(x, team, move.PieceType)
                            && x.CurrentBoardPosition.Notation == move.DisambiguationOriginBoardPosition.Notation);
                }
                else
                {
                    piece = pieces.Single(x => TeamAndTypeMatch(x, team, move.PieceType)
                            && x.CurrentBoardPosition.ColumnLetter == move.DisambiguationOriginBoardPosition.ColumnLetter);
                }

            }

            //If we have no disambiguation, find the piece for which the move is valid.
            else
            {
                piece = pieces.Single(x => TeamAndTypeMatch(x, team, move.PieceType) && IsMoveValid(team, x, move));
            }

            if (move.CaptureOnDestinationTile)
            {
                Destroy(board.GetPieceOnTileByNotation(move.DestinationBoardPosition.Notation).gameObject);
            }

            yield return StartCoroutine(piece.HandleMovement(move.DestinationBoardPosition.Notation, clockData.PieceMovementCompletesAfterSeconds));
        }
    }

    private static bool TeamAndTypeMatch(PieceScript pieceScript, ChessPieceTeam team, ChessPieceType type)
    {
        return pieceScript.Team == team && pieceScript.Type == type;
    }

    private bool IsMoveValid(ChessPieceTeam team, PieceScript piece, ChessMove move)
    {
        if (move.PieceType == ChessPieceType.Pawn)
        {
            return IsPawnMoveValid(team, piece, move);
        }

        if (move.PieceType == ChessPieceType.Bishop)
        {
            return IsBishopMoveValid(team, piece, move);
        }

        if (move.PieceType == ChessPieceType.Knight)
        {
            return IsKnightMoveValid(team, piece, move);
        }

        if (move.PieceType == ChessPieceType.Rook)
        {
            return IsRookMoveValid(team, piece, move);
        }

        if (move.PieceType == ChessPieceType.King
            || move.PieceType == ChessPieceType.Queen)
        {
            return IsRoyalMoveValid(team, piece, move);
        }

        return false;
    }

    // NOTE
    // This only works for pawn movement instructions where it moves forwards.
    private bool IsPawnMoveValid(ChessPieceTeam team, PieceScript piece, ChessMove move)
    {
        if (move.DestinationBoardPosition.ColumnLetter != piece.CurrentBoardPosition.ColumnLetter)
            return false;

        switch (team)
        {
            case ChessPieceTeam.Light:
                // A pawn on e2 can move to e3 or e4.
                return piece.CurrentBoardPosition.RowNumber == (move.DestinationBoardPosition.RowNumber - 1) || piece.CurrentBoardPosition.RowNumber == (move.DestinationBoardPosition.RowNumber - 2);
            case ChessPieceTeam.Dark:
                // A pawn on e7 can move to e6 or e5.
                return piece.CurrentBoardPosition.RowNumber == (move.DestinationBoardPosition.RowNumber + 1) || piece.CurrentBoardPosition.RowNumber == (move.DestinationBoardPosition.RowNumber + 2);
        }

        return false;
    }

    private bool IsKnightMoveValid(ChessPieceTeam team, PieceScript piece, ChessMove move)
    {
        var distance = board.GetAbsoluteBoardDistance(piece.CurrentBoardPosition, move.DestinationBoardPosition);
        return (distance.x == 2 && distance.y == 1) || (distance.x == 1 && distance.y == 2);
    }

    private bool IsBishopMoveValid(ChessPieceTeam team, PieceScript piece, ChessMove move)
    {
        var distance = board.GetAbsoluteBoardDistance(piece.CurrentBoardPosition, move.DestinationBoardPosition);
        if (!(distance.x == distance.y))
            return false;

        var moveBlocked = AnyPiecesOnPositionsBetween(piece.CurrentBoardPosition, move.DestinationBoardPosition);
        if (moveBlocked)
            return false;

        return true;
    }

    private bool IsRookMoveValid(ChessPieceTeam team, PieceScript piece, ChessMove move)
    {
        var distance = board.GetAbsoluteBoardDistance(piece.CurrentBoardPosition, move.DestinationBoardPosition);
        if (!((distance.x > 0 && distance.y == 0) || (distance.x == 0 && distance.y > 0)))
            return false;

        var moveBlocked = AnyPiecesOnPositionsBetween(piece.CurrentBoardPosition, move.DestinationBoardPosition);
        if (moveBlocked)
            return false;

        return true;
    }

    private bool IsRoyalMoveValid(ChessPieceTeam team, PieceScript piece, ChessMove move)
    {
        return true;
    }

    private bool AnyPiecesOnPositionsBetween(ChessBoardPosition currentPosition, ChessBoardPosition destinationPosition)
    {
        var positions = LineOfSightResolver.GetBoardTileNotationInRange(currentPosition, destinationPosition);

        foreach (var position in positions)
        {
            var piece = board.GetPieceOnTileByNotation(position.Notation);
            if (piece != null)
                return true;
        }

        return false;
    }
}
