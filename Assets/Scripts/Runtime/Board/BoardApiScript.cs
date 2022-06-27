using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardApiScript : MonoBehaviour
{
    public IEnumerable<PieceScript> GetAllPieces(bool activeOnly = false)
    {
        return transform.GetComponentsInChildren<PieceScript>().Where(x => !activeOnly || !x.IsCaptured);
    }

    public List<PieceScript> GetAllPieces()
    {
        return transform.GetComponentsInChildren<PieceScript>().ToList();
    }

    public List<PieceScript> GetAllActivePieces()
    {
        return transform.GetComponentsInChildren<PieceScript>().Where(x => !x.IsCaptured).ToList();
    }

    public PieceScript GetPieceByName(string name)
    {
        return transform.GetComponentsInChildren<PieceScript>().Single(x => x.name == name);
    }

    public Transform GetTileByName(string name)
    {
        return transform.GetComponentsInChildren<BoardTileScript>()
            .Single(x => x.name == name)
            .transform;
    }

    public GraveBoardScript GetGraveBoardApiForTeam(ChessPieceTeam team)
    {
        return transform.GetComponentsInChildren<GraveBoardScript>().Single(x => x.Team == team);
    }
}
