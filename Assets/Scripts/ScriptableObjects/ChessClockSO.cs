using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Clock Data", order = 2)]
public class ChessClockSO : ScriptableObject
{
    [SerializeField]
    private float pieceMovementCompletesAfterSeconds;

    [SerializeField]
    private float secondsBetweenTurns;

    public float PieceMovementCompletesAfterSeconds => pieceMovementCompletesAfterSeconds;

    public float SecondsBetweenTurns => secondsBetweenTurns;
}
