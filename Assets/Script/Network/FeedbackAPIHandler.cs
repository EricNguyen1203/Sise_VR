using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.UI;
using EnhancedScrollerDemos.GridSimulation; // Ensure you have the UnityEngine.UI namespace for UI components
public class FeedbackAPIHandler : MonoBehaviour
{
    public Transform likeImageHolder; // Parent object to hold like images
    public Transform dislikeImageHolder; // Parent object to hold dislike images
    public string apiUrl = "http://server.selab.edu.vn:20721/feedback"; // API URL for feedback
    public string imageUrlServer = "http://server.selab.edu.vn:20716/"; // Base URL for images
    public GameObject imagePrefab; // Prefab containing an Image component
    public Transform parentContainer; // Parent object to hold images
    public FocusImage imageFocus;
    public int getTopK;

    public void OnSubmit()
    {
        int likeCount = likeImageHolder.transform.childCount;
        int dislikeCount = dislikeImageHolder.transform.childCount;
        List<int> likeImageIds = new List<int>();
        List<int> dislikeImageIds = new List<int>();

        for (int i = 0; i < likeCount; i++)
        {
            Transform child = likeImageHolder.transform.GetChild(i);
            if (child != null && child.GetComponent<ToggleImage>() != null)
            {
                ToggleImage toggleImage = child.GetComponent<ToggleImage>();
                if (toggleImage != null && toggleImage.dataItem != null)
                {
                    likeImageIds.Add(toggleImage.dataItem.record_id);
                }
            }
        }

        for (int i = 0; i < dislikeCount; i++)
        {
            Transform child = dislikeImageHolder.transform.GetChild(i);
            if (child != null && child.GetComponent<ToggleImage>() != null)
            {
                ToggleImage toggleImage = child.GetComponent<ToggleImage>();
                if (toggleImage != null && toggleImage.dataItem != null)
                {
                    dislikeImageIds.Add(toggleImage.dataItem.record_id);
                }
            }
        }

        LikeGroup likeGroup = new LikeGroup
        {
            ids = likeImageIds,
            limit = 20,
            prior_scores = new List<float> { 0.9f, 0.8f, 0.7f },
        };
        FeedbackGroup feedbackGroup = new FeedbackGroup
        {
            ids = dislikeImageIds,
            limit = 20,
        };

        if (likeCount == 0 && dislikeCount == 0)
        {
            Debug.LogWarning("No images selected for feedback. Please select at least one image.");
            return;
        }

        // if (parentContainer.transform.childCount > 0)
        // {
        //     foreach (Transform child in parentContainer.transform)
        //     {
        //         Destroy(child.gameObject); // Clear previous images
        //     }
        // }
        StartCoroutine(SendFeedbackQuery(feedbackGroup, likeGroup));
    }

    IEnumerator SendFeedbackQuery(FeedbackGroup feedbackGroup, LikeGroup likeGroup)
    {
        if (feedbackGroup == null || likeGroup == null)
        {
            Debug.LogWarning("FeedbackGroup or LikeGroup is null. Cannot send feedback.");
            yield break;
        }

        FeedbackRequestBody requestBody = new FeedbackRequestBody
        {
            dislike = feedbackGroup,
            like = likeGroup,
            dataset = "lsc24",
            model = "clips"
        };

        string jsonData = JsonConvert.SerializeObject(requestBody);
        Debug.Log("Sending feedback data: " + jsonData);

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
            FeedbackAPIResponse response = JsonConvert.DeserializeObject<FeedbackAPIResponse>(request.downloadHandler.text);

            Debug.Log("Response status: " + response.response);
            Debug.Log("Response like: " + response.response.like.Count);
            Debug.Log("Response dislike: " + response.response.dislike.Count);

            DeleteDislikeImage(response.response.dislike);
            DeleteLikeImageInDisLikeImage(response.response.like, response.response.dislike);
            HandleLikeImage(response.response.like);
            // if (response.status == 200)
            // {

            //     foreach (var item in response.data.Take(getTopK))
            //     {
            //         GameObject newImageObject = Instantiate(imagePrefab, parentContainer);
            //         newImageObject.name = $"Image_{item.record_id}";

            //         Toggle toggleComponent = newImageObject.GetComponent<Toggle>();
            //         toggleComponent.isOn = false;
            //         toggleComponent.group = parentContainer.GetComponent<ToggleGroup>();
            //         // toggleComponent.onValueChanged.AddListener((isOn) => OnToggleChanged(toggleComponent));

            //         ToggleImage toggleImageComponent = newImageObject.GetComponent<ToggleImage>();
            //         toggleImageComponent.Setup(item); // Pass the item to the ToggleImage component
            //         toggleImageComponent.focusImage = imageFocus; // Pass the reference to FocusImage

            //         Transform imageObject = newImageObject.transform.GetChild(1);

            //         Image imageComponent = imageObject.GetComponent<Image>();

            //         if (imageComponent != null)
            //         {

            //             string imageUrl = item.img_link; // Use the image link from the API response
            //             imageUrl = imageUrl.Replace("http://server.selab.edu.vn:20716", this.imageUrlServer); // Replace with the base URL for images

            //             StartCoroutine(LoadImageFromURL(imageUrl, imageComponent));
            //         }

            //         yield return null; // Wait one frame to avoid freezing
            //     }
            // }
            // else
            // {
            //     Debug.LogWarning("API responded with status: " + response.status);
            // }
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

    void DeleteDislikeImage(List<DataItem> dataItems)
    {
        foreach (var item in dataItems)
        {
            if (item != null && parentContainer != null)
            {
                Transform child = parentContainer.Find(item.record_id.ToString());
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    void DeleteLikeImageInDisLikeImage(List<DataItem> likeItems, List<DataItem> dislikeItems)
    {
        foreach (var item in dislikeItems)
        {
            if (item != null && likeItems != null && likeItems.Count > 0)
            {
                foreach (var likeItem in likeItems)
                {
                    if (item.record_id == likeItem.record_id)
                    {
                        likeItems.Remove(likeItem);
                        break; // Exit the inner loop once a match is found
                    }
                }
            }
        }
    }

    void HandleLikeImage(List<DataItem> dataItems)
    {
        // Loop from the end to the beginning of the list to avoid index issues when removing items
        for (int i = dataItems.Count - 1; i >= 0; i--)
        {
            var item = dataItems[i];
            Transform itemchecker = parentContainer.Find(item.record_id.ToString());
            if (itemchecker != null)
            {
                itemchecker.SetAsFirstSibling(); // Move the new image to the top of the list
                
            }
            else
            {
                GameObject newImageObject = Instantiate(imagePrefab, parentContainer);
                newImageObject.name = $"{item.record_id}";
                newImageObject.transform.SetAsFirstSibling(); // Move the new image to the top of the list

                Toggle toggleComponent = newImageObject.GetComponent<Toggle>();
                toggleComponent.isOn = false;
                toggleComponent.group = parentContainer.GetComponent<ToggleGroup>();
                toggleComponent.onValueChanged.AddListener((isOn) => OnToggleChanged(toggleComponent));

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
