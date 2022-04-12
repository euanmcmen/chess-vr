using System;
using System.Collections;
using UnityEngine;

//[Serializable]
//public class PieceMoveInstruction
//{
//    [SerializeField]
//    public int MoveIndex;

//    [SerializeField]
//    public string MovePosition;
//}

public class PieceScript : MonoBehaviour
{
    [SerializeField]
    public ChessPieceTeam Team;

    [SerializeField]
    public ChessPieceType Type;

    [SerializeField]
    public string InitialPositionNotation;

    private BoardScript board;

    public string CurrentPositionNotation { get; private set; }

    public ChessBoardColumnLetter CurrentPositionColumnLetter { get; private set; }

    public int CurrentPositionRowNumber { get; private set; }

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

        var lerpTime = 0.0f;

        while (lerpTime < 1)
        {
            lerpTime += Time.deltaTime / board.pieceMoveCompletesAfterSeconds;
            transform.position = Vector3.Lerp(currentPosition, targetPosition, lerpTime);
            yield return null;
        }

        transform.position = targetPosition;

        SetCurrentPosition(destinationNotation);
    }

    private IEnumerator HandleMovementFloat()
    {
        yield break;
    }

    private Vector3 GetBoardTilePosition(string input)
    {
        var tilePos = board.GetTileByNotation(input).transform.position;
        return new Vector3(tilePos.x, transform.position.y, tilePos.z);
    }

    private void SetCurrentPosition(string notation)
    {
        Debug.LogFormat(gameObject, "{0} ({1} {2}) - Setting Position {3}", gameObject.name, Team.ToString(), Type.ToString(), notation);

        CurrentPositionNotation = notation;
        CurrentPositionColumnLetter = Enum.Parse<ChessBoardColumnLetter>(notation[0].ToString());
        CurrentPositionRowNumber = int.Parse(notation[1].ToString());
    }
}
