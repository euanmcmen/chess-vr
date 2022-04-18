using System.Collections;
using UnityEngine;

public class PieceScript : MonoBehaviour
{
    [SerializeField]
    public ChessPieceTeam Team;

    [SerializeField]
    public ChessPieceType Type;

    [SerializeField]
    public string InitialPositionNotation;

    private BoardScript board;

    public ChessBoardPosition CurrentBoardPosition { get; private set; }

    private void Awake()
    {
        board = transform.GetComponentInParent<BoardScript>();
    }

    private void Start()
    {
        SetCurrentPosition(InitialPositionNotation);
    }

    public IEnumerator HandleMovement(string destinationNotation, float movementTime)
    {
        var currentPosition = transform.position;
        var targetPosition = GetBoardTilePosition(destinationNotation);
        var currentPositionFloating = GetFloatPositionForPosition(currentPosition);
        var targetPositionFloating = GetFloatPositionForPosition(targetPosition);

        yield return StartCoroutine(HandleLerp(currentPosition, currentPositionFloating, movementTime / 3));

        yield return StartCoroutine(HandleLerp(currentPositionFloating, targetPositionFloating, movementTime / 3));

        yield return StartCoroutine(HandleLerp(targetPositionFloating, targetPosition, movementTime / 3));

        transform.position = targetPosition;

        SetCurrentPosition(destinationNotation);
    }

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

    private Vector3 GetBoardTilePosition(string input)
    {
        var tilePos = board.GetTileByNotation(input).transform.position;
        return new Vector3(tilePos.x, transform.position.y, tilePos.z);
    }

    private Vector3 GetFloatPositionForPosition(Vector3 position)
    {
        return new Vector3(position.x, position.y +  5, position.z);
    }

    private void SetCurrentPosition(string notation)
    {
        if (CurrentBoardPosition != null)
        {
            board.SetPieceOnTileByNotation(CurrentBoardPosition.Notation, null);
        }

        CurrentBoardPosition = new ChessBoardPosition(notation);

        board.SetPieceOnTileByNotation(CurrentBoardPosition.Notation, this);
    }
}
