using UnityEngine;

public class SimulationBoardLinkScript : MonoBehaviour
{
    [SerializeField]
    private BoardApiScript boardApiScript;

    public BoardApiScript BoardApi => boardApiScript;
}
