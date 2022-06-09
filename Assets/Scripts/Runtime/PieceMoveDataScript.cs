using Assets.Scripts.Runtime.Models;
using Normal.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PieceMoveDataScript :  RealtimeComponent<PieceMoveDataModel>
{
    public int SequenceId => model.sequenceId;

    public string PieceName => model.pieceName;

    public string DestinationTileName => model.destinationTileName;

    public int TurnIndex => model.turnIndex;

    public void SetupModel(int turnIndex, int inTurnMoveIndex, TurnMovePieceData turnMovePieceData)
    {
        model.sequenceId = int.Parse($"{turnIndex}{inTurnMoveIndex}");

        model.turnIndex = turnIndex;
        model.inTurnMoveIndex = inTurnMoveIndex;
        model.pieceName = turnMovePieceData.PieceName;
        model.destinationTileName = turnMovePieceData.DestinationTileName;
    }
}
