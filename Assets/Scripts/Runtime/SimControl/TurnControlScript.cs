using Assets.Scripts.Runtime.Logic;
using System.Collections;
using UnityEngine;

public class TurnControlScript : MonoBehaviour
{
    private MoveControlScript moveControlScript;

    private void Awake()
    {
        moveControlScript = GetComponent<MoveControlScript>();
    }

    public IEnumerator HandleTurn(ChessTurn chessTurn)
    {
        yield return StartCoroutine(moveControlScript.HandleTeamMove(ChessPieceTeam.Light, chessTurn.LightTeamMoveNotation));

        yield return StartCoroutine(moveControlScript.HandleTeamMove(ChessPieceTeam.Dark, chessTurn.DarkTeamMoveNotation));
    }
}