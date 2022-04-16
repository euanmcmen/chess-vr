using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Parser
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
            var match = Regex.Match(notation, MoveRegex);

            var matchKeys = match.Groups.Where(x => x.Success).ToDictionary(key => key.Name, value => value.Captures.SingleOrDefault());

            var matchedResult = new ChessMove();

            var destinationNotation = matchKeys["destNotation"].Value;
            matchedResult.DestinationBoardPosition = new ChessBoardPosition(destinationNotation);

            // Add Piece.  Pawn if piece is empty, otherwise use the table.
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

            // Add capture flag.
            if (!string.IsNullOrEmpty(matchKeys["isCapture"].Value))
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
