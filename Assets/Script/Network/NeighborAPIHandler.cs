using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighborHandler : MonoBehaviour
{
    private bool _enabledSimilarity;
    [SerializeField] private GameObject _neighborSlider;
    void Start()
    {
        _enabledSimilarity = false;
    }

    public void OnButtonClick()
    {
        _enabledSimilarity = !_enabledSimilarity;
        if (_enabledSimilarity)
        {
            _neighborSlider.SetActive(true);
        }
        else
        {
            _neighborSlider.SetActive(false);
        }
    }
}
