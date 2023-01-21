using UnityEngine.UI;

public abstract class KeypadButtonElementScript : KeypadElementScript
{
    public Button Button => GetComponent<Button>();
}
