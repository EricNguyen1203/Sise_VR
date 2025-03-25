using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VoskResultText : MonoBehaviour 
{
    public VoskSpeechToText VoskSpeechToText;
    public TMP_Text ResultText;

    public TMP_InputField inputField;

    void Awake()
    {
        VoskSpeechToText.OnTranscriptionResult += OnTranscriptionResult;
    }

    private void OnTranscriptionResult(string obj)
    {
        Debug.Log(obj);
        var result = new RecognitionResult(obj);
        // for (int i = 0; i < result.Phrases.Length; i++)
        // {
        //     if (i > 0)
        //     {
        //         ResultText.text += ", ";
        //     }

        //     ResultText.text += result.Phrases[i].Text;
        // }
        // if (result.Phrases.Length > 0)
        // {
        //     ResultText.text += result.Phrases[0].Text;
        //     ResultText.text += "\n\n";
        //     VoskSpeechToText.ToggleRecording();
        // }

        if (result.Phrases.Length > 0)
        {
            if (inputField == null)
            {
                Debug.LogError("InputField is not set");
                return;
            }
            if (inputField.text.Length > 0)
            {
                inputField.text = "";
            }

            if (inputField.placeholder != null)
            {
                inputField.placeholder.GetComponent<TMP_Text>().text = "";
            }
            inputField.text = result.Phrases[0].Text;
            VoskSpeechToText.ToggleRecording();
        }
    }
}
