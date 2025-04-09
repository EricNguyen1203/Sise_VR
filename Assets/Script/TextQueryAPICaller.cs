using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro; // if you're using TextMeshPro
using Newtonsoft.Json; // Add Newtonsoft.Json via Unity Package Manager or NuGet
using System.Linq;

public class TextQueryAPICaller : MonoBehaviour
{
    public TMP_InputField inputField; // Drag your input field (named "Text") here
    public string apiUrl = "http://server.selab.edu.vn:20711/search/search_with_text_query";
    public GameObject imagePrefab; // Prefab containing an Image component
    public Transform parentContainer; // Parent object to hold images
    public Image imageFocusScreen;

    [System.Serializable]
    public class Neighbor
    {
        public int? id;
        public string date;
        public string time;
        public float? new_lat;
        public float? new_lng;
        public string location_displayed;
        public string img_link;
        public float? score;
    }

    [System.Serializable]
    public class DataItem
    {
        public int? id;
        public string date;
        public string time;
        public float? new_lat;
        public float? new_lng;
        public string location_displayed;
        public string img_link;
        public float? score;
        public List<Neighbor> neighbors;
    }

    [System.Serializable]
    public class ApiResponse
    {
        public int status;
        public string message;
        public List<DataItem> data;
    }

    public void OnSubmit()
    {
        string userInput = inputField.text;
        StartCoroutine(SendTextQuery(userInput));
    }

    IEnumerator SendTextQuery(string text)
    {
        // Prepare request body
        Dictionary<string, string> requestBody = new Dictionary<string, string>
        {
            { "mode", "vec" },
            { "model", "clips" },
            { "text_query", text },
            { "user_id", "quan" },
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

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);

            // Deserialize the response
            ApiResponse response = JsonConvert.DeserializeObject<ApiResponse>(request.downloadHandler.text);

            if (response.status == 200)
            {

                foreach (var item in response.data.Take(100))
                {
                    GameObject newImageObject = Instantiate(imagePrefab, parentContainer);
                    newImageObject.name = $"Image_{item.id}";

                    Toggle toggleComponent = newImageObject.GetComponent<Toggle>();
                    toggleComponent.isOn = false;
                    toggleComponent.group = parentContainer.GetComponent<ToggleGroup>();
                    toggleComponent.onValueChanged.AddListener((isOn) => OnToggleChanged(toggleComponent));

                    Transform imageObject = newImageObject.transform.GetChild(1);

                    Image imageComponent = imageObject.GetComponent<Image>();

                    if (imageComponent != null)
                    {
                        
                        string imageUrl = item.img_link; // Use the image link from the API response
                        imageUrl = imageUrl.Replace("http://127.0.0.1:8000", "http://server.selab.edu.vn:20716");

                        StartCoroutine(LoadImageFromURL(imageUrl, imageComponent));
                    }

                    yield return null; // Wait one frame to avoid freezing
                }
            }
            else
            {
                Debug.LogWarning("API responded with status: " + response.status);
            }
        }
        else
        {
            Debug.LogError($"Error: {request.error}");
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

    void OnToggleChanged(Toggle selectedToggle)
    {
        if (selectedToggle.isOn)
        {
            Transform imageObject = selectedToggle.transform.GetChild(1);

            Image toggleImage = imageObject.GetComponent<Image>();

            if (toggleImage != null)
            {
                // Set the main image screen to the toggle's image
                if (imageFocusScreen != null)
                {
                    imageFocusScreen.sprite = toggleImage.sprite;
                    imageFocusScreen.color = Color.white; // Make sure it's visible
                }
            }
        }
    }
}
