using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : Singleton<Test> {
    
    public override IEnumerator Initialize() {
        yield return null;
    }
}
