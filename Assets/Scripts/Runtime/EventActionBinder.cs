using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class EventActionBinder
{
    public static void BindSubscribersToAction<TSubscriber>(Action<TSubscriber> action)
    {
        var implementations = GameObject.FindObjectsOfType<MonoBehaviour>().Where(x => x is TSubscriber).Cast<TSubscriber>().ToList();
        foreach (var implementation in implementations)
        {
            action(implementation);
        }

        Debug.LogFormat("Number of listeners for type {0}: {1}", typeof(TSubscriber).Name, implementations.Count);
    }
}