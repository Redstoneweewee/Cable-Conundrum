using System;
using System.Collections;
using System.Collections.Generic;
using SerializableData;
using UnityEngine;

public class ScriptInitializationPriority {
    /** 
    How to use this:
        public static List<InitializationPriority> list = new List<InitializationPriority> {
                                    ----------------------------------------
        new InitializationPriority( >>> priority number, lower = earlier <<<, 
                                    ----------------------------------------
        new List<ScriptTypeAndPlace> {
            // >>> scripts and place ---------------------- <<<
            new ScriptTypeAndPlace(typeof( <script_name> ),  InitPlace.<place_of_init> ), 
            new ScriptTypeAndPlace(typeof( <script_name> ),  InitPlace.<place_of_init> ), 
            new ScriptTypeAndPlace(typeof( <script_name> ),  InitPlace.<place_of_init> ), 
            ...
            // >>> scripts and place ---------------------- <<<
        }),
        ...
    */

    public static List<InitializationPriority> list = new List<InitializationPriority> {
        new InitializationPriority( 0, 
        new List<ScriptTypeAndPlace> {
            //scripts and place ----------------------
            new ScriptTypeAndPlace(typeof(AdminToggles),                InitPlace.Start), 
            new ScriptTypeAndPlace(typeof(ButtonsAttributes),           InitPlace.All), 
            new ScriptTypeAndPlace(typeof(ButtonsHandler),              InitPlace.All), 
            new ScriptTypeAndPlace(typeof(ButtonsOutlineLocal),         InitPlace.All), 
            new ScriptTypeAndPlace(typeof(ControlsData),                InitPlace.All), 
            new ScriptTypeAndPlace(typeof(SliderAttributes),            InitPlace.Start), 
            new ScriptTypeAndPlace(typeof(SoundsController),            InitPlace.Start), 
            new ScriptTypeAndPlace(typeof(TutorialController),          InitPlace.Start), 
            new ScriptTypeAndPlace(typeof(DataPersistenceManager),      InitPlace.Start),
            new ScriptTypeAndPlace(typeof(CableParentAttributes),       InitPlace.Level),
            new ScriptTypeAndPlace(typeof(SocketAttributes),            InitPlace.Level), 
            new ScriptTypeAndPlace(typeof(SocketHandler),               InitPlace.Level), 
            new ScriptTypeAndPlace(typeof(JointsOpacityGlobal),         InitPlace.Level), 
            new ScriptTypeAndPlace(typeof(LevelSelectorBackgroundData), InitPlace.LevelSelector), 
            new ScriptTypeAndPlace(typeof(DontDestroyOnLoad),           InitPlace.Start),
            new ScriptTypeAndPlace(typeof(WinningMessageSizeGlobal),    InitPlace.Level)
            //scripts and place ----------------------
        }),

        new InitializationPriority( 1, 
        new List<ScriptTypeAndPlace> {
            //scripts and place ----------------------
            new ScriptTypeAndPlace(typeof(PlugSelectorData),                  InitPlace.Level), 
            new ScriptTypeAndPlace(typeof(TutorialVideoAttributes),           InitPlace.Start), 
            new ScriptTypeAndPlace(typeof(CreditsGlobal),                     InitPlace.Menu), 
            new ScriptTypeAndPlace(typeof(LevelSelectorInitializerGlobal),    InitPlace.LevelSelector), 
            new ScriptTypeAndPlace(typeof(MenuInitializerGlobal),             InitPlace.Menu), 
            new ScriptTypeAndPlace(typeof(LevelSelectorBackgroundController), InitPlace.LevelSelector), 
            new ScriptTypeAndPlace(typeof(LevelResizeGlobal),                 InitPlace.Level), 
            new ScriptTypeAndPlace(typeof(ResizeGlobal),                      InitPlace.All)
            //scripts and place ----------------------
        }),

        new InitializationPriority( 2, 
        new List<ScriptTypeAndPlace> {
            //scripts and place ----------------------
            new ScriptTypeAndPlace(typeof(PlugSelectorController),    InitPlace.Level), 
            new ScriptTypeAndPlace(typeof(ElectricalStripController), InitPlace.Level), 
            new ScriptTypeAndPlace(typeof(ElectricalStripData),       InitPlace.Level), 
            new ScriptTypeAndPlace(typeof(GridsSkeleton),             InitPlace.Level), 
            new ScriptTypeAndPlace(typeof(JointsController),          InitPlace.Level), 
            new ScriptTypeAndPlace(typeof(JointsData),                InitPlace.Level), 
            new ScriptTypeAndPlace(typeof(PlugAttributes),            InitPlace.Level), 
            new ScriptTypeAndPlace(typeof(PlugHandler),               InitPlace.Level),
            new ScriptTypeAndPlace(typeof(WinningControllerGlobal),   InitPlace.Level)
            //scripts and place ----------------------
        }),

        new InitializationPriority( 3, 
        new List<ScriptTypeAndPlace> {
            //scripts and place ----------------------
            new ScriptTypeAndPlace(typeof(CableHandler),          InitPlace.Level)
            //scripts and place ----------------------
        }),

        new InitializationPriority( 4, 
        new List<ScriptTypeAndPlace> {
            //scripts and place ----------------------
            new ScriptTypeAndPlace(typeof(ObstacleAttributes), InitPlace.Level), 
            new ScriptTypeAndPlace(typeof(ObstacleHandler),    InitPlace.Level)
            //scripts and place ----------------------
        }),

        new InitializationPriority( 5, 
        new List<ScriptTypeAndPlace> {
            //scripts and place ----------------------
            new ScriptTypeAndPlace(typeof(GridsController),        InitPlace.Level),
            new ScriptTypeAndPlace(typeof(IntersectionController), InitPlace.Level)
            //scripts and place ----------------------
        }),

        new InitializationPriority( 6, 
        new List<ScriptTypeAndPlace> {
            //scripts and place ----------------------
            new ScriptTypeAndPlace(typeof(ControlsController), InitPlace.All)
            //scripts and place ----------------------
        }),

        new InitializationPriority( 7, 
        new List<ScriptTypeAndPlace> {
            //scripts and place ----------------------
            new ScriptTypeAndPlace(typeof(LevelInitializerGlobal), InitPlace.Level)
            //scripts and place ----------------------
        }),

        new InitializationPriority( 8, 
        new List<ScriptTypeAndPlace> {
            //scripts and place ----------------------
            new ScriptTypeAndPlace(typeof(LevelTitleGlobal),      InitPlace.Level), 
            new ScriptTypeAndPlace(typeof(PowerSwitchController), InitPlace.Level)
            //new ScriptTypeAndPlace(typeof(ScenesController),      InitPlace.All)
            //scripts and place ----------------------
        })
    };

}



public class InitializationPriority {
    private int priority;
    private List<ScriptTypeAndPlace> scriptTypeAndPlace;
    public InitializationPriority(int priority, List<ScriptTypeAndPlace> scriptTypeAndPlace) {
        this.priority = priority;
        this.scriptTypeAndPlace = scriptTypeAndPlace;
    }

    public int GetPriority() {
        return priority;
    }
    public List<ScriptTypeAndPlace> ScriptTypeAndPlace() {
        return scriptTypeAndPlace;
    }
}

public class ScriptTypeAndPlace {
    private Type scriptType;
    private InitPlace scriptPlace;
    public ScriptTypeAndPlace(Type scriptType, InitPlace scriptPlace) {
        this.scriptType = scriptType;
        this.scriptPlace = scriptPlace;
    }

    public Type GetScriptType() {
        return scriptType;
    }
    public InitPlace GetScriptPlace() {
        return scriptPlace;
    }
}

public enum InitPlace {
    All,             //Will initialize every time the scene is changed
    Start,           //Will only initialize once at the start game launch
    Menu,            //Will initialize every time the player enters the menu
    LevelSelector,   //Will initialize every time the player enters the level selector
    Level            //Will initialize every time the player enters any level
}