using UnityEngine;
using UnityEngine.UI;

public class FisheyeGridPlacer : MonoBehaviour
{
    public RectTransform parentCanvas;  // Assign UI Canvas
    public GameObject rectanglePrefab;  // Assign a UI Panel Prefab

    public int latitudeCount = 5;  // Number of latitude lines
    public int longitudeCount = 8; // Number of longitude slices
    public float radius = 200f;    // Main circle radius
    public float wMax = 40f;       // Max width of panels
    public float lMax = 40f;       // Max height of panels

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int lat = -latitudeCount; lat <= latitudeCount; lat++)
        {
            // Apply fisheye effect to latitude distribution
            float t = (lat + latitudeCount) / (float)(2 * latitudeCount);
            float latitudeAngle = Mathf.Asin(2 * t - 1) * Mathf.Rad2Deg; // Fisheye warping
            float latRadius = Mathf.Cos(latitudeAngle * Mathf.Deg2Rad) * radius;
            float yPos = Mathf.Sin(latitudeAngle * Mathf.Deg2Rad) * radius;

            for (int lon = 0; lon < longitudeCount; lon++)
            {
                float longitudeAngle = (lon / (float)(longitudeCount - 1)) * 360f;
                float xPos = Mathf.Cos(longitudeAngle * Mathf.Deg2Rad) * latRadius;

                // Adjust size dynamically based on fisheye effect
                float dX = Mathf.Abs(xPos) / radius;
                float dY = Mathf.Abs(yPos) / radius;
                float width = wMax * (1 - 0.8f * dX * dX);
                float height = lMax * (1 - 0.8f * dY * dY);

                CreateRectangle(new Vector2(xPos, yPos), width, height);
            }
        }
    }

    void CreateRectangle(Vector2 position, float width, float height)
    {
        GameObject rectObj = Instantiate(rectanglePrefab, parentCanvas.transform);
        RectTransform rectTransform = rectObj.GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.anchoredPosition = position;
    }
}
