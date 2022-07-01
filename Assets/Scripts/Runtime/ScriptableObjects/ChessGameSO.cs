using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Games/Game Data", order = 1)]
public class ChessGameSO : ScriptableObject
{
    [SerializeField]
    private string gamePGN;

    public string GamePGN => gamePGN;
}
