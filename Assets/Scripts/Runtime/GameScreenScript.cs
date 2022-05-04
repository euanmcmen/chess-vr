using Assets.Scripts.Runtime.Logic;
using Normal.Realtime;
using TMPro;
using UnityEngine;

public class GameScreenScript : RealtimeComponent<GameScreenModel>
{
    [SerializeField]
    private GameScreenTextSetScript lightTextSet;

    [SerializeField]
    private GameScreenTextSetScript darkTextSet;

    [SerializeField]
    private TMP_Text turnText;

    //public void HandleTurnParsedEvent(ChessTurn chessTurn)
    //{
    //    model.currentTurnNumber = chessTurn.TurnNumber.ToString();
    //    model.currentLightMove = chessTurn.LightTeamMoveNotation;
    //    model.currentDarkMove = chessTurn.DarkTeamMoveNotation;
    //}

    public void HandleTurnSetParsedEvent(ChessTurnSet chessTurnSet)
    {
        model.currentTurnNumber = chessTurnSet.Current.TurnNumber.ToString();

        lightTextSet.UpdateTextFields(chessTurnSet);

        darkTextSet.UpdateTextFields(chessTurnSet);
    }

    protected override void OnRealtimeModelReplaced(GameScreenModel previousModel, GameScreenModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.currentTurnNumberDidChange -= HandleCurrentTurnNumberDidChange;
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {

            }

            currentModel.currentTurnNumberDidChange += HandleCurrentTurnNumberDidChange;
        }
    }

    private void HandleCurrentTurnNumberDidChange(GameScreenModel model, string value)
    {
        UpdateScreenCurrentTurnNumber();
    }

    private void UpdateScreenCurrentTurnNumber()
    {
        turnText.text = model.currentTurnNumber.ToString();
    }
}
