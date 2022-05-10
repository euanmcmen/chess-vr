using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerResetScript : MonoBehaviour
{
    private const string resetTriggerTag = "PlayerResetTrigger";
    private const string resetPositionTag = "Respawn";

    private Transform resetPosition;

    private TeleportationProvider teleportationProvider;

    private List<Component> drivingComponents;

    private void Awake()
    {
        resetPosition = GameObject.FindGameObjectWithTag(resetPositionTag).transform;
        teleportationProvider = GetComponentInChildren<TeleportationProvider>();

        drivingComponents = new List<Component>
        {
            GetComponent<CharacterController>(),
            GetComponent<CharacterControllerDriver>(),
            GetComponentInChildren<ContinuousMoveProviderBase>(),
            GetComponentInChildren<ContinuousTurnProviderBase>()
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(resetTriggerTag))
        {
            ResetPlayerPosition();
        }
    }

    private void ResetPlayerPosition()
    {
        ToggleDrivingComponents(false);

        teleportationProvider.QueueTeleportRequest(new TeleportRequest() { destinationPosition = resetPosition.position });

        StartCoroutine(CompleteResetAfterDelay());
    }

    private IEnumerator CompleteResetAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);

        ToggleDrivingComponents(true);
    }

    private void ToggleDrivingComponents(bool enabled)
    {
        foreach (var component in drivingComponents)
        {
            if (component is CharacterController characterControllerComponent)
                characterControllerComponent.enabled = enabled;

            if (component is MonoBehaviour monoBehaviourComponent)
                monoBehaviourComponent.enabled = enabled;
        }
    }
}
