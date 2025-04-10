using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhancedCellView : MonoBehaviour
{
    // --- Assign in Inspector ---
    public TextMeshProUGUI itemLabel; // Example
    // Add other UI element references (Image, Button, etc.)

    // --- Properties ---
    public int CellIndex { get; set; } // Index within the controller's pool (optional, for debugging)
    public int DataIndex { get; private set; } = -1; // Index from the main data source
    public RectTransform RectTransform { get; private set; }

    void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        // Ensure anchor/pivot are set correctly for positioning (e.g., top-center)
        // This setup assumes vertical scrolling where position is set via anchoredPosition.y
        RectTransform.anchorMin = new Vector2(0.5f, 1);
        RectTransform.anchorMax = new Vector2(0.5f, 1);
        RectTransform.pivot = new Vector2(0.5f, 1);
    }

    // Configure the cell for a specific data index and update its UI
    public void ConfigureCell(int dataIndex, YourItemData data)
    {
        DataIndex = dataIndex;
        // Update visual elements based on the data
        if (itemLabel != null)
        {
            itemLabel.text = data.displayText;
        }
        // Update other UI elements (images, buttons, etc.)
        // e.g., itemIcon.sprite = data.iconSprite;

        // Ensure the GameObject itself is active if it wasn't
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    // Set the vertical position of the cell within the content RectTransform
    public void SetPosition(float yPosition)
    {
        // Y position is usually negative downwards from the top anchor
        RectTransform.anchoredPosition = new Vector2(0, yPosition);
    }

    // Deactivate the cell when it's no longer needed
    public void Deactivate()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        DataIndex = -1; // Reset data index when deactivated
    }
}

// --- Dummy Data Structure (reuse from previous examples) ---
public class YourItemData
{
    public string displayText;
    public int value;
    public YourItemData(string text, int val) { displayText = text; value = val; }
    // Add other fields (icon sprite, color, etc.) as needed
}