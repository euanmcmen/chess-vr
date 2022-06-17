using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Since the vertical gravity in the moving provider accumulates even when it's not in use, 
/// we need to disable the gravity when climbing so the player doesn't snap to the ground.
/// https://github.com/C-Through/XR-ClimbingLocomotion/blob/main/Assets/Climbing/Scripts/Complete/GravityToggler.cs
/// </summary>
public class GravityToggler : MonoBehaviour
{
    [SerializeField]
    private LocomotionSystem locomotionSystem;

    private ClimbingProvider climbingProvider;
    private ContinuousMoveProviderBase moveProvider;

    private void Awake()
    {
        climbingProvider = locomotionSystem.GetComponent<ClimbingProvider>();
        moveProvider = locomotionSystem.GetComponent<ContinuousMoveProviderBase>();
    }

    private void OnEnable()
    {
        climbingProvider.beginLocomotion += DisableGravity;
        climbingProvider.endLocomotion += EnableGravity;
    }

    private void OnDisable()
    {
        climbingProvider.beginLocomotion -= DisableGravity;
        climbingProvider.endLocomotion -= EnableGravity;
    }

    private void EnableGravity(LocomotionSystem _) => ToggleGravity(true);

    private void DisableGravity(LocomotionSystem _) => ToggleGravity(false);

    private void ToggleGravity(bool value)
    {
        moveProvider.useGravity = value;
    }
}
