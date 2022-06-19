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
    private List<ControllerVelocity> activeClimbingControllers;
    private PieceVelocityScript activeClimbingTarget;

    protected override void Awake()
    {
        base.Awake();
        isClimbing = false;
        activeClimbingTarget = null;
        activeClimbingControllers = new List<ControllerVelocity>();
    }

    public void AddTarget(GameObject target)
    {
        if (activeClimbingTarget != null)
        {
            return;
        }

        activeClimbingTarget = target.GetComponent<PieceVelocityScript>();
    }

    public void AddProvider(ControllerVelocity controllerVelocity)
    {
        if (!activeClimbingControllers.Contains(controllerVelocity))
        {
            activeClimbingControllers.Add(controllerVelocity);
        }
    }

    public void RemoveProvider(ControllerVelocity controllerVelocity)
    {
        if (activeClimbingControllers.Contains(controllerVelocity))
        {
            activeClimbingControllers.Remove(controllerVelocity);
        }            
    }

    private void Update()
    {
        BeginClimb();

        if (isClimbing)
        {
            ApplyVelocity();
        }

        EndClimb();
    }

    private void BeginClimb()
    {
        if (CanClimb() && BeginLocomotion())
        {
            isClimbing = true;
        }
    }

    private void EndClimb()
    {
        if (!CanClimb() && EndLocomotion())
        {
            isClimbing = false;
            activeClimbingTarget = null;
        }
    }

    private bool CanClimb()
    {
        return activeClimbingControllers.Count != 0;
    }

    private void ApplyVelocity()
    {
        var controllerVelocity = CalculateVelocityOfAllActiveControllers();
        var worldControllerVelocity = system.xrOrigin.transform.TransformDirection(controllerVelocity);
        var inverseWorldControllerVelocity = (-worldControllerVelocity);

        var pieceMovementVelocity = activeClimbingTarget.GetVelocity();

        characterController.Move((pieceMovementVelocity + inverseWorldControllerVelocity) * Time.deltaTime);
    }

    private Vector3 CalculateVelocityOfAllActiveControllers()
    {
        Vector3 totalVelocity = Vector3.zero;

        foreach (ControllerVelocity controllerVelocity in activeClimbingControllers)
        {
            totalVelocity += controllerVelocity.Velocity;
        }

        return totalVelocity;
    }
}
