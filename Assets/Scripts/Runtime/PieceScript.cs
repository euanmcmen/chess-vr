using Assets.Scripts.Runtime.Logic;
using System.Collections;
using UnityEngine;

public class PieceScript : MonoBehaviour
{
    public ChessPieceTeam Team;
    public ChessPieceType Type;
    public string InitialPositionNotation;

    public ChessBoardPosition CurrentBoardPosition { get; private set; }

    public bool IsCaptured { get; private set; }

    private BoardApiScript boardApi;
    private PieceMovementScript pieceMovementScript;
    private Transform currentTile;

    private void Awake()
    {
        boardApi = transform.GetComponentInParent<BoardApiScript>();
        pieceMovementScript = transform.GetComponent<PieceMovementScript>();
        IsCaptured = false;
    }

    private void Start()
    {
        var startingTile = boardApi.GetTileByNotation(InitialPositionNotation);

        SetCurrentPosition(startingTile);
    }

    public IEnumerator HandleMovement(string destinationNotation)
    {
        var targetTile = boardApi.GetTileByNotation(destinationNotation);
        var destinationPosition = GetPiecePositionOnTile(targetTile);

        yield return StartCoroutine(pieceMovementScript.HandleFloatToDestinationPosition(destinationPosition));

        SetCurrentPosition(targetTile);
    }

    public IEnumerator HandleMovementToGrave()
    {
        IsCaptured = true;

        var targetTile = boardApi.GetGraveBoardApiForTeam(Team).GetNextTile();
        var destinationPosition = GetPiecePositionOnTile(targetTile);

        yield return StartCoroutine(pieceMovementScript.HandleFloatToDestinationPosition(destinationPosition));

        SetCurrentPosition(targetTile);
    }

    private Vector3 GetPiecePositionOnTile(Transform targetTile)
    {
        return new Vector3(targetTile.position.x, transform.position.y, targetTile.position.z);
    }

    private void SetCurrentPosition(Transform targetTile)
    {
        if (currentTile != null)
        {
            currentTile.GetComponent<BoardTileScript>().Piece = null;
        }

        currentTile = targetTile;
        targetTile.GetComponent<BoardTileScript>().Piece = this;

        CurrentBoardPosition = IsCaptured ? null : new ChessBoardPosition(currentTile.name);
    }
}
