using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimilarHandler : MonoBehaviour
{
    private bool _enabledSimilarity;
    [SerializeField] private GameObject _similaritySlider;
    void Start()
    {
        _enabledSimilarity = false;
    }

    public void OnButtonClick()
    {
        _enabledSimilarity = !_enabledSimilarity;
        if (_enabledSimilarity)
        {
            _similaritySlider.SetActive(true);
        }
        else
        {
            _similaritySlider.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
