using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Piece Set", order = 4)]
public class ChessPieceSetSO : ScriptableObject
{
    [SerializeField]
    private GameObject pawnPrefab;

    [SerializeField]
    private GameObject bishopPrefab;

    [SerializeField]
    private GameObject knightPrefab;

    [SerializeField]
    private GameObject rookPrefab;

    [SerializeField]
    private GameObject queenPrefab;

    [SerializeField]
    private GameObject kingPrefab;

    public GameObject PawnPrefab => pawnPrefab;
    public GameObject BishopPrefab => bishopPrefab;
    public GameObject KnightPrefab => knightPrefab;
    public GameObject RookPrefab => rookPrefab;
    public GameObject QueenPrefab => queenPrefab;
    public GameObject KingPrefab => kingPrefab;
}
