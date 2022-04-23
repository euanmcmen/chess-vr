using Assets.Scripts.Runtime.Logic;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Runtime.Logic.Parser.MoveParser
{
    public static class ChessStandardMoveParser
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

        public static List<ChessMove> ResolveChessMoveNotation(ChessPieceTeam team, string notation)
        {
            var result = new List<ChessMove>();

            // Evaluate notation using move notation.
            var matchKeys = RegexHelper.GetMatchCollection(notation, MoveRegex);

            var matchedResult = new ChessMove();

            var destinationNotation = matchKeys["destNotation"];
            matchedResult.DestinationBoardPosition = new ChessBoardPosition(destinationNotation);

            // Add Piece.  Pawn if piece is empty, otherwise use the table.
            if (!string.IsNullOrEmpty(matchKeys["piece"]))
            {
                matchedResult.PieceType = ChessPieceLetterMap[matchKeys["piece"]];
            }
            else
            {
                matchedResult.PieceType = ChessPieceType.Pawn;
            }

            // Add Disambiguation.
            if (!string.IsNullOrEmpty(matchKeys["originNotation"]))
            {
                var value = matchKeys["originNotation"];
                matchedResult.DisambiguationOriginBoardPosition = new ChessBoardPosition(value);
            }

            // Add capture flag.
            if (!string.IsNullOrEmpty(matchKeys["isCapture"]))
            {
                matchedResult.CaptureOnDestinationTile = true;
            }

            // Infer pawn origin from destination notation if disambiguator was not provided.
            if (matchedResult.PieceType == ChessPieceType.Pawn &&
                matchedResult.DisambiguationOriginBoardPosition == null)
            {
                matchedResult.DisambiguationOriginBoardPosition =
                    new ChessBoardPosition(matchedResult.DestinationBoardPosition.ColumnLetter);
            }

            result.Add(matchedResult);

            return result;
        }

    }
}
