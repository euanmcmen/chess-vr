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

    private void Awake()
    {
        boardApi = transform.GetComponentInParent<BoardApiScript>();
        pieceMovementScript = transform.GetComponent<PieceMovementScript>();
    }

    private void Start()
    {
        var startingTile = boardApi.GetTileByName(InitialPositionNotation);

        SetPositionOnTile(startingTile);
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

    // NOTE - This relates to playback only.  Other functions in this file relate to Resolution.
    public IEnumerator PlayMovementToPosition(Vector3 tilePosition)
    {
        yield return StartCoroutine(pieceMovementScript.HandleFloatToDestinationPosition(tilePosition));
    }

    private void SetPositionOnTile(Transform tile)
    {
        model.currentTileName = tile.name;

        if (IsCaptured)
        {
            CurrentBoardPosition = null;
            return;
        }

        CurrentBoardPosition = new ChessBoardPosition(model.currentTileName);
    }
}
