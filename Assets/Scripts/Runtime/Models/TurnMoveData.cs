using System.Collections.Generic;

namespace Assets.Scripts.Runtime.Models
{
    /*
     * MovingPiecesData will contain two elements in a capture move.
     * Both are part of the same turn, but two distinct move instructions.
     * Otherwise it should be considered a list-of-one.
     * 
     * CapturedPieceMoveData will contain an element if a piece is captured.
     * Movement of a captured piece and movement of a regular piece are part of the same turn, but two distinct move instructions.
     */


    public class TurnMoveData
    {
        public ChessPieceTeam Team { get; set; }

        public TurnMovePieceData CapturedPieceMoveData { get; set; }

        public List<TurnMovePieceData> MovingPiecesData { get; set; }
    }
}