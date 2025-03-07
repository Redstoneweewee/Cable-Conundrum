using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Game enums

public enum LoadSceneTypes    { Start, Menu, LevelSelector, Level, NextLevel, PreviousLevel }
public enum ButtonTypes       { EnterLevel, NextLevel, PreviousLevel, 
                                EnterLevelSelector, 
                                EnterMenu, 
                                EnterSettings, ExitSettings, 
                                EnterExitConfirmation, ExitExitConfirmation, ExitGame, 
                                EnterTutorialPage, ExitTutorialPage, NextTutorialPage, PreviousTutorialPage,
                                EnterCredits, ExitCredits }

public enum SliderTypes       { Sound, Music }
public enum SoundTypes        { HoverOverButton, ClickOnButton, ClickOnPlug, PlugSnapEnter, PlugSnapExit, ChangeCable, Victory }

public enum LevelFailureTypes { None, Plugs, Cables, Obstacles }

//Editor enums
public enum PlugSelectorTypes { Plug, PermaPlug, Table }

//Level enums
public enum CableTypes       { UpLeft,    UpRight,    DownLeft,    DownRight,    LeftUp,    LeftDown,    RightUp,    RightDown,  
                               InUpLeft,  InUpRight,  InDownLeft,  InDownRight,  InLeftUp,  InLeftDown,  InRightUp,  InRightDown,
                               OutUpLeft, OutUpRight, OutDownLeft, OutDownRight, OutLeftUp, OutLeftDown, OutRightUp, OutRightDown  }
public enum ObstacleTypes    { Plug, LeftTableLeg, RightTableLeg, TableTop, Screw }
public enum Directions       { Up, Down, Left, Right }
public enum ShadowDirections { Up, Down, Left, Right, In, Out }


public enum ScreenAnchor     { Center, Up, Down, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight, UpInverted, DownInverted, LeftInverted, RightInverted }
public enum ScaleTypes       { Horizontal, Vertical, Both, Either, KeepWithScreen }
