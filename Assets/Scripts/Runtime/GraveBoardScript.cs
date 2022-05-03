using System.Linq;
using UnityEngine;

public class GraveBoardScript : MonoBehaviour
{
    public ChessPieceTeam Team;

    public Transform GetNextTile()
    {
        return transform.GetComponentsInChildren<BoardTileScript>().Where(x => x.Piece == null).First().transform;
    }
}
