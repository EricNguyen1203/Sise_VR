using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleImage : MonoBehaviour
{
    private Toggle _toggle;

    [HideInInspector] public DataItem dataItem;

    private FocusImage focusImage;

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
    }

    public void Setup(DataItem item, FocusImage focusRef)
    {
        // Set up the toggle with the data item and reference to the focus image
        // This is where you would set up the UI elements, e.g., setting the image source, etc.
        // For example:
        // imageComponent.sprite = item.imageSprite; // Assuming you have a reference to an Image component

        // Store references for later us
        dataItem = item;
        focusImage = focusRef;

        // Avoid duplicate listeners if reused
        _toggle.onValueChanged.RemoveAllListeners();
        _toggle.onValueChanged.AddListener(OnToggleChanged);

    }

    private void OnToggleChanged(bool isOn)
    {
        // Debug.LogWarning($"Toggle changed: {isOn}");
        if (isOn && focusImage != null)
        {
            focusImage.SetFocusedItem(this);
        }
    }
}
