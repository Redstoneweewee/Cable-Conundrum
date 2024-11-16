using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePlug {
    public Vector3 plugPosition;
    public bool isPluggedIn;
    public List<AttributesAndPosition> cables;


    public SavePlug(Vector3 plugPosition, bool isPluggedIn, List<AttributesAndPosition> cables) {
        this.plugPosition = plugPosition;
        this.isPluggedIn = isPluggedIn;
        this.cables = cables;
    }
}

public struct AttributesAndPosition {
    public bool             isInitialCable;
    public bool             isRotationCable;
    public bool             isIntersectionCable;
    public CableTypes       cableType;
    public Sprite           cableSprite;
    public float            zRotation;
    public Vector2          pivot;
    public ShadowDirections shadowDirection;
    public Directions       startingDirection;
    public Directions       endingDirection;
    public Vector2          directionMultiple;
    public Vector3 position;

    public AttributesAndPosition(CableChildAttributes attributes, Vector3 position) {
        this.isInitialCable      = attributes.isInitialCable;
        this.isRotationCable     = attributes.isRotationCable;
        this.isIntersectionCable = attributes.isIntersectionCable;
        this.cableType           = attributes.cableType;
        this.cableSprite         = attributes.cableSprite;
        this.zRotation           = attributes.zRotation;
        this.pivot               = attributes.pivot;
        this.shadowDirection     = attributes.shadowDirection;
        this.startingDirection   = attributes.startingDirection;
        this.endingDirection     = attributes.endingDirection;
        this.directionMultiple   = attributes.directionMultiple;
        this.position = position;
    }
}