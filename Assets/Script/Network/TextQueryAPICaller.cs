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
    public string apiUrl = "http://server.selab.edu.vn:20721/search/search_with_text_query";
    public string imageUrlServer = "http://server.selab.edu.vn:20716/"; // Base URL for images
    public GameObject imagePrefab; // Prefab containing an Image component
    public Transform parentContainer; // Parent object to hold images
    public FocusImage imageFocus;
    public int getTopK;
    [SerializeField] private TMP_Text shot2;

    public void OnSubmit()
    {
        string userInput = inputField.text;
        if (userInput.Length == 0)
        {
            Debug.LogWarning("Input field is empty. Please enter a query.");
            return;
        }

        if (parentContainer.transform.childCount > 0)
        {
            foreach (Transform child in parentContainer.transform)
            {
                Destroy(child.gameObject); // Clear previous images
            }
        }
        StartCoroutine(SendTextQuery(userInput));
    }

    IEnumerator SendTextQuery(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("Text query is empty. Please enter a valid query.");
            yield break;
        }

        if (shot2 != null && !string.IsNullOrEmpty(shot2.text))
        {
            Debug.Log("shot2 is not null, setting user_id to: " + shot2.text);
            text = text + " | " + shot2.text;
        }

        Dictionary<string, string> requestBody = new Dictionary<string, string>
        {
            { "model", "clips" },
            { "text_query", text },
            { "dataset", "lsc24" },
            { "display_window_size", "0"},
            { "temporal_window_size", "3" },
            { "use_temporal_window", "false"}
           
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
            QueryApiResponse response = JsonConvert.DeserializeObject<QueryApiResponse>(request.downloadHandler.text);

            if (response.status == 200)
            {

                foreach (var item in response.data.Take(getTopK))
                {
                    GameObject newImageObject = Instantiate(imagePrefab, parentContainer);
                    newImageObject.name = $"Image_{item.id}";

                    Toggle toggleComponent = newImageObject.GetComponent<Toggle>();
                    toggleComponent.isOn = false;
                    toggleComponent.group = parentContainer.GetComponent<ToggleGroup>();
                    // toggleComponent.onValueChanged.AddListener((isOn) => OnToggleChanged(toggleComponent));

                    ToggleImage toggleImageComponent = newImageObject.GetComponent<ToggleImage>();
                    toggleImageComponent.Setup(item); // Pass the item to the ToggleImage component
                    toggleImageComponent.focusImage = imageFocus; // Pass the reference to FocusImage

                    Transform imageObject = newImageObject.transform.GetChild(1);

                    Image imageComponent = imageObject.GetComponent<Image>();

                    if (imageComponent != null)
                    {

                        string imageUrl = item.img_link; // Use the image link from the API response
                        imageUrl = imageUrl.Replace("http://127.0.0.1:8000", this.imageUrlServer); // Replace with the base URL for images

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
                Image imageFocusScreen = imageFocus.GetComponent<Image>(); // Assuming FocusImage has an Image component
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
