using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ScrollViewCenterDetector : MonoBehaviour
{
    public ScrollRect scrollRect; // Assign in Inspector
    public RectTransform content; // Assign in Inspector
    public RectTransform viewport; // Assign in Inspector

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        Transform closestElement = GetCenterElement();

        Transform imageObject = closestElement.transform.GetChild(1);

        if (closestElement != null)
        {
            Debug.Log("Centered Item: " + closestElement.name);
            image.sprite = imageObject.GetComponent<Image>().sprite;
        }
    }

    Transform GetCenterElement()
    {
        Vector3 viewportCenter = viewport.position;

        Transform closest = null;
        float closestDistance = float.MaxValue;

        foreach (RectTransform child in content)
        {
            Vector3 childPos = child.position;
            float distance = Vector3.Distance(viewportCenter, childPos);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = child;
            }
        }

        return closest;
    }
}
