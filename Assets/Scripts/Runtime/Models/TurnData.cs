using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Runtime.Models
{
    public class TurnData
    {
        public int TurnNumber { get; set; }

        public TurnMoveData LightTeamMove { get; set; }

        public TurnMoveData DarkTeamMove { get; set; }
    }
}
