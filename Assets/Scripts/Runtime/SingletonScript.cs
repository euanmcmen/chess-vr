using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonScript : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour singletonComponent;

    private void Awake()
    {
        var allInstances = FindObjectsOfType(singletonComponent.GetType());
        if (allInstances.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
