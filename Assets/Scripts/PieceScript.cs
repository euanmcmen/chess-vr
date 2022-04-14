using System;
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

    //public string CurrentPositionNotation { get; private set; }

    //public ChessBoardColumnLetter CurrentPositionColumnLetter { get; private set; }

    //public int CurrentPositionRowNumber { get; private set; }

    public ChessBoardPosition CurrentBoardPosition { get; private set; }

    private void Awake()
    {
        board = transform.GetComponentInParent<BoardScript>();
    }

    private void Start()
    {
        SetCurrentPosition(InitialPositionNotation);
    }

    public IEnumerator HandleMovement(string destinationNotation)
    {
        Debug.LogFormat("{0}: Moving to {1}.", gameObject.name, destinationNotation);

        var currentPosition = transform.position;
        var targetPosition = GetBoardTilePosition(destinationNotation);
        var currentPositionFloating = GetFloatPositionForPosition(currentPosition);
        var targetPositionFloating = GetFloatPositionForPosition(targetPosition);

        // Current -> CurrentFloating -> TargetFloating -> Target

        yield return StartCoroutine(HandleLerp(currentPosition, currentPositionFloating, board.pieceMoveCompletesAfterSeconds));

        yield return StartCoroutine(HandleLerp(currentPositionFloating, targetPositionFloating, board.pieceMoveCompletesAfterSeconds));

        yield return StartCoroutine(HandleLerp(targetPositionFloating, targetPosition, board.pieceMoveCompletesAfterSeconds));

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
        CurrentBoardPosition = new ChessBoardPosition(notation);

        //CurrentPositionNotation = notation;
        //CurrentPositionColumnLetter = Enum.Parse<ChessBoardColumnLetter>(notation[0].ToString());
        //CurrentPositionRowNumber = int.Parse(notation[1].ToString());
    }
}
