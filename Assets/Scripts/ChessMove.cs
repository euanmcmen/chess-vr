public class ChessMove
{
    public ChessBoardPosition DisambiguationOriginBoardPosition { get; set; }

    public ChessBoardPosition DestinationBoardPosition { get; set; }

    public ChessPieceType PieceType { get; set; }

    public bool CaptureOnDestinationTile { get; set; }
}