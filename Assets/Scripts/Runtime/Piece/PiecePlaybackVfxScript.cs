using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PiecePlaybackVfxScript : MonoBehaviour
{
    private PieceConfigDataScript pieceConfigDataScript;
    private VisualEffect visualEffect;

    private void Awake()
    {
        pieceConfigDataScript = GetComponent<PieceConfigDataScript>();
        visualEffect = GetComponent<VisualEffect>();
    }

    public void PlayLandingVisualEffect()
    {
        visualEffect.Play();
    }
}
