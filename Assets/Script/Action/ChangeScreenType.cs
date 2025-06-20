using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScreenType : MonoBehaviour
{
    private TextQueryAPICaller _textQueryAPICaller;
    
    // Start is called before the first frame update
    void Start()
    {
        _textQueryAPICaller = FindObjectOfType<TextQueryAPICaller>();
        if (_textQueryAPICaller == null)
        {
            Debug.LogError("TextQueryAPICaller not found in the scene.");
            return;
        }
    }

    public void ChangeToTextScreen(Transform newContainer = null)
    {
        if (_textQueryAPICaller == null)
        {
            Debug.LogError("TextQueryAPICaller is not initialized.");
            return;
        }

        // Clear the current container if provided
        if (newContainer != null)
        {
            foreach (Transform child in newContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // Change the screen type to text
        _textQueryAPICaller.ChangeParentConTainer(newContainer);
    }
}
