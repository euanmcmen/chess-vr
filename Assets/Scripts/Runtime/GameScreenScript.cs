using Assets.Scripts.Runtime.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameScreenScript : MonoBehaviour
{
    [SerializeField]
    private TMP_Text lightText;

    [SerializeField]
    private TMP_Text darkText;

    [SerializeField]
    private TMP_Text turnText;

    public UnityEvent OnTurnFinished;

    public void HandleTurnParsedEvent(ChessTurn chessTurn)
    {
        turnText.text = chessTurn.TurnNumber.ToString();

        lightText.text = chessTurn.LightTeamMoveNotation;

        darkText.text = chessTurn.DarkTeamMoveNotation;

        OnTurnFinished.Invoke();
    }
}
