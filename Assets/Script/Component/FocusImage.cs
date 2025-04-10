using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusImage : MonoBehaviour
{
    private ToggleImage currentFocus;
    private Image imageComponent; // Assuming you have an Image component to show the focused item

    void Awake()
    {
        imageComponent = GetComponent<Image>(); // Assuming this script is attached to an Image component
    }

    public void SetFocusedItem(ToggleImage item)
    {
        currentFocus = item;
        Transform imageObject = item.transform.GetChild(1); // Assuming the image is the second child
        Image image = imageObject.GetComponent<Image>(); // Get the Image component from the child
        if (image != null)
        {
            imageComponent.sprite = image.sprite; // Set the focused image sprite
        }

        // Optionally update a UI image, text, etc. here
    }
}