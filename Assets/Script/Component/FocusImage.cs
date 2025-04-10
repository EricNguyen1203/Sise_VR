using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusImage : MonoBehaviour
{
    public ToggleImage currentFocus;
    private Image imageComponent; // Assuming you have an Image component to show the focused item

    void Awake()
    {

        imageComponent = GetComponent<Image>(); // Assuming this script is attached to an Image component
    }

    public void SetFocusedItem(ToggleImage item)
    {
        currentFocus = item;
        Debug.LogWarning($"FocusImage: {currentFocus.dataItem.img_link}");
        Transform imageObject = item.transform.GetChild(1); // Assuming the image is the second child
        Image image = imageObject.GetComponent<Image>(); // Get the Image component from the child
        if (image != null)
        {
            imageComponent.sprite = image.sprite; // Set the focused image sprite
        }

        // Optionally update a UI image, text, etc. here
        Debug.LogWarning($"FocusImage After: {currentFocus.dataItem.img_link}");
    }

    // public void ClearFocus()
    // {
    //     currentFocus = null;
    //     imageComponent.sprite = null; // Clear the image when focus is lost
    // }
}