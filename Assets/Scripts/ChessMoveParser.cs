using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ChessMove
{
    public int DisambiguationOriginRowNumber { get; set; }
    public ChessBoardColumnLetter DisambiguationOriginColumnLetter { get; set; }
    public string DisambiguationOriginNotation { get; set; }

    public int DestinationRowNumber { get; set; }
    public ChessBoardColumnLetter DestinationColumnLetter { get; set; }
    public string DestinationNotation { get; set; }

    public ChessPieceType PieceType { get; set; }

    public ChessPieceMoveType PieceMoveType { get; set; }
}

public static class ChessMoveParser
{
    private const string PawnMoveRegex = "^[a-h][1-9]$";

    private static readonly Dictionary<char, ChessPieceType> ChessPieceLetterMap = new Dictionary<char, ChessPieceType>()
    {
        { 'N', ChessPieceType.Knight },
        { 'B', ChessPieceType.Bishop },
        { 'R', ChessPieceType.Rook },
        { 'Q', ChessPieceType.Queen },
        { 'K', ChessPieceType.King }
    };

    public static List<ChessMove> ResolveChessNotation(ChessPieceTeam team, string notation)
    {
        if (IsCastleMove(notation))
        {
            return ResolveCastleNotation(team, notation);
        }

        if (IsPawnMove(notation))
        {
            return ResolvePawnMove(team, notation);
        }

        // If capture notation, add a capture event on the target square.

        var result = new List<ChessMove>();

        var piece = GetPiece(notation);

        var positionNotation = GetDestinationPosition(notation);
        var positionRow = GetDestinationRowNumberFromNotation(positionNotation);
        var positionColumn = GetDestinationColumnLetterFromNotation(positionNotation);

        // if the move contains a disambiguator after the piece and before the destination, set in result.

        result.Add(new ChessMove
        {
            DestinationColumnLetter = positionColumn,
            DestinationRowNumber = positionRow,
            DestinationNotation = positionNotation,
            PieceType = piece
        });

        return result;
    }

    private static List<ChessMove> ResolveCastleNotation(ChessPieceTeam team, string notation)
    {
        var result = new List<ChessMove>();

        var rowNumber = GetCastleRowNumberForTeam(team);

        var kingMove = new ChessMove
        {
            //DestinationColumnLetter = ChessBoardColumnLetter.g,
            DestinationRowNumber = rowNumber,
            //DestinationNotation = $"g{rowNumber}",
            //DisambiguationOriginColumnLetter = ChessBoardColumnLetter.e,
            DisambiguationOriginRowNumber = rowNumber,
            //DisambiguationOriginNotation = $"e{rowNumber}",
            PieceType = ChessPieceType.King
        };

        var rookMove = new ChessMove
        {
            //DestinationColumnLetter = ChessBoardColumnLetter.f,
            DestinationRowNumber = rowNumber,
            //DestinationNotation = $"f{rowNumber}",
            //DisambiguationOriginColumnLetter = ChessBoardColumnLetter.h,
            DisambiguationOriginRowNumber = rowNumber,
            //DisambiguationOriginNotation = $"h{rowNumber}",
            PieceType = ChessPieceType.Rook
        };

        // Kingside
        if (notation == "O-O")
        {
            kingMove = new ChessMove
            {
                DestinationColumnLetter = ChessBoardColumnLetter.g,
                //DestinationRowNumber = rowNumber,
                DestinationNotation = $"g{rowNumber}",
                DisambiguationOriginColumnLetter = ChessBoardColumnLetter.e,
                //DisambiguationOriginRowNumber = rowNumber,
                DisambiguationOriginNotation = $"e{rowNumber}",
                //PieceType = ChessPieceType.King
            };

            rookMove = new ChessMove
            {
                DestinationColumnLetter = ChessBoardColumnLetter.f,
                //DestinationRowNumber = rowNumber,
                DestinationNotation = $"f{rowNumber}",
                DisambiguationOriginColumnLetter = ChessBoardColumnLetter.h,
                //DisambiguationOriginRowNumber = rowNumber,
                DisambiguationOriginNotation = $"h{rowNumber}",
                //PieceType = ChessPieceType.Rook
            };

        }

        // Queenside
        else if (notation == "O-O-O")
        {
            kingMove = new ChessMove
            {
                DestinationColumnLetter = ChessBoardColumnLetter.c,
                //DestinationRowNumber = rowNumber,
                DestinationNotation = $"c{rowNumber}",
                DisambiguationOriginColumnLetter = ChessBoardColumnLetter.e,
                //DisambiguationOriginRowNumber = rowNumber,
                DisambiguationOriginNotation = $"e{rowNumber}",
                //PieceType = ChessPieceType.King
            };

            rookMove = new ChessMove
            {
                DestinationColumnLetter = ChessBoardColumnLetter.d,
                //DestinationRowNumber = rowNumber,
                DestinationNotation = $"f{rowNumber}",
                DisambiguationOriginColumnLetter = ChessBoardColumnLetter.a,
                //DisambiguationOriginRowNumber = rowNumber,
                DisambiguationOriginNotation = $"a{rowNumber}",
                //PieceType = ChessPieceType.Rook
            };
        }


        result.Add(kingMove);
        result.Add(rookMove);

        return result;
    }

    private static List<ChessMove> ResolvePawnMove(ChessPieceTeam team, string notation)
    {
        // e4
        // f2
        // a6

        var result = new List<ChessMove>();

        var positionNotation = GetDestinationPosition(notation);
        var positionRow = GetDestinationRowNumberFromNotation(positionNotation);
        var positionColumn = GetDestinationColumnLetterFromNotation(positionNotation);

        result.Add(new ChessMove
        {
            DestinationColumnLetter = positionColumn,
            DestinationRowNumber = positionRow,
            DestinationNotation = positionNotation,
            DisambiguationOriginColumnLetter= positionColumn,
            PieceType = ChessPieceType.Pawn
        });

        return result;
    }


    private static int GetCastleRowNumberForTeam(ChessPieceTeam team)
    {
        return team switch
        {
            ChessPieceTeam.Light => 1,
            ChessPieceTeam.Dark => 7,
            _ => 0,
        };
    }

    private static bool IsPawnMove(string notation)
    {
        return Regex.IsMatch(notation, PawnMoveRegex);
    }

    private static bool IsCastleMove(string notation)
    {
        return notation == "O-O" || notation == "O-O-O";
    }

    private static bool IsCaptureMove(string notation)
    {
        return notation.Contains("x");
    }

    private static ChessPieceType GetPiece(string notation)
    {
        return ChessPieceLetterMap[notation[0]];
    }

    private static string GetDestinationPosition(string notation)
    {
        int lengthSubtraction = notation.EndsWith('+') || notation.EndsWith('#') ? 3 : 2;
        return notation.Substring(notation.Length - lengthSubtraction, 2);
    }

    private static ChessBoardColumnLetter GetDestinationColumnLetterFromNotation(string positionNotation)
    {
        return Enum.Parse<ChessBoardColumnLetter>(positionNotation[0].ToString());
    }

    private static int GetDestinationRowNumberFromNotation(string positionNotation)
    {
        return int.Parse(positionNotation[1].ToString());
    }
}
