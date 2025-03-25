using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpherialImageGrid : MonoBehaviour
{
    public int rows = 5; // Adjust for better square shape
    public int cols = 5;
    public float radius = 0.2f;
    public GameObject quadPrefab; // Assign a simple plane or quad

    public string imageUrl = "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png";

    void Start()
    {
        CreateSphericalGrid();
    }

    void CreateSphericalGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // Convert grid index to latitude and longitude
                float phi = Mathf.Lerp(-Mathf.PI / 2, Mathf.PI / 2, (float)i / (rows - 1)); // Latitude
                float theta = Mathf.Lerp(0, 2 * Mathf.PI, (float)j / cols); // Longitude

                // Convert spherical coordinates to Cartesian with the given of center at transform.position
                Vector3 pos = transform.position + new Vector3(
                    radius * Mathf.Cos(phi) * Mathf.Cos(theta),
                    radius * Mathf.Sin(phi),
                    radius * Mathf.Cos(phi) * Mathf.Sin(theta)
                );

                // Instantiate quad and set local position
                GameObject quad = Instantiate(quadPrefab, pos, Quaternion.identity, transform); 
                quad.gameObject.SetActive(true);

                // Rotate to face outward
                quad.transform.LookAt(transform.position);
                // quad.transform.Rotate(0, 0, 0); // Adjust for correct facing

                StartCoroutine(ApplyTexture(quad, imageUrl));
            }
        }
    }

    IEnumerator ApplyTexture(GameObject quad, string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Renderer renderer = quad.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"))
                {
                    mainTexture = texture
                };
            }
            
        }
        else
        {
            Debug.LogError("Error downloading image: " + request.error);
        }
    }
}
