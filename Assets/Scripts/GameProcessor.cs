using Assets.Scripts.MovementValidator;
using Assets.Scripts.Parser;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameProcessor : MonoBehaviour
{
    [SerializeField]
    private BoardScript board;

    [SerializeField]
    private ChessClockSO clockData;

    [SerializeField]
    private ChessGameSO gameData;

    private int turnIndex;
    private List<string> turns;
    private PieceMovementValidator pieceMovementValidator;
    private List<PieceScript> pieces;

    private void Awake()
    {
        turnIndex = 0;
        
        turns = ChessGameParser.ResolveTurnsInGame(gameData.GamePGN);
        
        pieces = board.transform.GetComponentsInChildren<PieceScript>()
            .ToList();

        pieceMovementValidator = new PieceMovementValidator(board);
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(clockData.SecondsBetweenTurns);
        StartCoroutine(TurnLoop());
    }

    private IEnumerator TurnLoop()
    {
        var turn = ChessTurnParser.ResolveChessTurn(turns[turnIndex]);

        Debug.LogFormat("Turn {0} - Light Move: '{1}' Dark Move: '{2}'", turn.TurnNumber, turn.LightTeamMoveNotation, turn.DarkTeamMoveNotation);

        yield return StartCoroutine(HandleTeamMove(ChessPieceTeam.Light, turn.LightTeamMoveNotation));

        yield return StartCoroutine(HandleTeamMove(ChessPieceTeam.Dark, turn.DarkTeamMoveNotation));

        turnIndex = turn.TurnNumber;

        if (turnIndex == turns.Count)
        {
            yield break;
        }

        yield return new WaitForSeconds(clockData.SecondsBetweenTurns);

        StartCoroutine(TurnLoop());
    }

    private IEnumerator HandleTeamMove(ChessPieceTeam team, string notation)
    {
        var moves = ChessMoveParser.ResolveChessNotation(team, notation);

        if (moves == null)
        {
            Debug.LogWarningFormat("Unprocessable move: {0}", notation);
            yield break;
        }

        foreach (var move in moves)
        {
            if (move.CaptureOnDestinationTile)
            {
                Destroy(
                    board.GetPieceOnTileByNotation(move.DestinationBoardPosition.Notation).gameObject);
            }

            yield return StartCoroutine(
                    GetPieceToMove(team, move)
                        .HandleMovement(move.DestinationBoardPosition.Notation, clockData.PieceMovementCompletesAfterSeconds));
        }
    }

    private PieceScript GetPieceToMove(ChessPieceTeam team, ChessMove move)
    {
        var matchingPieces = pieces
            .Where(x => x.Team == team && x.Type == move.PieceType)
            .ToList();

        return move.DisambiguationOriginBoardPosition != null
            ? GetPieceToMoveFromDisambiguation(matchingPieces, move)
            : matchingPieces.Single(x => pieceMovementValidator.IsMoveValid(team, x, move));
    }

    private PieceScript GetPieceToMoveFromDisambiguation(List<PieceScript> matchingPieces, ChessMove move)
    {
        return !move.DisambiguationOriginBoardPosition.IsPartialNotation
            ? matchingPieces.Single(x => x.CurrentBoardPosition.Notation == move.DisambiguationOriginBoardPosition.Notation)
            : matchingPieces.Single(x => x.CurrentBoardPosition.ColumnLetter == move.DisambiguationOriginBoardPosition.ColumnLetter);
    }
}
