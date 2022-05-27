using Assets.Scripts.Runtime.Logic;
using Normal.Realtime;
using System.Collections;
using UnityEngine;

public class PieceScript : RealtimeComponent<PieceModel>, IRunningStateChangedSubscriber
{
    public ChessPieceTeam Team;
    public ChessPieceType Type;
    public string InitialPositionNotation;

    public ChessBoardPosition CurrentBoardPosition { get; private set; }

    public bool IsCaptured => model.isCaptured;

    private BoardApiScript boardApi;
    private PieceMovementScript pieceMovementScript;
    private PieceTurnScript pieceTurnScript;
    //private Transform currentTile;

    //private bool modelIsMoving;
    //private Transform modelIsMovingTo;
    //private bool isRunning;

    private void Awake()
    {
        boardApi = transform.GetComponentInParent<BoardApiScript>();
        pieceMovementScript = transform.GetComponent<PieceMovementScript>();
        pieceTurnScript = GetComponent<PieceTurnScript>();
    }

    private void Start()
    {
        var startingTile = boardApi.GetTileByName(InitialPositionNotation);

        SetPositionOnTile(startingTile);

        //SetCurrentPosition(startingTile);
    }

    public void HandleRunningStateChangedClient(bool value)
    {
        //StopAllCoroutines();
    }

    public void HandleRunningStateChanged(bool value)
    {
        //if (value)
        //{
        //    if (modelIsMoving)
        //    {
        //        SetCurrentPosition(modelIsMovingTo);
        //    }
        //}
    }

    public void SetPositionOnTile(string tileName)
    {
        var targetTile = boardApi.GetTileByName(tileName);
        SetPositionOnTile(targetTile);
    }

    public void SetCaptured()
    {
        model.isCaptured = true;
    }

    //public void SetPositionOnTile(string tileName)
    //{
    //    var targetTile = boardApi.GetTileByName(tileName);
    //    SetPositionOnTile(targetTile);
    //}

    //public void SetPositionOnGraveTile()
    //{
    //    model.isCaptured = true;
    //    var targetTile = boardApi.GetGraveBoardApiForTeam(Team).GetNextTile();
    //    SetPositionOnTile(targetTile);
    //}

    public IEnumerator PlayMovement()
    {
        Debug.Log("Movement starting", gameObject);

        pieceTurnScript.SetMovementStarted();

        // After updating the piece's position in data, this function will move the piece to the position on the board.

        // Read the current tile notation from the mode, get the tile with that name, and move to that position.
        var currentTile = boardApi.GetTileByName(model.currentTileName);
        Debug.LogFormat(gameObject, "Current Tile: {0}", currentTile.name);
        yield return StartCoroutine(pieceMovementScript.HandleFloatToDestinationPosition(currentTile.position));

        pieceTurnScript.SetMovementFinished();
        Debug.Log("Movement finished", gameObject);
    }

    //public IEnumerator HandleMovement(string destinationNotation)
    //{
    //    var targetTile = boardApi.GetTileByNotation(destinationNotation);

    //    yield return StartCoroutine(pieceMovementScript.HandleFloatToDestinationPosition(targetTile.position));

    //    SetCurrentPosition(targetTile);
    //}



    //public IEnumerator HandleMovementToGrave()
    //{
    //    IsCaptured = true;

    //    var targetTile = boardApi.GetGraveBoardApiForTeam(Team).GetNextTile();

    //    yield return StartCoroutine(pieceMovementScript.HandleFloatToDestinationPosition(targetTile.position));

    //    SetCurrentPosition(targetTile);
    //}


    private void SetPositionOnTile(Transform tile)
    {
        //currentTile = targetTile;
        //CurrentBoardPosition = IsCaptured ? null : new ChessBoardPosition(currentTile.name);

        model.currentTileName = tile.name;

        if (IsCaptured)
        {
            CurrentBoardPosition = null;
            return;
        }

        CurrentBoardPosition = new ChessBoardPosition(model.currentTileName);
    }
}
