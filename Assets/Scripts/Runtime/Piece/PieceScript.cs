using Assets.Scripts.Runtime.Logic;
using Normal.Realtime;
using UnityEngine;

//NOTE - This is functionally a "Piece Setup Script".  Perhaps I need a PieceApi script with *Setup and *Playback (created) scripts.

public class PieceScript : RealtimeComponent<PieceModel>
{
    [SerializeField]
    private ChessPieceTeam team;

    [SerializeField]
    private ChessPieceType type;

    [SerializeField]
    private string initialPositionNotation;

    public ChessBoardPosition CurrentBoardPosition { get; private set; }

    public ChessPieceTeam Team => team;

    public ChessPieceType Type => type;

    public string InitialPositionNotation => initialPositionNotation;

    public bool IsCaptured => model.isCaptured;

    private BoardApiScript boardApi;

    private void Awake()
    {
        boardApi = transform.GetComponentInParent<BoardApiScript>();
    }

    private void Start()
    {
        var startingTile = boardApi.GetTileByName(InitialPositionNotation);

        SetPositionOnTile(startingTile);
    }

    public void CompletePieceBuild(ChessPieceTeam team, ChessPieceType type, string initialPositionNotation)
    {
        this.team = team;
        this.type = type;
        this.initialPositionNotation = initialPositionNotation;
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
