using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardApiScript : MonoBehaviour
{
    //public GameObject GetByNetworkIdentityId(string networkIId)
    //{
    //    return transform.GetComponentsInChildren<NetworkIdentityScript>().Where(x => x.Id == networkIId).Single().gameObject;
    //}

    public List<PieceScript> GetAllPieces()
    {
        return transform.GetComponentsInChildren<PieceScript>().ToList();
    }

    public List<PieceScript> GetAllActivePieces()
    {
        return transform.GetComponentsInChildren<PieceScript>().Where(x => !x.IsCaptured).ToList();
    }


    //public List<PieceScript> GetAllPiecesForTeam(ChessPieceTeam team)
    //{
    //    return transform.GetComponentsInChildren<PieceScript>().Where(x => x.Team == team).ToList();
    //}


    //public List<PieceScript> GetAllCapturedPieces()
    //{
    //    return transform.GetComponentsInChildren<PieceScript>().Where(x => x.IsCaptured).ToList();
    //}

    public PieceScript GetPieceByName(string name)
    {
        return transform.GetComponentsInChildren<PieceScript>().Single(x => x.name == name);
    }

    //public Transform GetTileByNotation(string notation)
    //{
    //    return transform.Find(notation.ToLower());
    //}

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
