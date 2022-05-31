using System.Linq;
using UnityEngine;

public class GraveBoardScript : MonoBehaviour
{
    public ChessPieceTeam Team;

    private BoardApiScript boardApiScript;

    private void Awake()
    {
        boardApiScript = GetComponentInParent<BoardApiScript>();
    }

    public Transform GetNextTile()
    {
        //return transform.GetComponentsInChildren<BoardTileScript>().Where(x => x.Piece == null).First().transform;
        return transform.GetChild(boardApiScript.GetAllPieces().Count(x => x.IsCaptured && x.Team == Team));
    }
}
