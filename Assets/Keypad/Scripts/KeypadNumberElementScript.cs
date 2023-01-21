using TMPro;

public class KeypadNumberElementScript : KeypadButtonElementScript
{
    public string Number => GetComponentInChildren<TMP_Text>().text;

    public override KeypadElements KeyElement => KeypadElements.Number;
}
