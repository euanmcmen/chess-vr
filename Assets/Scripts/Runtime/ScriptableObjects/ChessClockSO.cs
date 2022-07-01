using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Settings/Clock Data", order = 2)]
public class ChessClockSO : ScriptableObject
{
    [SerializeField]
    private float secondsBetweenMoves;

    public float SecondsBetweenMoves => secondsBetweenMoves;
}
