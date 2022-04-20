using System.Collections;
using UnityEngine;

public class PieceScript : MonoBehaviour
{
    [SerializeField]
    private PieceConfigSO pieceConfig;

    public ChessPieceTeam Team;
    public ChessPieceType Type;
    public string InitialPositionNotation;

    private BoardApiScript boardApi;

    public ChessBoardPosition CurrentBoardPosition { get; private set; }

    private void Awake()
    {
        boardApi = transform.GetComponentInParent<BoardApiScript>();
    }

    private void Start()
    {
        SetCurrentPosition(InitialPositionNotation);
    }

    // Handles: movement
    public IEnumerator HandleMovement(string destinationNotation)
    {
        var currentPosition = transform.position;
        var targetPosition = GetBoardTilePosition(destinationNotation);
        var currentPositionFloating = GetFloatPositionForPosition(currentPosition);
        var targetPositionFloating = GetFloatPositionForPosition(targetPosition);

        var movementPartCompletedAfterSeconds = pieceConfig.PieceMovementCompletesAfterSeconds / 3;

        yield return StartCoroutine(HandleLerp(currentPosition, currentPositionFloating, movementPartCompletedAfterSeconds));

        yield return StartCoroutine(HandleLerp(currentPositionFloating, targetPositionFloating, movementPartCompletedAfterSeconds));

        yield return StartCoroutine(HandleLerp(targetPositionFloating, targetPosition, movementPartCompletedAfterSeconds));

        transform.position = targetPosition;

        SetCurrentPosition(destinationNotation);
    }

    // Handles: movement
    private IEnumerator HandleLerp(Vector3 current, Vector3 target, float timeToComplete)
    {
        var lerpTime = 0.0f;

        while (lerpTime < 1)
        {
            lerpTime += Time.deltaTime / timeToComplete;
            transform.position = Vector3.Lerp(current, target, lerpTime);
            yield return null;
        }

    }

    // Handles: movement
    private Vector3 GetBoardTilePosition(string input)
    {
        var tilePos = boardApi.GetTileByNotation(input).transform.position;
        return new Vector3(tilePos.x, transform.position.y, tilePos.z);
    }

    // Handles: movement
    private Vector3 GetFloatPositionForPosition(Vector3 position)
    {
        return new Vector3(position.x, position.y + pieceConfig.PieceMovementFloatHeight, position.z);
    }

    // Handles: setting of piece board position property.
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
