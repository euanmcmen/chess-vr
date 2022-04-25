using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardApiScript : MonoBehaviour
{
    // Pieces
    public List<PieceScript> GetAllPieces()
    {
        return transform.GetComponentsInChildren<PieceScript>().ToList();
    }

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

    // Tile Highlight
    public void ShowTileHighlightByNotation(string notation)
    {
        GetBoardTileScriptByNotation(notation).ShowHighlight();
    }

    // Tile Highlight
    public void HideTileHighlightByNotation(string notation)
    {
        GetBoardTileScriptByNotation(notation).HideHighlight();
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
