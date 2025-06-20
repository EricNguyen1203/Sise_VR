using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro; // if you're using TextMeshPro
using Newtonsoft.Json; // Add Newtonsoft.Json via Unity Package Manager or NuGet
using System.Linq;
using AirFishLab.ScrollingList;
using System.Runtime.ExceptionServices;
using Oculus.Interaction.PoseDetection;
using Unity.VisualScripting;

public class TextQueryAPICaller : MonoBehaviour
{
    public TMP_InputField inputField; // Drag your input field (named "Text") here
    public string apiUrl = "http://server.selab.edu.vn:20721/search/search_with_text_query";
    public string imageUrlServer = "http://server.selab.edu.vn:20716/"; // Base URL for images
    public GameObject imagePrefab; // Prefab containing an Image component
    public Transform parentContainer; // Parent object to hold images
    public FocusImage imageFocus;
    public int getTopK;
    public List<CircularScrollingList> circularScrollingListsShot1;
    public List<TMP_InputField> inputFieldsShot1;
    private Dictionary<string, CircularScrollingList> circularScrollingListDictShot1 = new Dictionary<string, CircularScrollingList>();
    private Dictionary<string, TMP_InputField> inputFieldDictShot1 = new Dictionary<string, TMP_InputField>();

    private string model;
    private string dataset;
    private string mode;

    [SerializeField] private TMP_InputField shot2;

    public List<CircularScrollingList> circularScrollingListsShot2;
    public List<TMP_InputField> inputFieldsShot2;
    private Dictionary<string, CircularScrollingList> circularScrollingListDictShot2 = new Dictionary<string, CircularScrollingList>();
    private Dictionary<string, TMP_InputField> inputFieldDictShot2 = new Dictionary<string, TMP_InputField>();

    void Start()
    {
        for (int i = 0; i < circularScrollingListsShot1.Count; i++)
        {
            string key = circularScrollingListsShot1[i].name;
            if (!circularScrollingListDictShot1.ContainsKey(key))
            {
                circularScrollingListDictShot1.Add(key, circularScrollingListsShot1[i]);
            }
        }

        for (int i = 0; i < inputFieldsShot1.Count; i++)
        {
            string key = inputFieldsShot1[i].name;
            if (!inputFieldDictShot1.ContainsKey(key))
            {
                inputFieldDictShot1.Add(key, inputFieldsShot1[i]);
            }
        }

        for (int i = 0; i < circularScrollingListsShot2.Count; i++)
        {
            string key = circularScrollingListsShot2[i].name;
            if (!circularScrollingListDictShot2.ContainsKey(key))
            {
                circularScrollingListDictShot2.Add(key, circularScrollingListsShot2[i]);
            }
        }

        for (int i = 0; i < inputFieldsShot2.Count; i++)
        {
            string key = inputFieldsShot2[i].name;
            if (!inputFieldDictShot2.ContainsKey(key))
            {
                inputFieldDictShot2.Add(key, inputFieldsShot2[i]);
            }
        }
    }
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

        // Begin indicating that the API call is starting
        LoadingIndicator.Instance.RunWithIndicator(SendTextQuery(userInput));
        // StartCoroutine(SendTextQuery(userInput));
    }

    IEnumerator SendTextQuery(string text)
    {
        string query = AddFilterText(text, circularScrollingListDictShot1, inputFieldDictShot1);
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("Text query is empty. Please enter a valid query.");
            yield break;
        }

        if (shot2 != null && shot2.text.Length > 0)
        {
            string shot2Text = AddFilterText(shot2.text, circularScrollingListDictShot2, inputFieldDictShot2);
            Debug.Log("shot2 is not null, setting user_id to: " + shot2Text);
            query = query + " | " + shot2Text;
        }

        Debug.Log("Text query: " + query);

        GetAPIParams();

        Dictionary<string, string> requestBody = new Dictionary<string, string>
        {
            { "model", model },
            { "text_query", query },
            { "dataset", dataset },
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
                    newImageObject.name = $"{item.record_id}";

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
                        imageUrl = imageUrl.Replace("http://server.selab.edu.vn:20716", this.imageUrlServer); // Replace with the base URL for images

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



    void GetAPIParams()
    {
        for (int i = 0; i < circularScrollingListDictShot1.Count; i++)
        {
            // Get the 4th child of the current CircularScrollingList
            string name = circularScrollingListDictShot1.ElementAt(i).Key;
            if (name == "Dataset")
            {
                Transform child = circularScrollingListDictShot1.ElementAt(i).Value.transform.GetChild(4).GetChild(0);
                Text childText = child.GetComponent<Text>();
                if (childText != null)
                {
                    dataset = childText.text;
                    Debug.Log($"Dataset: {dataset}");
                }
            }
            else if (name == "Model")
            {
                Transform child = circularScrollingListDictShot1.ElementAt(i).Value.transform.GetChild(4).GetChild(0);
                Text childText = child.GetComponent<Text>();
                if (childText != null)
                {
                    model = childText.text;
                    Debug.Log($"Model: {model}");
                }
            }
            else if (name == "Mode")
            {
                Transform child = circularScrollingListDictShot1.ElementAt(i).Value.transform.GetChild(4).GetChild(0);
                Text childText = child.GetComponent<Text>();
                if (childText != null)
                {
                    mode = childText.text;
                    Debug.Log($"Mode: {mode}");
                }
            }
        }
    }

    string AddFilterText(string text, Dictionary<string, CircularScrollingList> circularScrollingListDict, Dictionary<string, TMP_InputField> inputFieldDict)
    {
        // Add filter text to the input field
        for (int i = 0; i < circularScrollingListDict.Count; i++)
        {
            string key = circularScrollingListDict.ElementAt(i).Key;
            if (key == "a")
            {
                Transform child = circularScrollingListDict.ElementAt(i).Value.transform.GetChild(4).GetChild(0);
                Text childText = child.GetComponent<Text>();
                if (childText != null)
                {
                    string currentText = childText.text;
                    text = text + " -a " + currentText;
                }
            }
        }

        for (int i = 0; i < inputFieldDict.Count; i++)
        {
            string key = inputFieldDict.ElementAt(i).Key;
            if (key == "d")
            {
                string currentText = inputFieldDict.ElementAt(i).Value.text;
                // text = text + " -d " + currentText;

                if (currentText.Length > 0)
                {
                    text = text + " -d " + currentText;
                }
            }
            else if (key == "ocr")
            {
                string currentText = inputFieldDict.ElementAt(i).Value.text;
                // text = text + " -t " + currentText;

                if (currentText.Length > 0)
                {
                    text = text + " -ocr " + currentText;
                }
            }
            else if (key == "l")
            {
                string currentText = inputFieldDict.ElementAt(i).Value.text;
                // text = text + " -l " + currentText;

                if (currentText.Length > 0)
                {
                    text = text + " -l " + currentText;
                }
            }
        }
        return text;
    }

    public void ChangeParentConTainer(Transform newParent)
    {
        if (parentContainer != null)
        {
            // Move all children to the new parent
            for (int i = parentContainer.childCount - 1; i >= 0; i--)
            {
                Transform child = parentContainer.GetChild(i);
                child.SetParent(newParent, false); // Keep the local position and rotation
                child.SetAsFirstSibling(); // Optional: Set as first sibling if needed
            }
            parentContainer = newParent;
            Debug.Log("Parent container changed to: " + newParent.name);
        }
        else
        {
            Debug.LogWarning("Parent container is null. Cannot change parent.");
        }

    }
}
