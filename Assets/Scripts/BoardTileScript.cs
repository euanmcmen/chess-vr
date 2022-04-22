using UnityEngine;

public class BoardTileScript : MonoBehaviour
{
    public PieceScript Piece { get; set; }

    private GameObject highlight;

    private void Awake()
    {
        highlight = transform.Find("Highlight").gameObject;
    }

    public void ShowHighlight()
    {
        highlight.SetActive(true);
    }
    public void HideHighlight()
    {
        highlight.SetActive(false);
    }
}
