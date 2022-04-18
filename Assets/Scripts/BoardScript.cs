using Assets.Scripts.LineOfSight;
using Assets.Scripts.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    public GameObject GetTileByNotation(string notation)
    {
        return transform.Find(notation.ToLower()).gameObject;
    }

    public PieceScript GetPieceOnTileByNotation(string notation)
    {
        return transform.Find(notation.ToLower()).GetComponent<BoardTileScript>().Piece;
    }

    public void SetPieceOnTileByNotation(string notation, PieceScript piece)
    {
        transform.Find(notation.ToLower()).GetComponent<BoardTileScript>().Piece = piece;
    }

    public Vector2Int GetAbsoluteBoardDistance(ChessBoardPosition currentPosition, ChessBoardPosition destinationPosition)
    {
        var columnDifference = Math.Abs((int)destinationPosition.ColumnLetter - (int)currentPosition.ColumnLetter);
        var rowDifference = Math.Abs(destinationPosition.RowNumber - currentPosition.RowNumber);
        return new Vector2Int(columnDifference, rowDifference);
    }
}