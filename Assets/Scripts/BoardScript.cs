using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    public float pieceMoveCompletesAfterSeconds;

    [SerializeField]
    private float nextMoveAfterSeconds;

    private int turnIndex;

    private List<string> turns;

    private void Awake()
    {
        turnIndex = 0;

        turns = new List<string>()
        {
            "e4 e5",
            "Nf3 Nc6",
            "Bb5 a6",
            "Ba4 Nf6",
            "O-O Be7",
            "Re1 b5",
            "Bb3 d6",
            "c3 O-O",
            "h3 Bb7",
            "d4 Na5",
            "Bc2 Nc4",
            "b3 Nb6",
            "Nbd2 Nbd7",
            "b4 exd4",
            "cxd4 a5",
            "bxa5 c5",
            "e5 dxe5",
            "dxe5 Nd5",
            "Ne4 Nb4",
            "Bb1 Rxa5",
            "Qe2 Nb6",
            "Nfg5 Bxe4",
            "Qxe4 g6",
            "Qh4 h5",
            "Qg3 Nc4",
            "Nf3 Kg7",
            "Qf4 Rh8",
            "e6 f5",
            "Bxf5 Qf8",
            "Be4 Qxf4",
            "Bxf4 Re8",
            "Rad1 Ra6",
            "Rd7 Rxe6",
            "Ng5 Rf6",
            "Bf3 Rxf4",
            "Ne6+ Kf6",
            "Nxf4 Ne5",
            "Rb7 Bd6",
            "Kf1 Nc2",
            "Re4 Nd4",
            "Rb6 Rd8",
            "Nd5+ Kf5",
            "Ne3+ Ke6",
            "Be2 Kd7",
            "Bxb5+ Nxb5",
            "Rxb5 Kc6",
            "a4 Bc7",
            "Ke2 g5",
            "g3 Ra8",
            "Rb2 Rf8",
            "f4 gxf4",
            "gxf4 Nf7",
            "Re6+ Nd6",
            "f5 Ra8",
            "Rd2 Rxa4",
            "f6 1-0",
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TurnLoop());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator TurnLoop()
    {
        yield return StartCoroutine(HandleTurn());

        yield return new WaitForSeconds(nextMoveAfterSeconds);

        turnIndex++;

        StartCoroutine(TurnLoop());
    }

    private IEnumerator HandleTurn()
    {
        var moves = turns[turnIndex].Split(' ');
        var lightMove = moves[0];
        var darkMove = moves[1];

        yield return StartCoroutine(HandleTeamMove(ChessPieceTeam.Light, lightMove));

        yield return StartCoroutine(HandleTeamMove(ChessPieceTeam.Dark, darkMove));
    }

    private IEnumerator HandleTeamMove(ChessPieceTeam team, string notation)
    {
        Debug.LogFormat("Executing {0} team move: {1}", team, notation);

        var moves = ChessMoveParser.ResolveChessNotation(team, notation);

        foreach (var move in moves)
        {
            PieceScript piece = null;

            // If we have a fully disambiguated move, use the piece on that origin tile.
            if (!string.IsNullOrEmpty(move.DisambiguationOriginNotation))
            {
                piece = transform.GetComponentsInChildren<PieceScript>()
                    .Single(x => x.Team == team && x.Type == move.PieceType && x.CurrentPositionNotation == move.DisambiguationOriginNotation);
            }

            //If we have a partially disambiguated move, use the piece on that row or column.
            else if (move.DisambiguationOriginColumnLetter != ChessBoardColumnLetter.None)
            {
                piece = transform.GetComponentsInChildren<PieceScript>()
                    .Single(x => x.Team == team && x.Type == move.PieceType && x.CurrentPositionColumnLetter == move.DisambiguationOriginColumnLetter);
            }
            else if (move.DisambiguationOriginRowNumber != 0)
            {
                piece = transform.GetComponentsInChildren<PieceScript>()
                    .Single(x => x.Team == team && x.Type == move.PieceType && x.CurrentPositionRowNumber == move.DisambiguationOriginRowNumber);
            }

            //If we have no disambiguation, find the piece for which the move is valid.
            else
            {
                piece = transform.GetComponentsInChildren<PieceScript>()
                    .Single(x => x.Team == team && x.Type == move.PieceType && IsMoveValid(team, x, move));
            }

            yield return StartCoroutine(piece.HandleMovement(move.DestinationNotation));
        }
    }

    private bool IsMoveValid(ChessPieceTeam team, PieceScript piece, ChessMove move)
    {
        //if (!HasLineOfSight(team, piece, move))
        //    return false;

        if (move.PieceType == ChessPieceType.Pawn)
        {
            return IsPawnMoveValid(team, piece, move);
        }

        if (move.PieceType == ChessPieceType.Bishop)
        {
            return IsBishopMoveValid(team, piece, move);
        }

        if (move.PieceType == ChessPieceType.Knight)
        {
            return IsKnightMoveValid(team, piece, move);
        }

        if (move.PieceType == ChessPieceType.Rook)
        {
            return IsRookMoveValid(team, piece, move);
        }

        if (move.PieceType == ChessPieceType.King 
            || move.PieceType == ChessPieceType.Queen)
        {
            return IsRoyalMoveValid(team, piece, move);
        }

        return false;
    }

    //private bool HasLineOfSight(ChessPieceTeam team, PieceScript piece, ChessMove move)
    //{
    //    // Return true if move.IsJump is true.  This is true for Knights and Castle-move instructions.

    //    //Fire a ray cast from the piece to the destination tile.  Return whether the ray hits a piece on the same team.
    //    var currentPosition = piece.transform.position;
    //    var targetTile = GetTileByNotation(move.DestinationNotation);

    //    //var targetPosition = new Vector3(targetTile.transform.position.x - targetTile.transform.localScale.x, 
    //    //    currentPosition.y, 
    //    //    targetTile.transform.position.z - targetTile.transform.localScale.z);

    //    var targetPosition = new Vector3(targetTile.transform.position.x, targetTile.transform.position.y + 0.1f, targetTile.transform.position.z);

    //    var direction = targetPosition - currentPosition;
    //    //var distance = Vector3.Distance(currentPosition, targetPosition);
    //    //var safeDistance = distance - targetTile.transform.localScale.x;
        
    //    var ray = new Ray(piece.transform.position, direction);

    //    var hits = Physics.SphereCastAll(ray, 1);

    //    var result = true;

    //    foreach (var hit in hits)
    //    {
    //        var hitPiece = hit.collider.gameObject.GetComponent<PieceScript>();

    //        if (hitPiece == null)
    //            continue;

    //        result = false;
    //    }

    //    return result;
    //}

    //
    // NOTE
    //
    // This only works for pawn movement instructions where it moves forwards.
    // Pawn captures diagonally.
    // Pawn captures should 
    private bool IsPawnMoveValid(ChessPieceTeam team, PieceScript piece, ChessMove move)
    {
        if (move.DestinationColumnLetter != piece.CurrentPositionColumnLetter)
            return false;

        switch (team)
        {
            case ChessPieceTeam.Light:
                // A pawn on e2 can move to e3 or e4.
                return piece.CurrentPositionRowNumber == (move.DestinationRowNumber - 1) || piece.CurrentPositionRowNumber == (move.DestinationRowNumber - 2);
            case ChessPieceTeam.Dark:
                // A pawn on e7 can move to e6 or e5.
                return piece.CurrentPositionRowNumber == (move.DestinationRowNumber + 1) || piece.CurrentPositionRowNumber == (move.DestinationRowNumber + 2);
        }

        return false;
    }

    public bool IsKnightMoveValid(ChessPieceTeam team, PieceScript piece, ChessMove move)
    {
        // Two increments of a row, one increment of a column.
        // Two increments of a column, one increment of a row.

        var a = Math.Abs(move.DestinationRowNumber - piece.CurrentPositionRowNumber);
        var b = Math.Abs((int)move.DestinationColumnLetter - (int)piece.CurrentPositionColumnLetter);
        return (a == 2 && b == 1) || (a == 1 && b == 2);
    }

    public bool IsBishopMoveValid(ChessPieceTeam team, PieceScript piece, ChessMove move)
    {
        var a = Math.Abs(move.DestinationRowNumber - piece.CurrentPositionRowNumber);
        var b = Math.Abs((int)move.DestinationColumnLetter - (int)piece.CurrentPositionColumnLetter);
        return a == b;
    }

    public bool IsRoyalMoveValid(ChessPieceTeam team, PieceScript piece, ChessMove move)
    {
        //var a = Math.Abs(move.DestinationRowNumber - piece.CurrentPositionRowNumber);
        //var b = Math.Abs((int)move.DestinationColumnLetter - (int)piece.CurrentPositionColumnLetter);
        //return a == 1 || a == 2 || b == 1 || b == 2;

        return true;
    }

    public bool IsRookMoveValid(ChessPieceTeam team, PieceScript piece, ChessMove move)
    {
        var a = Math.Abs(move.DestinationRowNumber - piece.CurrentPositionRowNumber);
        var b = Math.Abs((int)move.DestinationColumnLetter - (int)piece.CurrentPositionColumnLetter);
        return (a > 0 && b == 0) || (b > 0 && a == 0);
    }

    public GameObject GetTileByNotation(string notation)
    {
        return transform.Find(notation.ToLower()).gameObject;
    }
}