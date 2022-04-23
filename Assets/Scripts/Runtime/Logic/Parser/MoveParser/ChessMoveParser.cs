using Assets.Scripts.Runtime.Logic;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Runtime.Logic.Parser.MoveParser
{
    public static class ChessMoveParser
    {
        public static List<ChessMove> ResolveChessNotation(ChessPieceTeam team, string notation)
        {
            if (IsCastleMove(notation))
            {
                return ChessCastleMoveParser.ResolveCastleNotation(team, notation);
            }

            else if (IsWinConditionNotation(notation))
            {
                return null;
            }

            return ChessStandardMoveParser.ResolveChessMoveNotation(team, notation);
        }

        private static bool IsWinConditionNotation(string notation)
        {
            return notation == "1-0" || notation == "0-1";
        }

        private static bool IsCastleMove(string notation)
        {
            return notation == "O-O" || notation == "O-O-O";
        }
    }
}