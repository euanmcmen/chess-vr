using Assets.Scripts.Runtime.Logic;
using System.Collections;
using UnityEngine;

public class PieceScript : MonoBehaviour
{
    public ChessPieceTeam Team;
    public ChessPieceType Type;
    public string InitialPositionNotation;

    private BoardApiScript boardApi;
    private PieceMovementScript pieceMovementScript;

    public ChessBoardPosition CurrentBoardPosition { get; private set; }

    private void Awake()
    {
        boardApi = transform.GetComponentInParent<BoardApiScript>();
        pieceMovementScript = transform.GetComponent<PieceMovementScript>();
    }

    private void Start()
    {
        SetCurrentPosition(InitialPositionNotation);
    }

    public IEnumerator HandleMovement(string destinationNotation)
    {
        var destinationPosition = GetPiecePositionOnTileAtNotation(destinationNotation);
        yield return StartCoroutine(pieceMovementScript.HandleFloatToDestinationPosition(destinationPosition));

        SetCurrentPosition(destinationNotation);
    }

    private Vector3 GetPiecePositionOnTileAtNotation(string notation)
    {
        var tilePos = boardApi.GetTileByNotation(notation).transform.position;
        return new Vector3(tilePos.x, transform.position.y, tilePos.z);
    }

    private void SetCurrentPosition(string notation)
    {
        if (CurrentBoardPosition != null)
        {
            boardApi.SetPieceOnTileByNotation(CurrentBoardPosition.Notation, null);
        }

        CurrentBoardPosition = new ChessBoardPosition(notation);

        boardApi.SetPieceOnTileByNotation(CurrentBoardPosition.Notation, this);
    }
}
