using System.Collections.Generic;

namespace Assets.Scripts.Parser
{
    public static class ChessParser
    {

        public static List<ChessMove> ResolveChessNotation(ChessPieceTeam team, string notation)
        {
            return IsCastleMove(notation)
                ? ChessCastleMoveParser.ResolveCastleNotation(team, notation)
                : ChessStandardMoveParser.ResolveChessMoveNotation(team, notation);
        }

        private static bool IsCastleMove(string notation)
        {
            return notation == "O-O" || notation == "O-O-O";
        }
    }
}