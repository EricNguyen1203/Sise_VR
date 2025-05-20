
using Oculus.Interaction;

public class ButtonEyeInteractable : EyeInteractable
{
    private InteractableUnityEventWrapper _eventWrapper;

    private void Start()
    {
        _eventWrapper = GetComponent<InteractableUnityEventWrapper>();
    }

    public override void Select(bool state, GazeImageFeedback feedback = null)
    {
        base.Select(state);

        if (state && _eventWrapper != null)
        {
            _eventWrapper.WhenSelect.Invoke();
        }
    }
}