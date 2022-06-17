using UnityEngine;

public class ClimbingProviderResolver : MonoBehaviour
{
    private ClimbingProvider climbingProvider;

    private void Awake()
    {
        climbingProvider = GetComponentInChildren<ClimbingProvider>();
    }

    public ClimbingProvider GetClimbingProvider()
    {
        if (climbingProvider == null)
        {
            Debug.LogWarningFormat("ClimbingProvider on object {0} was accessed before Awake.", gameObject.name);
            climbingProvider = GetComponentInChildren<ClimbingProvider>();
        }

        return climbingProvider;
    }
}
