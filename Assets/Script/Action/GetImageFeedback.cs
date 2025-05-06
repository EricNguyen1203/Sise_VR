using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Ensure you have the UnityEngine.UI namespace for UI components
using UnityEngine.Networking;

public class ImageFeedbackHandler : MonoBehaviour
{
    public GameObject imagePrefab; // Prefab containing an Image component
    public Transform _container; // Panel to show similarity slider
    public Transform _otherContainer; // Panel to show similarity slider
    [SerializeField] private FocusImage _focusImage; // Button to toggle similarity

    public void GetImage()
    {
        if (_focusImage == null || _focusImage.currentFocus == null || _focusImage.currentFocus.dataItem.img_link == "") 
        {
            Debug.Log("FocusImage or currentFocus is null. Cannot toggle similarity.");
            Debug.Log("Check focusImage", _focusImage);
            Debug.Log("Check currentFocus", _focusImage.currentFocus);
            return;
        }

        Debug.Log("GetImageFeedback: " + _focusImage.currentFocus.dataItem.img_link);

        string img_link = _focusImage.currentFocus.dataItem.img_link;
        img_link = img_link.Replace("http://server.selab.edu.vn:20716/", "http://10.0.1.21:20716/");
        if(CheckToggleExists(img_link, _container))
        {
            Debug.Log("Toggle already exists. No need to create a new one.");
            return;
        }
        Debug.LogWarning($"GetImageFeedback: {img_link}");
        GameObject newImage = Instantiate(imagePrefab, _container);

        newImage.GetComponent<ToggleImage>().Setup(_focusImage.currentFocus.dataItem); // Set up the toggle with the data item
        newImage.GetComponent<ToggleImage>().focusImage = _focusImage; // Set the focus image reference

        Transform imageObject = newImage.transform.GetChild(1);

        Image imageComponent = imageObject.GetComponent<Image>();

        if (imageComponent != null)
        {
            // Assuming you have a method to load the image from the URL
            StartCoroutine(LoadImageFromUrl(img_link, imageComponent));
        }
    }

    IEnumerator LoadImageFromUrl(string url, Image imageComponent)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error loading image: " + request.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                imageComponent.sprite = sprite;
            }
        }
    }

    bool CheckToggleExists(string img_link, Transform Container)
    {   
        // Check if the toggle already exists in the container
        foreach (Transform child in Container)
        {
            ToggleImage toggleImage = child.GetComponent<ToggleImage>();
            Debug.LogWarning($"CheckToggleExists: {toggleImage.dataItem.img_link}");
            string imageUrl = toggleImage.dataItem.img_link.Replace("http://server.selab.edu.vn:20716/", "http://10.0.1.21:20716/");
            if (toggleImage != null && imageUrl == img_link)
            {
                Debug.Log("Toggle already exists. No need to create a new one.");
                return true;
            }
        }
        return false;
    }
}
