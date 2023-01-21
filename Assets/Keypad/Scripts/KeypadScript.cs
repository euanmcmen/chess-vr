using UnityEngine;
using UnityEngine.Events;

public class KeypadScript : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_InputField inputText;

    [Space]
    [SerializeField]
    private UnityEvent<string> OnSubmit;

    [Space]
    [SerializeField]
    private int characterLimit = 0;

    //private List<KeypadElementScript> elements;
    private GameObject keypadControls;

    private void Awake()
    {
        //elements = GetComponentsInChildren<KeypadElementScript>().ToList();
        keypadControls = GetComponentInChildren<KeypadRootElementScript>().gameObject;

        BindButtons();

        inputText.onSelect.AddListener((x) => keypadControls.SetActive(true));

        keypadControls.SetActive(false);
    }

    private void BindButtons()
    {
        foreach (var element in GetComponentsInChildren<KeypadNumberElementScript>())
        {
            //var elementButton = element.GetComponent<Button>();
            //var text = elementButton.GetComponentInChildren<TMP_Text>().text;
            element.Button.onClick.AddListener(() => InsertCharacter(element.Number));
        }

        GetComponentInChildren<KeypadSubmitElementScript>().Button.onClick.AddListener(Submit);
        GetComponentInChildren<KeypadBackspaceElementScript>().Button.onClick.AddListener(Backspace);
        GetComponentInChildren<KeypadSubmitElementScript>().Button.onClick.AddListener(Cancel);
    }

    private void Submit()
    {
        Debug.LogFormat("Submit: {0}", inputText.text);
        OnSubmit.Invoke(inputText.text);
        keypadControls.SetActive(false);
    }

    private void InsertCharacter(string text)
    {
        if (characterLimit > 0 && inputText.text.Length >= characterLimit)
        {
            return;
        }

        inputText.text += text;
    }

    private void Backspace()
    {
        if (inputText.text.Length == 0)
        {
            return;
        }

        inputText.text = inputText.text[..^1];
    }

    private void Cancel()
    {
        keypadControls.SetActive(false);
        inputText.text = string.Empty;
    }
}
