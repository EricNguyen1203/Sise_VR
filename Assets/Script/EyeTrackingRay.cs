using System;
using System.Collections.Generic;
using System.Numerics;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using Unity.Collections;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRay : MonoBehaviour
{
    [SerializeField]
    private float _rayDistance = 1.0f;

    [SerializeField]
    private float _rayWidth = 0.01f;

    [SerializeField]
    private LayerMask _layersToInclude;

    [SerializeField]
    private Color _rayColorDefaultState = Color.yellow;

    [SerializeField]
    private Color _rayColorHoverState = Color.red;

    [SerializeField]
    private OVRHand _handUsedForPinchSelection;

    [SerializeField]
    private ActiveStateSelector _handPoseSelector; // NEW: Hand pose selector


    [SerializeField]
    private bool _mockHandUsedForPinchSelection;

    private bool _intercepting;

    private bool _allowPinchSelection;

    private LineRenderer _lineRenderer;

    private Dictionary<int, EyeInteractable> _interactableObjects = new Dictionary<int, EyeInteractable>();
    
    private EyeInteractable _lastEyeInteractable;
    // Start is called before the first frame update

    private bool IsPinching;
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _allowPinchSelection = _handUsedForPinchSelection != null ;
        _handPoseSelector.WhenSelected += () => IsPinching = true;
        _handPoseSelector.WhenUnselected += () => IsPinching = false;
        SetupRay();
    }

    void SetupRay()
    {
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = _rayWidth;
        _lineRenderer.endWidth = _rayWidth;
        _lineRenderer.startColor = _rayColorDefaultState;
        _lineRenderer.endColor = _rayColorDefaultState;
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y, 
            transform.position.z + _rayDistance));
    }
    
    private void Update()
    {
        _lineRenderer.enabled = !IsPinching;

        SelectionStarted();

        if(!_intercepting) {
            _lineRenderer.startColor = _lineRenderer.endColor = _rayColorDefaultState;
            _lineRenderer.SetPosition(1, new Vector3(0, 0, transform.position.z + _rayDistance));
            OnHoverEnded();
        }
    }


    void FixedUpdate()
    {
        if(IsPinching) return;

        Vector3 rayDirection = transform.TransformDirection(Vector3.forward) * _rayDistance;

        _intercepting = Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, Mathf.Infinity ,_layersToInclude);

        if (_intercepting){
            OnHoverEnded();

            _lineRenderer.startColor = _lineRenderer.endColor = _rayColorHoverState;

            // keep cache of eye interactable objects
            if (!_interactableObjects.TryGetValue(hit.transform.GetHashCode(), out EyeInteractable eyeInteractable)){
                eyeInteractable = hit.transform.GetComponent<EyeInteractable>();
                _interactableObjects.Add(hit.transform.GetHashCode(), eyeInteractable);
            }

            var toLocalSpace = transform.InverseTransformPoint(eyeInteractable.transform.position);
            _lineRenderer.SetPosition(1, new Vector3(0, 0, toLocalSpace.z));

            eyeInteractable.Hover(true);

            _lastEyeInteractable = eyeInteractable;
        }
    }

    private void SelectionStarted(){
        if (IsPinching){
            _lastEyeInteractable?.Select(true, (_handUsedForPinchSelection?.IsTracked ?? false) ? _handUsedForPinchSelection.transform : transform);
        }
        else {
            _lastEyeInteractable?.Select(false);

        }
    }

    private void OnHoverEnded(){
        foreach (var interactable in _interactableObjects) interactable.Value.Hover(false);
    }

    private void OnDestroy() => _interactableObjects.Clear();
    // private bool IsPinching() => (_allowPinchSelection && _handUsedForPinchSelection.GetFingerIsPinching(OVRHand.HandFinger.Index) || _mockHandUsedForPinchSelection);
    // private bool IsPinching();

}
