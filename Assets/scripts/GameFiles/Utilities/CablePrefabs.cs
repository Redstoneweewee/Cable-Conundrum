using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CablePrefabs : MonoBehaviour {
    /* Cables:
    * [ [0 ]UpLeft,    [1 ]UpRight,    [2 ]DownLeft,    [3 ]DownRight,    [4 ]LeftUp,    [5 ]LeftDown,    [6 ]RightUp,    [7 ]RightDown,   ]
    * [ [8 ]InUpLeft,  [9 ]InUpRight,  [10]InDownLeft,  [11]InDownRight,  [12]InLeftUp,  [13]InLeftDown,  [14]InRightUp,  [15]InRightDown, ]
    * [ [16]OutUpLeft, [17]OutUpRight, [18]OutDownLeft, [19]OutDownRight, [20]OutLeftUp, [21]OutLeftDown, [22]OutRightUp, [23]OutRightDown ]
    * Link: https://docs.google.com/document/d/1-T7I-lNiF93s7gjlgOsbJBzwmPMfbWuQ_Jdx-Hd63DM/edit?usp=sharing
    */
    [SerializeField] public List<GameObject> cablePrefabs;
    /* Sprites:
    * [0]Out, [1]Out Turn, [2]In Turn
    */
    [SerializeField] public List<Sprite> cableSprites;
}
