using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Clock Data", order = 2)]
public class ChessClockSO : ScriptableObject
{
    [SerializeField]
    private float secondsBetweenTurns;

    public float SecondsBetweenTurns => secondsBetweenTurns;
}
