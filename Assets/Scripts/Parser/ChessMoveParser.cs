using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Parser
{
    public static class ChessMoveParser
    {
        private const string MoveRegex = "^(?'piece'[NBRQK]?)(?'originNotation'[a-h]?[1-9]?)(?'isCapture'[xX]?)(?'destNotation'[a-h][1-9])(?'isCheckCheckmate'[+#]?)$";

        // https://regex101.com/r/nLXEql/1

        private static readonly Dictionary<string, ChessPieceType> ChessPieceLetterMap = new Dictionary<string, ChessPieceType>()
        {
            { "N", ChessPieceType.Knight },
            { "B", ChessPieceType.Bishop },
            { "R", ChessPieceType.Rook },
            { "Q", ChessPieceType.Queen },
            { "K", ChessPieceType.King }
        };

        public static List<ChessMove> ResolveChessNotation(ChessPieceTeam team, string notation)
        {
            if (IsCastleMove(notation))
            {
                return ResolveCastleNotation(team, notation);
            }

            return ResolveChessMoveNotation(team, notation);
        }

        private static List<ChessMove> ResolveChessMoveNotation(ChessPieceTeam team, string notation)
        {
            var result = new List<ChessMove>();

            // Evaluate notation using move notation.
            var match = Regex.Match(notation, MoveRegex);

            var matchKeys = match.Groups.Where(x => x.Success).ToDictionary(key => key.Name, value => value.Captures.SingleOrDefault());

            var matchedResult = new ChessMove();

            // Add Piece.
            // Pawn if piece is empty, otherwise use the table.
            if (!string.IsNullOrEmpty(matchKeys["piece"].Value))
            {
                matchedResult.PieceType = ChessPieceLetterMap[matchKeys["piece"].Value];
            }
            else
            {
                matchedResult.PieceType = ChessPieceType.Pawn;
            }

            // Add Disambiguation.
            if (!string.IsNullOrEmpty(matchKeys["originNotation"].Value))
            {
                var value = matchKeys["originNotation"].Value;
                matchedResult.DisambiguationOriginBoardPosition = new ChessBoardPosition(value);
            }

            if (!string.IsNullOrEmpty(matchKeys["isCapture"].Value))
            {
                // Add capture instruction to move list.  Don't modify matchedResult.
            }

            if (!string.IsNullOrEmpty(matchKeys["destNotation"].Value))
            {
                var value = matchKeys["destNotation"].Value;
                matchedResult.DestinationBoardPosition = new ChessBoardPosition(value);

                // Infer pawn origin from destination notation if disambiguator was not provided.
                if (matchedResult.PieceType == ChessPieceType.Pawn &&
                    matchedResult.DisambiguationOriginBoardPosition == null)
                {
                    matchedResult.DisambiguationOriginBoardPosition = 
                        new ChessBoardPosition(matchedResult.DestinationBoardPosition.ColumnLetter);
                }
            }

            result.Add(matchedResult);

            return result;
        }

        private static List<ChessMove> ResolveCastleNotation(ChessPieceTeam team, string notation)
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

        private static bool IsCastleMove(string notation)
        {
            return notation == "O-O" || notation == "O-O-O";
        }
    }
}