using TMPro;
using UnityEngine;

public class TextMeshProVirtualKeyboardInputSource : MonoBehaviour
{
    [SerializeField]
    private OVRVirtualKeyboard virtualKeyboard;

    [SerializeField]
    private TMP_InputField inputField;

    void Start()
    {
        inputField.onSelect.AddListener(OnInputFieldSelect);
        inputField.onValueChanged.AddListener(OnInputFieldValueChange);
        virtualKeyboard.CommitTextEvent.AddListener(OnCommitText);
        virtualKeyboard.BackspaceEvent.AddListener(OnBackspace);
        virtualKeyboard.KeyboardHiddenEvent.AddListener(OnKeyboardHidden);
    }

    public void OnInputFieldValueChange(string arg0)
    {
        Debug.Log("OnInputFieldValueChange: " + arg0);
        virtualKeyboard.ChangeTextContext(arg0);
    }

    public void OnInputFieldSelect(string arg0)
    {
        // inputField.Select();
        // inputField.ActivateInputField();
        virtualKeyboard.ChangeTextContext(inputField.text);
        virtualKeyboard.gameObject.SetActive(true);
    }

    public void OnKeyboardHidden()
    {
        if (!inputField.isFocused)
        {
            return;
        }
        // if the user hides the keyboard
        inputField.DeactivateInputField();
    }

    public void OnCommitText(string arg0)
    {

        if (!inputField.isFocused)
        {
            return;
        }
        inputField.onValueChanged.RemoveListener(OnInputFieldValueChange);
        if (arg0 == "\n" && !inputField.multiLine)
        {
            inputField.OnSubmit(null);
        }
        Debug.Log("OnCommitText: " + arg0);
        // inputField.SetTextWithoutNotify(inputField.text + arg0);
        inputField.text += arg0;
        Debug.Log("inputField.text: " + inputField.text);
        inputField.MoveTextEnd(false);
        inputField.onValueChanged.AddListener(OnInputFieldValueChange);
    }


    public void OnBackspace()
    {
        Debug.Log("OnBackspace");
        if (!inputField.isFocused)
        {
            return;
        }
        if (inputField.text.Length > 0)
        {
            inputField.onValueChanged.RemoveListener(OnInputFieldValueChange);
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
            inputField.MoveTextEnd(false);
            inputField.onValueChanged.AddListener(OnInputFieldValueChange);
        }
    }

    public void OnChangeTextField(TMP_InputField newInputField)
    {
        // Debug.Log("OnChangeTextField: " + inputField.text);
        inputField = newInputField;
    }
}