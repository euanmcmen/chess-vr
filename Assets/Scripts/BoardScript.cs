using Assets.Scripts.LineOfSight;
using Assets.Scripts.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    // Pieces
    public IEnumerable<PieceScript> GetMatchingPieces(ChessPieceTeam team, ChessPieceType type)
    {
        return transform.GetComponentsInChildren<PieceScript>().Where(x => x.Team == team && x.Type == type);
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

    // Tile Piece
    public void RemovePieceOnTileByNotation(string notation)
    {
        Destroy(GetBoardTileScriptByNotation(notation).Piece.gameObject);
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