using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Runtime.Models
{
    public class TurnData
    {
        public TurnMoveData LightTeamMove { get; set; }

        public TurnMoveData DarkTeamMove { get; set; }

        //public TurnData()
        //{
        //    LightTeamMoves = new List<TurnMoveData>();
        //    DarkTeamMoves = new List<TurnMoveData>();
        //}

        //public void AddMoveDataForTeam(ChessPieceTeam team, TurnMoveData turnMoveData)
        //{
        //    if (team == ChessPieceTeam.Light)
        //        LightTeamMoves.Add(turnMoveData);
        //    else if (team == ChessPieceTeam.Dark)
        //        DarkTeamMoves.Add(turnMoveData);
        //}
    }
}
