using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PieceLandingFxScript : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        GetComponent<AudioSource>().Play();
        GetComponent<VisualEffect>().Play();

        yield return new WaitForSeconds(5);

        Realtime.Destroy(gameObject);
    }
}
