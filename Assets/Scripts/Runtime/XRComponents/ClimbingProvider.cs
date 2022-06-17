using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbingProvider : LocomotionProvider
{
    [SerializeField]
    private CharacterController characterController;

    private bool isClimbing;
    private List<ControllerVelocity> activeControllers;

    protected override void Awake()
    {
        base.Awake();
        isClimbing = false;
        activeControllers = new List<ControllerVelocity>();
    }

    public void AddProvider(ControllerVelocity controllerVelocity)
    {
        if (!activeControllers.Contains(controllerVelocity))
            activeControllers.Add(controllerVelocity);
    }

    public void RemoveProvider(ControllerVelocity controllerVelocity)
    {
        if (activeControllers.Contains(controllerVelocity))
            activeControllers.Remove(controllerVelocity);
    }

    private void Update()
    {
        if (CanClimb() && BeginLocomotion())
            isClimbing = true;

        if (isClimbing)
            ApplyVelocity();

        if (!CanClimb() && EndLocomotion())
            isClimbing = false;
    }

    private bool CanClimb()
    {
        return activeControllers.Count != 0;
    }

    private void ApplyVelocity()
    {
        var controllerVelocity = CalculateVelocityOfAllActiveControllers();
        var origin = system.xrOrigin.transform;

        var localOrientationVelocity = origin.TransformDirection(controllerVelocity);
        localOrientationVelocity *= Time.deltaTime;

        characterController.Move(-localOrientationVelocity);
    }

    private Vector3 CalculateVelocityOfAllActiveControllers()
    {
        Vector3 totalVelocity = Vector3.zero;

        foreach (ControllerVelocity controllerVelocity in activeControllers)
        {
            totalVelocity += controllerVelocity.Velocity;
        }

        return totalVelocity;
    }
}
