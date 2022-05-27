using UnityEngine;

namespace Assets.Scripts.Runtime
{
    public class TurnDataScript : MonoBehaviour
    {
        public int TurnIndex { get; set; }
        public int InTurnMoveSequenceIndex { get; set; }

        public string DarkPieceBeingCapturedName { get; set; }
        public string DarkPieceBeingCapturedDestinationTileName { get; set; }

        public string LightPieceBeingMovedName { get; set; }
        public string LightPieceBeingMovedDestinationTileName { get; set; }

        public string LightPieceBeingCapturedName { get; set; }
        public string LightPieceBeingCapturedDestinationTileName { get; set; }

        public string DarkPieceBeingMovedName { get; set; }
        public string DarkPieceBeingMovedDestinationTileName { get; set; }
    }
}