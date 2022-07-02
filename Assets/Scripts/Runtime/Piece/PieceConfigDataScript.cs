using UnityEngine;

public class PieceConfigDataScript : MonoBehaviour
{
    [SerializeField]
    private PieceConfigSO pieceConfig;

    public PieceConfigSO PieceConfig => pieceConfig;
}
