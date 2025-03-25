using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class VoskDialogText : MonoBehaviour 
{
    public VoskSpeechToText VoskSpeechToText;
    public Text DialogText;

    Regex hi_regex = new Regex(@"hello");
    Regex who_regex = new Regex(@"who are you");
    Regex pass_regex = new Regex(@"(okay|let's go)");
    Regex help_regex = new Regex(@"help");

    Regex goat_regex = new Regex(@"(goat|take the goat)");
    Regex wolf_regex = new Regex(@"(wolf|take the wolf)");
    Regex cabbage_regex = new Regex(@"(cabbage|take the cabbage)");

    Regex goat_back_regex = new Regex(@"(goat back|return the goat)");
    Regex wolf_back_regex = new Regex(@"(wolf back|return the wolf)");
    Regex cabbage_back_regex = new Regex(@"(cabbage back|return the cabbage)");

    Regex forward_regex = new Regex(@"move forward");
    Regex back_regex = new Regex(@"(go back|return back)");

    // State
    bool goat_left;
    bool wolf_left;
    bool cabbage_left;
    bool man_left;

    void Awake()
    {
        VoskSpeechToText.OnTranscriptionResult += OnTranscriptionResult;
        ResetState();
    }

    void ResetState()
    {
        goat_left = true;
        wolf_left = true;
        cabbage_left = true;
        man_left = true;
    }

    void CheckState() {
        if (goat_left && wolf_left && !man_left) {
            AddFinalResponse("The wolf ate the goat, start over.");
            return;
        }
        if (goat_left && cabbage_left && !man_left) {
            AddFinalResponse("The goat ate the cabbage, start over.");
            return;
        }
        if (!goat_left && !wolf_left && man_left) {
            AddFinalResponse("The wolf ate the goat, start over.");
            return;
        }
        if (!goat_left && !cabbage_left && man_left) {
            AddFinalResponse("The goat ate the cabbage, start over.");
            return;
        }
        if (!goat_left && !wolf_left && !cabbage_left && !man_left) {
            AddFinalResponse("Well done! Try again?");
            return;
        }

        AddResponse("Okay, what next?");
    }

    void Say(string response)
    {
        System.Diagnostics.Process.Start("/usr/bin/say", response); 
    }

    void AddFinalResponse(string response) {
        Say(response);
        DialogText.text = response + "\n";
        ResetState();
    }

    void AddResponse(string response) {
        Say(response);
        DialogText.text = response + "\n\n";

        DialogText.text += "Farmer is " + (man_left ? "on the left" : "on the right") + "\n";
        DialogText.text += "Wolf is " + (wolf_left ? "on the left" : "on the right") + "\n";
        DialogText.text += "Goat is " + (goat_left ? "on the left" : "on the right") + "\n";
        DialogText.text += "Cabbage is " + (cabbage_left ? "on the left" : "on the right") + "\n\n";
    }

    private void OnTranscriptionResult(string obj)
    {
        Debug.Log(obj);
        var result = new RecognitionResult(obj);
        foreach (RecognizedPhrase p in result.Phrases)
        {
            if (hi_regex.IsMatch(p.Text)) {
                AddResponse("Hello to you!");
                return;
            }
            if (who_regex.IsMatch(p.Text)) {
                AddResponse("I am a teaching robot.");
                return;
            }
            if (pass_regex.IsMatch(p.Text)) {
                AddResponse("Great!");
                return;
            }
            if (help_regex.IsMatch(p.Text)) {
                AddResponse("Think for yourself!");
                return;
            }
            if (goat_back_regex.IsMatch(p.Text)) {
                if (goat_left) {
                    AddResponse("The goat is still on the left bank.");
                } else if (man_left) {
                    AddResponse("The farmer is still on the left bank.");
                } else {
                    goat_left = true;
                    man_left = true;
                    CheckState();
                }
                return;
            }
            if (wolf_back_regex.IsMatch(p.Text)) {
                if (wolf_left) {
                    AddResponse("The wolf is still on the left bank.");
                } else if (man_left) {
                    AddResponse("The farmer is still on the left bank.");
                } else {
                    wolf_left = true;
                    man_left = true;
                    CheckState();
                }
                return;
            }
            if (wolf_regex.IsMatch(p.Text)) {
                if (!wolf_left) {
                    AddResponse("The wolf is already on the right bank.");
                } else if (!man_left) {
                    AddResponse("The farmer is already on the right bank.");
                } else {
                    wolf_left = false;
                    man_left = false;
                    CheckState();
                }
                return;
            }
            if (goat_regex.IsMatch(p.Text)) {
                if (!goat_left) {
                    AddResponse("The goat is already on the right bank.");
                } else if (!man_left) {
                    AddResponse("The farmer is already on the right bank.");
                } else {
                    goat_left = false;
                    man_left = false;
                    CheckState();
                }
                return;
            }
            if (cabbage_regex.IsMatch(p.Text)) {
                if (!cabbage_left) {
                    AddResponse("The cabbage is already on the right bank.");
                } else if (!man_left) {
                    AddResponse("The farmer is already on the right bank.");
                } else {
                    cabbage_left = false;
                    man_left = false;
                    CheckState();
                }
                return;
            }
        }
        if (result.Phrases.Length > 0 && result.Phrases[0].Text != "") {
            AddResponse("I don't understand. Try again.");
        }
    }
}
