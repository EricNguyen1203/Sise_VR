using TMPro;
using UnityEngine;
using UnityEngine.Events;

// [RequireComponent(typeof(Collider))]
// [RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{

    [field: SerializeField]
    public bool IsHovered { get; private set; }

    [field: SerializeField]
    public bool IsSelected { get; private set; }
    [SerializeField]
    private UnityEvent<GameObject> OnObjectHovered;

    [SerializeField]
    private UnityEvent<GameObject> OnObjectSelected;

    [SerializeField]
    private Material _onHoverActiveMaterial;

    [SerializeField]
    private Material _onSelectActiveMaterial;

    [SerializeField]
    private Material _onIdleMaterial;

    private MeshRenderer _meshRenderer;

    private Transform _originalAnchor;

    private TextMeshPro _statusText;

    private ToggleImage _toggleImage;

    // Start is called before the first frame update
    void Start()
    {
        // _meshRenderer = GetComponent<MeshRenderer>();
        // _originalAnchor = transform.parent;
        _toggleImage = GetComponent<ToggleImage>();
        // _statusText = GetComponentInChildren<TextMeshPro>();
    }

    public void Hover(bool state)
    {
        IsHovered = state;
    }

    public void Select(bool state, GazeImageFeedback gazeImageFeedback = null)
    {
        IsSelected = state;
        if (state && gazeImageFeedback != null && _toggleImage != null)
        {
            gazeImageFeedback.GetImage(_toggleImage);
        }

        // if (state) transform.SetParent(_originalAnchor);
        // else transform.SetParent(null);

        // if (state) _meshRenderer.material = _onSelectActiveMaterial;
        // else _meshRenderer.material = _onIdleMaterial;

        // OnObjectSelected?.Invoke(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // if (IsHovered)
        // {
        //     _meshRenderer.material = _onHoverActiveMaterial;
        //     OnObjectHovered?.Invoke(gameObject);
        //     _statusText.text = $"<color=\"yellow\">Hovered</color>";
        // }
        // if (IsSelected)
        // {
        //     _meshRenderer.material = _onSelectActiveMaterial;
        //     OnObjectSelected?.Invoke(gameObject);
        //     _statusText.text = $"<color=\"green\">Selected</color>";
        // }
        // if (!IsHovered && !IsSelected)
        // {
        //     _meshRenderer.material = _onIdleMaterial;
        //     _statusText.text = $"<color=\"white\">Idle</color>";
        // }
    }
}
