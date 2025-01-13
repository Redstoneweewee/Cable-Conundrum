using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntersectionData : Singleton<IntersectionData> {
    [HideInInspector] public bool hasIntersection = false;


    public override IEnumerator Initialize() {
        yield return null;
    }
}
