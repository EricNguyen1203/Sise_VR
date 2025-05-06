using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.UI; // Ensure you have the UnityEngine.UI namespace for UI components

public class SimilarHandler : MonoBehaviour
{
    private bool _enabledSimilarity;
    [SerializeField] private GameObject _similaritySlider;
    public GameObject imagePrefab; // Prefab containing an Image component

    [SerializeField] string apiUrl = "http://server.selab.edu.vn:20721/explore/explore_similar_images"; // API URL for similarity query
    [SerializeField] string imageUrl = "http://server.selab.edu.vn:20716"; // Base URL for images
    [SerializeField] private Transform _similarityContainer; // Panel to show similarity slider
    [SerializeField] private FocusImage _focusImage; // Button to toggle similarity
    [SerializeField] private int _getTopK; // Button to toggle similarity
    void Start()
    {
        _enabledSimilarity = false;
    }

    public void OnButtonClick()
    {
        if (_focusImage == null || _focusImage.currentFocus == null)
        {
            Debug.Log("FocusImage or currentFocus is null. Cannot toggle similarity.");
            Debug.Log("Check focusImage", _focusImage);
            Debug.Log("Check currentFocus", _focusImage.currentFocus);
            return;
        }

        if (_similarityContainer.transform.childCount > 0)
        {
            foreach (Transform child in _similarityContainer.transform)
            {
                Destroy(child.gameObject); // Clear previous images
            }
        }

        StartCoroutine(SendImageSimilarityQuery(_focusImage.currentFocus.dataItem.img_link));
    }

    IEnumerator SendImageSimilarityQuery(string img_link)
    {

        Debug.Log("Sending image similarity query to: " + apiUrl);
        img_link = img_link.Replace("http://server.selab.edu.vn:20716/", "http://10.0.1.21:20716/");
        // Prepare request body
        Dictionary<string, object> requestBody = new Dictionary<string, object>
        {
            { "image_urls", new List<string> { img_link } },
            { "model", "clips" },
            { "dataset", "lsc24" }
        };

        string jsonData = JsonConvert.SerializeObject(requestBody);

        // Create UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {request.error}");
            yield break;
        }

        if (request.result == UnityWebRequest.Result.Success)
        {

            // Deserialize the response
            ExploreSimilarImageAPiResponse response = JsonConvert.DeserializeObject<ExploreSimilarImageAPiResponse>(request.downloadHandler.text);

            if (response.response == null || response.response.Count == 0)
            {
                Debug.Log("No similar images found in the response.");
                yield break;
            }

            foreach (var item in response.response.Take(_getTopK))
            {
                GameObject newImageObject = Instantiate(imagePrefab, _similarityContainer);
                newImageObject.name = $"Image_{item.record_id}";

                Toggle toggleComponent = newImageObject.GetComponent<Toggle>();
                toggleComponent.isOn = false;
                toggleComponent.group = _similarityContainer.GetComponent<ToggleGroup>();
                // toggleComponent.onValueChanged.AddListener((isOn) => OnToggleChanged(toggleComponent));

                ToggleImage toggleImageComponent = newImageObject.GetComponent<ToggleImage>();
                toggleImageComponent.Setup(item); // Pass the item to the ToggleImage component
                toggleImageComponent.focusImage = _focusImage; // Pass the reference to FocusImage

                Transform imageObject = newImageObject.transform.GetChild(1);

                Image imageComponent = imageObject.GetComponent<Image>();

                if (imageComponent != null)
                {

                    string imageUrl = item.img_link; // Use the image link from the API response
                    imageUrl = imageUrl.Replace("http://server.selab.edu.vn:20716/", this.imageUrl); // Replace with the base URL for images

                    StartCoroutine(LoadImage(imageUrl, imageComponent));
                }

                yield return null; // Wait one frame to avoid freezing
            }
        }
    }

    IEnumerator LoadImage(string imageUrl, Image imageComponent)
    {
        using (UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return imageRequest.SendWebRequest();

            if (imageRequest.result == UnityWebRequest.Result.ConnectionError || imageRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error loading image: {imageRequest.error}");
                yield break;
            }

            if (imageRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                imageComponent.sprite = sprite;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
