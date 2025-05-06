
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.UI;
public class NeighborHandler : MonoBehaviour
{
    private bool _enabledSimilarity;
    [SerializeField] private GameObject _neighborSlider;
    public GameObject imagePrefab; // Prefab containing an Image component

    [SerializeField] string apiUrl = "http://server.selab.edu.vn:20721/explore/explore_neighbor_images"; // API URL for similarity query
    [SerializeField] string imageUrl = "http://server.selab.edu.vn:20716"; // Base URL for images
    [SerializeField] private Transform _neighbortyContainer; // Panel to show similarity slider
    [SerializeField] private FocusImage _focusImage; // Button to toggle similarity

    void Start()
    {
        _enabledSimilarity = false;
    }

    public void OnButtonClick()
    {
        if (_focusImage == null || _focusImage.currentFocus.dataItem.img_link == null)
        {
            Debug.Log("FocusImage or currentFocus is null. Cannot toggle similarity. From NeighborHandler");
            return;
        }

        if (_neighbortyContainer.transform.childCount > 0)
        {
            foreach (Transform child in _neighbortyContainer.transform)
            {
                Destroy(child.gameObject); // Clear previous images
            }
        }

        StartCoroutine(SendImageNeighborQuery(_focusImage.currentFocus.dataItem.img_link));
    }

    IEnumerator SendImageNeighborQuery(string img_link)
    {
        img_link = img_link.Replace("http://server.selab.edu.vn:20716/", "");
        img_link = img_link.Replace(".jpg", "");

        // Prepare request body
        Dictionary<string, object> requestBody = new Dictionary<string, object>
        {
            { "image_url", img_link},
            { "span", 30 },
            { "dataset", "lsc24" }
        };

        string jsonData = JsonConvert.SerializeObject(requestBody);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Response Neighbor: " + jsonResponse);

            // Deserialize the response
            ExploreNeighborImageAPiResponse response = JsonConvert.DeserializeObject<ExploreNeighborImageAPiResponse>(jsonResponse);

            if (response.response == null || response.response.Count == 0)
            {
                Debug.Log("No similar images found in the response.");
                yield break;
            }

            foreach (var item in response.response)
            {
                GameObject newImageObject = Instantiate(imagePrefab, _neighbortyContainer);
                newImageObject.name = $"Image_{item.record_id}";

                Toggle toggleComponent = newImageObject.GetComponent<Toggle>();
                toggleComponent.isOn = false;
                toggleComponent.group = _neighbortyContainer.GetComponent<ToggleGroup>();
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
                    _neighbortyContainer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 5800);
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
}
