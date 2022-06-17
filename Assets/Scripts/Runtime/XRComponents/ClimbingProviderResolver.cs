using UnityEngine;

public class ClimbingProviderResolver : MonoBehaviour
{
    [SerializeField]
    private ClimbingProvider climbingProvider;

    public ClimbingProvider GetClimbingProvider()
    {
        return climbingProvider;
    }
}
