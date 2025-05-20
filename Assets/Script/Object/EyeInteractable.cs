using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
/// <summary>
/// Base class for any object that can be interacted with using gaze.
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class EyeInteractable : MonoBehaviour
{
    [field: SerializeField]
    public bool IsHovered { get; private set; }

    [field: SerializeField]
    public bool IsSelected { get; protected set; }

    public virtual void Hover(bool state)
    {
        IsHovered = state;
    }

    public virtual void Select(bool state, GazeImageFeedback feedback = null)
    {
        IsSelected = state;
    }
}
