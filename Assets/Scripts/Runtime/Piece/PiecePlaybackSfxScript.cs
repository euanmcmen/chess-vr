using UnityEngine;

public class PiecePlaybackSfxScript : MonoBehaviour
{
    private PieceConfigDataScript pieceConfigDataScript;
    private AudioSource pieceAudioSource;

    private void Awake()
    {
        pieceConfigDataScript = GetComponent<PieceConfigDataScript>();
        pieceAudioSource = GetComponent<AudioSource>();
    }

    public void PlayLandingSoundEffect()
    {
        pieceAudioSource.PlayOneShot(pieceConfigDataScript.PieceConfig.PieceMovementFinishedSoundEffect);
    }
}
