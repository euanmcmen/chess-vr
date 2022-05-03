namespace Assets.Scripts.Runtime.Logic
{
    public class ChessMove
    {
        public IChessBoardPosition DisambiguationOriginBoardPosition { get; set; }

        public ChessBoardPosition DestinationBoardPosition { get; set; }

        public ChessPieceType PieceType { get; set; }

        public bool CaptureOnDestinationTile { get; set; }
    }
}