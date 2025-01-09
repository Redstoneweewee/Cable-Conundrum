using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LevelSelectorBackgroundController : Singleton<LevelSelectorBackgroundController> {
    private LevelSelectorBackgroundData D;


    public override IEnumerator Initialize() {
        yield return null;
    }
    public override void OnAwake() {
        D = LevelSelectorBackgroundData.Instance;
    }

    void Update() {
        if(D.center.x != transform.position.x || D.center.y != transform.position.y) {
            D.center = transform.position;
        }
        D.deltaMousePosition = ControlsController.Instance.GetPointerPosition() - D.center;
        D.deltaMousePosition = math.clamp(D.deltaMousePosition, new Vector2(-Screen.width/2, -Screen.height/2), new Vector2(Screen.width/2, Screen.height/2));
        D.targetBackgroundImagePosition = D.center + (D.deltaMousePosition * D.backgroundMovementMultiplier);
        D.targetButtonsImagePosition = D.center + (D.deltaMousePosition * D.buttonsMovementMultiplier);
        MoveBackgroundImageByMouse();
        MoveButtonsImageByMouse();
    }  

    private void MoveBackgroundImageByMouse() {
        if(Utilities.IsApproximate(D.backgroundImageOnly.transform.position, D.targetBackgroundImagePosition, 0.001f)) { return; }
        D.backgroundImageOnly.transform.position = Vector2.Lerp(D.backgroundImageOnly.transform.position, D.targetBackgroundImagePosition, D.mouseMoveInterpolation);
    }

    private void MoveButtonsImageByMouse() {
        if(Utilities.IsApproximate(D.buttonsImageOnly.transform.position, D.targetButtonsImagePosition, 0.001f)) { return; }
        D.buttonsImageOnly.transform.position = Vector2.Lerp(D.buttonsImageOnly.transform.position, D.targetButtonsImagePosition, D.mouseMoveInterpolation);
    }
}
