using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Games/Game Set", order = 2)]
public class ChessGameSetSO : ScriptableObject
{
    [SerializeField]
    private List<ChessGameSO> chessGames;

    public List<ChessGameSO> ChessGames => chessGames;
}
