public class ChessMove
{
    public ChessBoardPosition DisambiguationOriginBoardPosition { get; set; }

    public ChessBoardPosition DestinationBoardPosition { get; set; }

    //public int DisambiguationOriginRowNumber { get; set; }
    //public ChessBoardColumnLetter DisambiguationOriginColumnLetter { get; set; }
    //public string DisambiguationOriginNotation { get; set; }

    //public int DestinationRowNumber { get; set; }
    //public ChessBoardColumnLetter DestinationColumnLetter { get; set; }
    //public string DestinationNotation { get; set; }

    public ChessPieceType PieceType { get; set; }

    public ChessPieceMoveType PieceMoveType { get; set; }
}