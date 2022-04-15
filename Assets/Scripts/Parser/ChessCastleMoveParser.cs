using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Parser
{
    public static class ChessCastleMoveParser
    {
        public static List<ChessMove> ResolveCastleNotation(ChessPieceTeam team, string notation)
        {
            var result = new List<ChessMove>();

            ChessBoardColumnLetter rookOrigin, rookDestination, kingOrigin, kingDestination;

            // Kingside
            if (notation == "O-O")
            {
                kingOrigin = ChessBoardColumnLetter.e;
                kingDestination = ChessBoardColumnLetter.g;
                rookOrigin = ChessBoardColumnLetter.h;
                rookDestination = ChessBoardColumnLetter.f;
            }
            // Queenside
            else if (notation == "O-O-O")
            {
                kingOrigin = ChessBoardColumnLetter.e;
                kingDestination = ChessBoardColumnLetter.c;
                rookOrigin = ChessBoardColumnLetter.a;
                rookDestination = ChessBoardColumnLetter.d;
            }
            else throw new System.Exception("Invalid castle notation");

            var rowNumber = GetCastleRowNumberForTeam(team);

            var kingMove = new ChessMove
            {
                DisambiguationOriginBoardPosition = new ChessBoardPosition($"{kingOrigin}{rowNumber}"),
                DestinationBoardPosition = new ChessBoardPosition($"{kingDestination}{rowNumber}"),
                PieceType = ChessPieceType.King,
                PieceMoveType = ChessPieceMoveType.Move
            };

            var rookMove = new ChessMove
            {
                DisambiguationOriginBoardPosition = new ChessBoardPosition($"{rookOrigin}{rowNumber}"),
                DestinationBoardPosition = new ChessBoardPosition($"{rookDestination}{rowNumber}"),
                PieceType = ChessPieceType.Rook,
                PieceMoveType = ChessPieceMoveType.Move
            };

            result.Add(kingMove);
            result.Add(rookMove);

            return result;
        }

        private static int GetCastleRowNumberForTeam(ChessPieceTeam team)
        {
            return team switch
            {
                ChessPieceTeam.Light => 1,
                ChessPieceTeam.Dark => 8,
                _ => 0,
            };
        }
    }
}
