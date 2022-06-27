using Assets.Scripts.Runtime.Logic;
using Normal.Realtime;
using TMPro;
using UnityEngine;

public class GameScreenTextSetScript : RealtimeComponent<GameScreenTextSetModel>
{
    [SerializeField]
    private ChessPieceTeam team;

    private TMP_Text previousText;
    private TMP_Text currentText;
    private TMP_Text nextText;

    private const string previousKey = "Previous";
    private const string currentKey = "Current";
    private const string nextKey = "Next";

    private void Awake()
    {
        previousText = transform.Find(previousKey).GetComponent<TMP_Text>();
        currentText = transform.Find(currentKey).GetComponent<TMP_Text>();
        nextText = transform.Find(nextKey).GetComponent<TMP_Text>();
    }

    public void UpdateTextFields(ChessTurnSet chessTurnSet)
    {
        model.previousMove = ExtractMoveNotation(chessTurnSet.Previous);
        model.currentMove = ExtractMoveNotation(chessTurnSet.Current);
        model.nextMove = ExtractMoveNotation(chessTurnSet.Next);
    }

    protected override void OnRealtimeModelReplaced(GameScreenTextSetModel previousModel, GameScreenTextSetModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.previousMoveDidChange -= HandlePreviousMoveDidChange;
            previousModel.currentMoveDidChange -= HandleCurrentMoveDidChange;
            previousModel.nextMoveDidChange -= HandleNextMoveDidChange;
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {

            }

            currentModel.previousMoveDidChange += HandlePreviousMoveDidChange;
            currentModel.currentMoveDidChange += HandleCurrentMoveDidChange;
            currentModel.nextMoveDidChange += HandleNextMoveDidChange;
        }
    }

    #region Change handlers
    private void HandlePreviousMoveDidChange(GameScreenTextSetModel model, string value)
    {
        UpdateScreenPreviousMove();
    }

    private void HandleCurrentMoveDidChange(GameScreenTextSetModel model, string value)
    {
        UpdateScreenCurrentMove();
    }

    private void HandleNextMoveDidChange(GameScreenTextSetModel model, string value)
    {
        UpdateScreenNextMove();
    }
    #endregion

    #region UI Update
    private void UpdateScreenPreviousMove()
    {
        previousText.text = model.previousMove;
    }

    private void UpdateScreenCurrentMove()
    {
        currentText.text = model.currentMove;
    }

    private void UpdateScreenNextMove()
    {
        nextText.text = model.nextMove;
    }
    #endregion

    private string ExtractMoveNotation(ChessTurn chessTurn)
    {
        if (chessTurn == null)
            return string.Empty;

        return team == ChessPieceTeam.Light
            ? chessTurn.LightTeamMoveNotation
            : chessTurn.DarkTeamMoveNotation;
    }
}
