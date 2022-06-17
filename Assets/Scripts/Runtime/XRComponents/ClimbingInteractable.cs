using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbingInteractable : XRBaseInteractable
{
    //[SerializeField] 
    //private ClimbingProvider climbingProvider;

    protected override void Awake()
    {
        base.Awake();
        //FindClimbingProvider();
    }

    //private void FindClimbingProvider()
    //{
    //    if (climbingProvider == null)
    //        climbingProvider = FindObjectOfType<ClimbingProvider>();
    //}

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        TryAddInteractor(args.interactorObject);
    }

    private void TryAddInteractor(IXRSelectInteractor interactor)
    {
        if (interactor.transform.TryGetComponent(out ControllerVelocity controllerVelocity))
        {
            interactor.transform.GetComponentInParent<ClimbingProviderResolver>()
                .GetClimbingProvider()
                .AddProvider(controllerVelocity);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        TryRemoveInteractor(args.interactorObject);
    }

    private void TryRemoveInteractor(IXRSelectInteractor interactor)
    {
        if (interactor.transform.TryGetComponent(out ControllerVelocity controllerVelocity))
        {
            interactor.transform.GetComponentInParent<ClimbingProviderResolver>()
                .GetClimbingProvider()
                .RemoveProvider(controllerVelocity);
        }
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        return base.IsSelectableBy(interactor) && interactor is XRDirectInteractor;
    }

    public override bool IsHoverableBy(IXRHoverInteractor interactor)
    {
        return base.IsHoverableBy(interactor) && interactor is XRDirectInteractor;
    }
}
