using Unity.XR.CoreUtils;
using UnityEngine;

/// <summary>
/// https://github.com/C-Through/XR-ClimbingLocomotion/blob/main/Assets/Climbing/Scripts/Complete/CharacterDriver.cs
/// </summary>
/// 
public class CharacterDriver : MonoBehaviour
{
    private XROrigin origin;
    private CharacterController controller;

    private void Awake()
    {
        origin = GetComponent<XROrigin>();
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        UpdateController();
    }

    private void UpdateController()
    {
        // Get the height of the player
        float height = Mathf.Clamp(origin.CameraInOriginSpaceHeight, 1, 2);

        // Cut in half, add skin
        Vector3 center = origin.CameraInOriginSpacePos;
        center.y = (height / 2.0f) + controller.skinWidth;

        // Apply
        controller.height = height;
        controller.center = center;
    }
}
