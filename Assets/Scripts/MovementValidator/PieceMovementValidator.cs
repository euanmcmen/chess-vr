﻿using Assets.Scripts.LineOfSight;

namespace Assets.Scripts.MovementValidator
{
    public class PieceMovementValidator
    {
        private readonly BoardScript board;

        public PieceMovementValidator(BoardScript board)
        {
            this.board = board;
        }

        public bool IsMoveValid(ChessPieceTeam team, PieceScript piece, ChessMove move)
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

            return team switch
            {
                ChessPieceTeam.Light => piece.CurrentBoardPosition.RowNumber == (move.DestinationBoardPosition.RowNumber - 1) || piece.CurrentBoardPosition.RowNumber == (move.DestinationBoardPosition.RowNumber - 2),// A pawn on e2 can move to e3 or e4.
                ChessPieceTeam.Dark => piece.CurrentBoardPosition.RowNumber == (move.DestinationBoardPosition.RowNumber + 1) || piece.CurrentBoardPosition.RowNumber == (move.DestinationBoardPosition.RowNumber + 2),// A pawn on e7 can move to e6 or e5.
                _ => false,
            };
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
}