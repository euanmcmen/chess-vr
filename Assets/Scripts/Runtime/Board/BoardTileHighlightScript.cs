using Normal.Realtime;
using UnityEngine;

public class BoardTileHighlightScript : RealtimeComponent<BoardTileHighlightModel>
{
    private GameObject highlight;

    private void Awake()
    {
        highlight = transform.Find("Highlight").gameObject;
    }

    public void ShowHighlight()
    {
        model.isTileHighlightActive = true;
    }
    public void HideHighlight()
    {
        model.isTileHighlightActive = false;
    }

    protected override void OnRealtimeModelReplaced(BoardTileHighlightModel previousModel, BoardTileHighlightModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.isTileHighlightActiveDidChange -= HandleIsTileHighlightActiveDidChange;
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {
            }

            currentModel.isTileHighlightActiveDidChange += HandleIsTileHighlightActiveDidChange;
        }
    }

    private void HandleIsTileHighlightActiveDidChange(BoardTileHighlightModel model, bool value)
    {
        SyncHighlightWithModel();
    }

    private void SyncHighlightWithModel()
    {
        highlight.SetActive(model.isTileHighlightActive);
    }
}
