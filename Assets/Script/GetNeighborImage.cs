using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class ImageGridSpawner : MonoBehaviour
{
    public GameObject imagePrefab; // Prefab containing an Image component
    public Transform parentContainer; // Parent object to hold images
    public int totalImages = 1000; // Number of images to create
    public List<string> imageUrls; // List of URLs to fetch images from

    private void Start()
    {
        if (imagePrefab == null || parentContainer == null || imageUrls.Count == 0)
        {
            Debug.LogError("Missing references! Make sure prefab, parent, and URLs are set.");
            return;
        }

        StartCoroutine(SpawnImages());
    }

    IEnumerator SpawnImages()
    {
        for (int i = 0; i < totalImages; i++)
        {
            GameObject newImageObject = Instantiate(imagePrefab, parentContainer);
            newImageObject.name = $"Image_{i}";
            
            Toggle toggleComponent = newImageObject.GetComponent<Toggle>();
            toggleComponent.isOn = false;
            toggleComponent.group = parentContainer.GetComponent<ToggleGroup>();

            Transform imageObject = newImageObject.transform.GetChild(1);

            Image imageComponent = imageObject.GetComponent<Image>();

            if (imageComponent != null)
            {
                string imageUrl = imageUrls[i % imageUrls.Count]; // Loop URLs if needed
                StartCoroutine(LoadImageFromURL(imageUrl, imageComponent));
            }

            yield return null; // Wait one frame to avoid freezing
        }
    }

    IEnumerator LoadImageFromURL(string url, Image targetImage)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                targetImage.sprite = sprite;
            }
            else
            {
                Debug.LogError($"Failed to load image: {url}, Error: {request.error}");
            }
        }
    }
}
