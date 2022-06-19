using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceVelocityScript : MonoBehaviour
{
    private Vector3 current;

    private Vector3 previous;

    public Vector3 GetVelocity()
    {
        return (current - previous) / Time.deltaTime;
    }

    void LateUpdate()
    {
        previous = current;
        current = transform.position;
    }
}
