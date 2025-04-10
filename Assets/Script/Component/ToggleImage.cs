using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleImage : MonoBehaviour
{
    public Toggle _toggle;

    [HideInInspector] public DataItem dataItem;

    public FocusImage focusImage;

    private void Awake()
    {
        if (_toggle == null)
            _toggle = GetComponent<Toggle>();
       
    }

    public void Setup(DataItem item)
    {
        // Set up the toggle with the data item and reference to the focus image
        // This is where you would set up the UI elements, e.g., setting the image source, etc.
        // For example:
        // imageComponent.sprite = item.imageSprite; // Assuming you have a reference to an Image component

        // Store references for later us
        dataItem = item;

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
            Debug.Log($"FocusImage Toggle: {focusImage.currentFocus.dataItem.img_link}");
        }
    }
}
