using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Piece Config", order = 3)]
public class PieceConfigSO : ScriptableObject
{
    [SerializeField]
    private float pieceMovementCompletesAfterSeconds;

    [SerializeField]
    private float pieceMovementFloatHeight;

    public float PieceMovementCompletesAfterSeconds => pieceMovementCompletesAfterSeconds;

    public float PieceMovementFloatHeight => pieceMovementFloatHeight;
}
