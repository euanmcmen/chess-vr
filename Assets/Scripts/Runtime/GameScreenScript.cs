using Assets.Scripts.Runtime.Logic;
using Normal.Realtime;
using TMPro;
using UnityEngine;

public class GameScreenScript  : RealtimeComponent<GameScreenModel>
{
    [SerializeField]
    private TMP_Text lightText;

    [SerializeField]
    private TMP_Text darkText;

    [SerializeField]
    private TMP_Text turnText;

    public void HandleTurnParsedEvent(ChessTurn chessTurn)
    {
        model.currentTurnNumber = chessTurn.TurnNumber.ToString();
        model.currentLightMove = chessTurn.LightTeamMoveNotation;
        model.currentDarkMove = chessTurn.DarkTeamMoveNotation;
    }

    protected override void OnRealtimeModelReplaced(GameScreenModel previousModel, GameScreenModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.currentTurnNumberDidChange -= HandleCurrentTurnNumberDidChange;
            previousModel.currentLightMoveDidChange -= HandleScreenCurrentLightMoveDidChange;
            previousModel.currentDarkMoveDidChange -= HandleScreenCurrentDarkMoveDidChange;
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {

            }

            currentModel.currentTurnNumberDidChange += HandleCurrentTurnNumberDidChange;
            currentModel.currentLightMoveDidChange += HandleScreenCurrentLightMoveDidChange;
            currentModel.currentDarkMoveDidChange += HandleScreenCurrentDarkMoveDidChange;
        }
    }

    #region Change handlers
    private void HandleCurrentTurnNumberDidChange(GameScreenModel model, string value)
    {
        UpdateScreenCurrentTurnNumber();
    }

    private void HandleScreenCurrentLightMoveDidChange(GameScreenModel model, string value)
    {
        UpdateScreenCurrentLightMove();
    }

    private void HandleScreenCurrentDarkMoveDidChange(GameScreenModel model, string value)
    {
        UpdateScreenCurrentDarkMove();
    }
    #endregion

    #region UI Update
    private void UpdateScreenCurrentTurnNumber()
    {
        turnText.text = model.currentTurnNumber.ToString();
    }

    private void UpdateScreenCurrentLightMove()
    {
        lightText.text = model.currentLightMove;
    }

    private void UpdateScreenCurrentDarkMove()
    {
        darkText.text = model.currentDarkMove;
    }
    #endregion
}
