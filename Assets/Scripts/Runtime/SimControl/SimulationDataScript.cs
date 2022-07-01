using System.Collections;
using UnityEngine;

public class SimulationDataScript : MonoBehaviour
{
    [SerializeField]
    private ChessGameSetSO gameSet;

    [SerializeField]
    private ChessClockSO clockData;

    public ChessGameSetSO GameSet => gameSet;
    public ChessClockSO ClockData => clockData;
}