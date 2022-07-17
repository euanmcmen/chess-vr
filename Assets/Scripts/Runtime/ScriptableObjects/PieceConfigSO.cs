using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Pieces/Piece Config", order = 1)]
public class PieceConfigSO : ScriptableObject
{
    [SerializeField]
    private float pieceMovementCompletesAfterSeconds;

    [SerializeField]
    private float pieceMovementFloatHeight;

    [SerializeField]
    private GameObject pieceMovementFinishedFxPrefab;

    public float PieceMovementCompletesAfterSeconds => pieceMovementCompletesAfterSeconds;

    public float PieceMovementFloatHeight => pieceMovementFloatHeight;

    public GameObject PieceMovementFinishedFxPrefab => pieceMovementFinishedFxPrefab;
}
