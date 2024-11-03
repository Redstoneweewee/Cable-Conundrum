using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CableAttributes : MonoBehaviour {

    [SerializeField] private bool             isInitialCable;
    [SerializeField] private bool             isRotationCable;
    [SerializeField] private bool             isIntersectionCable;
    [SerializeField] private CableTypes       cableType;
    [SerializeField] private Image            cableImage;
    [SerializeField] private float            zRotation;
    [SerializeField] private Vector2          pivot;
    [SerializeField] private ShadowDirections shadowDirection;
    [SerializeField] private Directions       startingDirection;
    [SerializeField] private Directions       endingDirection;
    [SerializeField] private Vector2          directionMultiple;

    public bool             IsInitialCable      { get{return isInitialCable;     } set{isInitialCable      = value;} }
    public bool             IsRotationCable     { get{return isRotationCable;    } set{isRotationCable     = value;} }
    public bool             IsIntersectionCable { get{return isIntersectionCable;} set{isIntersectionCable = value;} }
    public CableTypes       CableType           { get{return cableType;          } set{cableType           = value;} }
    public Image            CableImage          { get{return cableImage;         } set{cableImage          = value;} }
    public float            ZRotation           { get{return zRotation;          } set{zRotation           = value;} }
    public Vector2          Pivot               { get{return pivot;              } set{pivot               = value;} }
    public ShadowDirections ShadowDirection     { get{return shadowDirection;    } set{shadowDirection     = value;} }
    public Directions       StartingDirection   { get{return startingDirection;  } set{startingDirection   = value;} }
    public Directions       EndingDirection     { get{return endingDirection;    } set{endingDirection     = value;} }
    public Vector2          DirectionMultiple   { get{return directionMultiple;  } set{directionMultiple   = value;} }

}
