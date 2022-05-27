using Normal.Realtime;
using System;

public class NetworkIdentityScript : RealtimeComponent<IdModel>
{
    public string Id => model.id;

    protected override void OnRealtimeModelReplaced(IdModel previousModel, IdModel currentModel)
    {
        if (previousModel != null)
        {
        }

        if (currentModel != null)
        {
            if (string.IsNullOrEmpty(currentModel.id))
            {
                currentModel.id = Guid.NewGuid().ToString();
            }
        }
    }
}
