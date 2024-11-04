using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CableChildAttributes : MonoBehaviour {

    //Are edited when cables are regenerated 
    [SerializeField] public bool             isInitialCable;
    [SerializeField] public bool             isRotationCable;
    [SerializeField] public bool             isIntersectionCable;
    [SerializeField] public CableTypes       cableType;
    [SerializeField] public Image            cableImage;
    [SerializeField] public float            zRotation;
    [SerializeField] public Vector2          pivot;
    [SerializeField] public ShadowDirections shadowDirection;
    [SerializeField] public Directions       startingDirection;
    [SerializeField] public Directions       endingDirection;
    [SerializeField] public Vector2          directionMultiple;

}
