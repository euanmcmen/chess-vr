using Assets.Scripts.Runtime.Logic;
using Assets.Scripts.Runtime.Logic.Parser.GameParser;
using Assets.Scripts.Runtime.Logic.Parser.MoveParser;
using Assets.Scripts.Runtime.Logic.Parser.TurnParser;
using Assets.Scripts.Runtime.Logic.Resolvers;
using Assets.Scripts.Runtime.Models;
using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChessGameSetupControlScript : MonoBehaviour
{
    [SerializeField]
    private GameObject pieceMovementDataPrefab;

    private event Action<List<string>> onGameParsed;

    private SimulationDataScript simulationDataScript;
    private SimulationBoardLinkScript simulationBoardLinkScript;
    private PieceMovementResolver pieceMovementValidator;

    private void Awake()
    {
        simulationDataScript = GetComponent<SimulationDataScript>();
        simulationBoardLinkScript = GetComponent<SimulationBoardLinkScript>();

        pieceMovementValidator = new PieceMovementResolver(simulationBoardLinkScript.BoardApi);
    }

    private void Start()
    {
        EventActionBinder.BindSubscribersToAction<IGameParsedSubscriber>((implementation) => onGameParsed += implementation.HandleGameParsed);
    }

    public IEnumerator CreateTurnData()
    {
        var randomGame = GetRandomGame();

        var turnNotations = ChessGameParser.ResolveTurnsInGame(randomGame.GamePGN);

        onGameParsed.Invoke(turnNotations);

        foreach (var turn in turnNotations)
        {
            var resolvedTurn = ChessTurnParser.ResolveChessTurn(turn);

            var resolvedTurnMoveData = new TurnData()
            {
                TurnNumber = resolvedTurn.TurnNumber,
                LightTeamMove = ResolveMoveDataForTurnTeam(ChessPieceTeam.Light, resolvedTurn.LightTeamMoveNotation),
                DarkTeamMove = ResolveMoveDataForTurnTeam(ChessPieceTeam.Dark, resolvedTurn.DarkTeamMoveNotation)
            };

            CreateTurnDataGameObjects(resolvedTurnMoveData);

            yield return null;
        }
    }

    private ChessGameSO GetRandomGame()
    {
        var numberOfGames = simulationDataScript.GameSet.ChessGames.Count;
        var randomGameIndex = UnityEngine.Random.Range(0, numberOfGames);
        return simulationDataScript.GameSet.ChessGames[randomGameIndex];
    }

    private TurnMoveData ResolveMoveDataForTurnTeam(ChessPieceTeam team, string teamMoveNotation)
    {
        var teamMoves = ChessMoveParser.ResolveChessNotation(team, teamMoveNotation);
        if (teamMoves == null)
        {
            Debug.LogWarningFormat("Unprocessable move: {0} {1}", team, teamMoveNotation);
            return null;
        }

        return ResolveMoveDataForTurnTeamPieces(team, teamMoves);
    }

    private TurnMoveData ResolveMoveDataForTurnTeamPieces(ChessPieceTeam team, List<ChessMove> moves)
    {
        TurnMoveData result = new() { Team = team, MovingPiecesData = new List<TurnMovePieceData>() };

        var allActivePieces = simulationBoardLinkScript.BoardApi.GetAllPieces(activeOnly: true);

        if (moves.Count == 1)
        {
            // In the case of a single move:
            // a regular instruction - d4, Nf2, Bb7...
            // a capture instruction - exd4, Nxf2, Bxb7
            var onlyMove = moves[0];

            if (onlyMove.CaptureOnDestinationTile)
            {
                result.CapturedPieceMoveData = ResolveCaptureMoveDataForPiece(team, onlyMove, ref allActivePieces);
            }
        }

        //Identify the piece to move.
        // In the case of multiple moves, it will be a castle instruction such as O-O or O-O-O.
        // Otherwise it will be the movement part of a regular instruction.
        // There will only be one capture in the case of a regular instruction.
        foreach (var move in moves)
        {
            result.MovingPiecesData.Add(ResolveMoveDataForPiece(team, move, ref allActivePieces));
        }

        // Result can contain:
        // One capture move and list-of-one move. (Regular movement instruction)
        // Null capture move and list-of-two moves. (Castle movement instruction)
        return result;
    }

    private TurnMovePieceData ResolveCaptureMoveDataForPiece(ChessPieceTeam currentTeam, ChessMove move, ref IEnumerable<PieceScript> allActivePieces)
    {
        var otherTeam = EnumHelper.GetOtherTeam(currentTeam);

        //Identify the piece to be captured.
        var pieceToCapture = allActivePieces
            .Where(x => x.Team == otherTeam)
            .Single(x => x.CurrentBoardPosition.Notation == move.DestinationBoardPosition.Notation);

        //Identify the piece's destination tile.
        var pieceToCaptureDestinationTileName = simulationBoardLinkScript.BoardApi.GetGraveBoardApiForTeam(otherTeam).GetNextTile().name;

        // Prepare results for capture.
        var result = new TurnMovePieceData
        {
            PieceName = pieceToCapture.name,
            DestinationTileName = pieceToCaptureDestinationTileName
        };

        // Finish and apply move update.
        ApplyPieceCapture(pieceToCapture, pieceToCaptureDestinationTileName);

        return result;
    }

    private TurnMovePieceData ResolveMoveDataForPiece(ChessPieceTeam currentTeam, ChessMove move, ref IEnumerable<PieceScript> allActivePieces)
    {
        var matchingPieces = allActivePieces.Where(x => x.Team == currentTeam && x.Type == move.PieceType);

        var pieceToMove = move.DisambiguationOriginBoardPosition != null
            ? GetPieceToMoveFromDisambiguation(matchingPieces, move)
            : GetPieceToMoveFromResolver(matchingPieces, currentTeam, move);

        // Prepare results for move.
        var result = new TurnMovePieceData
        {
            PieceName = pieceToMove.name,
            DestinationTileName = move.DestinationBoardPosition.Notation
        };

        // Finish and apply movement update.
        ApplyPieceMove(pieceToMove, move.DestinationBoardPosition.Notation);

        return result;
    }

    private void ApplyPieceCapture(PieceScript pieceToCapture, string tileNameToMoveTo)
    {
        pieceToCapture.SetCaptured();
        pieceToCapture.SetPositionOnTile(tileNameToMoveTo);
    }

    private void ApplyPieceMove(PieceScript pieceToMove, string tileNameToMoveTo)
    {
        pieceToMove.SetPositionOnTile(tileNameToMoveTo);
    }

    private PieceScript GetPieceToMoveFromDisambiguation(IEnumerable<PieceScript> matchingPieces, ChessMove move)
    {
        return move.DisambiguationOriginBoardPosition is DisambiguationChessBoardPosition
            ? matchingPieces.Single(x => x.CurrentBoardPosition.ColumnLetter == move.DisambiguationOriginBoardPosition.ColumnLetter)
            : matchingPieces.Single(x => x.CurrentBoardPosition.Notation == move.DisambiguationOriginBoardPosition.Notation);
    }

    private PieceScript GetPieceToMoveFromResolver(IEnumerable<PieceScript> matchingPieces, ChessPieceTeam team, ChessMove move)
    {
        try
        {
            return matchingPieces.Single(x => pieceMovementValidator.ResolveMovingPiece(team, x, move));
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Could not resolve move.  Error: {0}; Team: {1}; Move: {2}", e.Message, team, move.DestinationBoardPosition.Notation);
            throw;
        }
    }

    private void CreateTurnDataGameObjects(TurnData resolvedTurnMoveData)
    {
        var moveList = new List<TurnMovePieceData>();
        moveList.AddRange(GetTurnMovePieceDataForMove(resolvedTurnMoveData.LightTeamMove));
        moveList.AddRange(GetTurnMovePieceDataForMove(resolvedTurnMoveData.DarkTeamMove));

        for (int i = 0; i < moveList.Count; i++)
        {
            if (moveList[i] == null)
                continue;

            var instantiationOptions = new Realtime.InstantiateOptions()
            {
                destroyWhenOwnerLeaves = false,
                destroyWhenLastClientLeaves = true,
                ownedByClient = false
            };

            Realtime.Instantiate(pieceMovementDataPrefab.name, instantiationOptions)
                .GetComponent<PieceMoveDataScript>().SetupModel(resolvedTurnMoveData.TurnNumber, i, moveList[i]);
        }
    }

    private List<TurnMovePieceData> GetTurnMovePieceDataForMove(TurnMoveData turnMoveData)
    {
        var moveList = new List<TurnMovePieceData>();
        if (turnMoveData != null)
        {
            moveList.Add(turnMoveData.CapturedPieceMoveData);
            moveList.AddRange(turnMoveData.MovingPiecesData);
        }
        return moveList;
    }
}
