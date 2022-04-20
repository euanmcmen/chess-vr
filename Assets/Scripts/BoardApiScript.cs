using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BoardApiScript : MonoBehaviour
{
    // Tile
    public GameObject GetTileByNotation(string notation)
    {
        return GetBoardTileTransformByNotation(notation).gameObject;
    }

    // Tile Piece
    public PieceScript GetPieceOnTileByNotation(string notation)
    {
        return GetBoardTileScriptByNotation(notation).Piece;
    }

    // Tile Piece
    public void SetPieceOnTileByNotation(string notation, PieceScript piece)
    {
        GetBoardTileScriptByNotation(notation).Piece = piece;
    }

    private BoardTileScript GetBoardTileScriptByNotation(string notation)
    {
        return GetBoardTileTransformByNotation(notation).GetComponent<BoardTileScript>();
    }

    private Transform GetBoardTileTransformByNotation(string notation)
    {
        return transform.Find(notation.ToLower());
    }
}
