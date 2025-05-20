using UnityEngine;
using System.Collections;

public class LoadingIndicator : MonoBehaviour
{
    public static LoadingIndicator Instance;

    [SerializeField] private GameObject loadingPanel; // Assign the panel prefab in inspector

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional if you want persistence
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Show()
    {
        loadingPanel.SetActive(true);
    }

    public void Hide()
    {
        loadingPanel.SetActive(false);
    }

    // Wrap a coroutine with loading indicator
    public void RunWithIndicator(IEnumerator coroutine)
    {
        StartCoroutine(RunCoroutineWithLoading(coroutine));
    }

    private IEnumerator RunCoroutineWithLoading(IEnumerator coroutine)
    {
        Show();
        yield return StartCoroutine(coroutine);
        Hide();
    }
}
