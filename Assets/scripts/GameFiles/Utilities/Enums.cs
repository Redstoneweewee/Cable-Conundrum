using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Game enums

public enum LoadSceneTypes    { Menu, LevelSelector, Level, NextLevel, PreviousLevel }
public enum ButtonTypes       { EnterLevel, NextLevel, PreviousLevel, EnterLevelSelector, EnterMenu, EnterSettings, ExitSettings, EnterExitConfirmation, ExitExitConfirmation, ExitGame }
public enum SliderTypes       { Sound, Music }
public enum LevelFailureTypes { None, Plugs, Cables, Obstacles }

//Editor enums
public enum PlugSelectorTypes { Plug, PermaPlug, Table }

//Level enums
public enum CableTypes       { UpLeft,    UpRight,    DownLeft,    DownRight,    LeftUp,    LeftDown,    RightUp,    RightDown,  
                               InUpLeft,  InUpRight,  InDownLeft,  InDownRight,  InLeftUp,  InLeftDown,  InRightUp,  InRightDown,
                               OutUpLeft, OutUpRight, OutDownLeft, OutDownRight, OutLeftUp, OutLeftDown, OutRightUp, OutRightDown  }
public enum ObstacleTypes    { Plug, LeftTableLeg, RightTableLeg, TableTop }
public enum Directions       { Up, Down, Left, Right }
public enum ShadowDirections { Up, Down, Left, Right, In, Out }

