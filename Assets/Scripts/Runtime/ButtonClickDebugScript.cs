using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickDebugScript : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    [ContextMenu("Send Click")]
    public void SendClickEvent()
    {
        Debug.LogFormat("{0} - Sending click to attached button.", gameObject.name);
        button.onClick.Invoke();
    }
}
