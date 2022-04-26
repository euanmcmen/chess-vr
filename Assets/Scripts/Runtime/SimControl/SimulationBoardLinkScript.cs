using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SimulationBoardLinkScript : MonoBehaviour
{
    [SerializeField]
    private BoardApiScript boardApiScript;

    public BoardApiScript BoardApi => boardApiScript;
}
