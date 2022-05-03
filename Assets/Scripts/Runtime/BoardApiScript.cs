using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardApiScript : MonoBehaviour
{
    public List<PieceScript> GetAllPieces()
    {
        return transform.GetComponentsInChildren<PieceScript>().ToList();
    }

    public Transform GetTileByNotation(string notation)
    {
        return transform.Find(notation.ToLower());
    }

    public GraveBoardScript GetGraveBoardApiForTeam(ChessPieceTeam team)
    {
        return transform.GetComponentsInChildren<GraveBoardScript>().Single(x => x.Team == team);
    }
}
