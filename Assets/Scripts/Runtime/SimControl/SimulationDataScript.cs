using System.Collections;
using UnityEngine;

public class SimulationDataScript : MonoBehaviour
{
    [SerializeField]
    private ChessGameSO gameData;

    [SerializeField]
    private ChessClockSO clockData;

    public ChessGameSO GameData => gameData;
    public ChessClockSO ClockData => clockData;
}