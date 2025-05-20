using UnityEngine;

public class ToggleEyeInteractable : EyeInteractable
{
    private ToggleImage _toggleImage;

    private void Start()
    {
        _toggleImage = GetComponent<ToggleImage>();
    }

    public override void Select(bool state, GazeImageFeedback feedback = null)
    {
        this.IsSelected = state;

        if (state && feedback != null && _toggleImage != null)
        {
            feedback.GetImage(_toggleImage);
        }
    }
}
