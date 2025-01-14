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
        new List<ScriptInitAttributes> {
            // >>> scripts and place ---------------------- <<<
            new ScriptInitAttributes(typeof( <script_name> ),  new ExecuteOn[]{ ExecuteOn.<place_of_init1>, ExecuteOn.<place_of_init2> ... }), 
            new ScriptInitAttributes(typeof( <script_name> ),  new ExecuteOn[]{ ExecuteOn.<place_of_init1>, ExecuteOn.<place_of_init2> ... }), 
            new ScriptInitAttributes(typeof( <script_name> ),  new ExecuteOn[]{ ExecuteOn.<place_of_init1>, ExecuteOn.<place_of_init2> ... }), 
            ...
            // >>> scripts and place ---------------------- <<<
        }),
        ...
    */

    public static List<InitializationPriority> initList = new List<InitializationPriority> {
        new InitializationPriority( 0, 
        new List<ScriptInitAttributes> {
            //scripts and place ----------------------
            new ScriptInitAttributes(typeof(AdminToggles),                new ExecuteOn[]{ ExecuteOn.Start }), 
            new ScriptInitAttributes(typeof(ButtonsAttributes),           new ExecuteOn[]{ ExecuteOn.All }), 
            new ScriptInitAttributes(typeof(ButtonsHandler),              new ExecuteOn[]{ ExecuteOn.All }), 
            new ScriptInitAttributes(typeof(ButtonsOutlineLocal),         new ExecuteOn[]{ ExecuteOn.All }), 
            new ScriptInitAttributes(typeof(ControlsData),                new ExecuteOn[]{ ExecuteOn.All }), 
            new ScriptInitAttributes(typeof(SliderAttributes),            new ExecuteOn[]{ ExecuteOn.Start }), 
            new ScriptInitAttributes(typeof(SoundsController),            new ExecuteOn[]{ ExecuteOn.Start }), 
            new ScriptInitAttributes(typeof(TutorialController),          new ExecuteOn[]{ ExecuteOn.Start }), 
            new ScriptInitAttributes(typeof(DataPersistenceManager),      new ExecuteOn[]{ ExecuteOn.Start }),
            new ScriptInitAttributes(typeof(CableParentAttributes),       new ExecuteOn[]{ ExecuteOn.Level }),
            new ScriptInitAttributes(typeof(SocketAttributes),            new ExecuteOn[]{ ExecuteOn.Level }), 
            new ScriptInitAttributes(typeof(SocketHandler),               new ExecuteOn[]{ ExecuteOn.Level }), 
            new ScriptInitAttributes(typeof(JointsOpacityGlobal),         new ExecuteOn[]{ ExecuteOn.Level }), 
            new ScriptInitAttributes(typeof(LevelSelectorBackgroundData), new ExecuteOn[]{ ExecuteOn.LevelSelector }), 
            new ScriptInitAttributes(typeof(DontDestroyOnLoad),           new ExecuteOn[]{ ExecuteOn.Start }),
            new ScriptInitAttributes(typeof(WinningMessageSizeGlobal),    new ExecuteOn[]{ ExecuteOn.Level })
            //scripts and place ----------------------
        }),

        new InitializationPriority( 1, 
        new List<ScriptInitAttributes> {
            //scripts and place ----------------------
            new ScriptInitAttributes(typeof(PlugSelectorData),                  new ExecuteOn[]{ ExecuteOn.Level }), 
            new ScriptInitAttributes(typeof(TutorialVideoAttributes),           new ExecuteOn[]{ ExecuteOn.Start }), 
            new ScriptInitAttributes(typeof(CreditsGlobal),                     new ExecuteOn[]{ ExecuteOn.Menu }), 
            new ScriptInitAttributes(typeof(LevelSelectorInitializerGlobal),    new ExecuteOn[]{ ExecuteOn.LevelSelector }), 
            new ScriptInitAttributes(typeof(MenuInitializerGlobal),             new ExecuteOn[]{ ExecuteOn.Menu }), 
            new ScriptInitAttributes(typeof(LevelSelectorBackgroundController), new ExecuteOn[]{ ExecuteOn.LevelSelector }), 
            new ScriptInitAttributes(typeof(LevelResizeGlobal),                 new ExecuteOn[]{ ExecuteOn.Level }), 
            new ScriptInitAttributes(typeof(ResizeGlobal),                      new ExecuteOn[]{ ExecuteOn.All })
            //scripts and place ----------------------
        }),

        new InitializationPriority( 2, 
        new List<ScriptInitAttributes> {
            //scripts and place ----------------------
            new ScriptInitAttributes(typeof(PlugSelectorController),    new ExecuteOn[]{ ExecuteOn.Level }), 
            new ScriptInitAttributes(typeof(ElectricalStripController), new ExecuteOn[]{ ExecuteOn.Level }), 
            new ScriptInitAttributes(typeof(ElectricalStripData),       new ExecuteOn[]{ ExecuteOn.Level }), 
            new ScriptInitAttributes(typeof(GridsSkeleton),             new ExecuteOn[]{ ExecuteOn.Level }), 
            new ScriptInitAttributes(typeof(JointsController),          new ExecuteOn[]{ ExecuteOn.Level }), 
            new ScriptInitAttributes(typeof(JointsData),                new ExecuteOn[]{ ExecuteOn.Level }), 
            new ScriptInitAttributes(typeof(PlugAttributes),            new ExecuteOn[]{ ExecuteOn.Level }), 
            new ScriptInitAttributes(typeof(PlugHandler),               new ExecuteOn[]{ ExecuteOn.Level }),
            new ScriptInitAttributes(typeof(WinningControllerGlobal),   new ExecuteOn[]{ ExecuteOn.Level })
            //scripts and place ----------------------
        }),

        new InitializationPriority( 3, 
        new List<ScriptInitAttributes> {
            //scripts and place ----------------------
            new ScriptInitAttributes(typeof(CableHandler), new ExecuteOn[]{ ExecuteOn.Level })
            //scripts and place ----------------------
        }),

        new InitializationPriority( 4, 
        new List<ScriptInitAttributes> {
            //scripts and place ----------------------
            new ScriptInitAttributes(typeof(ObstacleAttributes), new ExecuteOn[]{ ExecuteOn.Level }), 
            new ScriptInitAttributes(typeof(ObstacleHandler),    new ExecuteOn[]{ ExecuteOn.Level })
            //scripts and place ----------------------
        }),

        new InitializationPriority( 5, 
        new List<ScriptInitAttributes> {
            //scripts and place ----------------------
            new ScriptInitAttributes(typeof(GridsController), new ExecuteOn[]{ ExecuteOn.Level })
            //scripts and place ----------------------
        }),

        new InitializationPriority( 6, 
        new List<ScriptInitAttributes> {
            //scripts and place ----------------------
            new ScriptInitAttributes(typeof(ControlsController),     new ExecuteOn[]{ ExecuteOn.All }),
            new ScriptInitAttributes(typeof(IntersectionController), new ExecuteOn[]{ ExecuteOn.Level })
            //scripts and place ----------------------
        }),

        new InitializationPriority( 7, 
        new List<ScriptInitAttributes> {
            //scripts and place ----------------------
            new ScriptInitAttributes(typeof(LevelInitializerGlobal), new ExecuteOn[]{ ExecuteOn.Level })
            //scripts and place ----------------------
        }),

        new InitializationPriority( 8, 
        new List<ScriptInitAttributes> {
            //scripts and place ----------------------
            new ScriptInitAttributes(typeof(LevelTitleGlobal),      new ExecuteOn[]{ ExecuteOn.Level }), 
            new ScriptInitAttributes(typeof(PowerSwitchController), new ExecuteOn[]{ ExecuteOn.Level })
            //new ScriptInitAttributes(typeof(ScenesController),      ExecuteOn.All)
            //scripts and place ----------------------
        })
    };


    public static List<InitializationPriority> loadList = new List<InitializationPriority> {
        new InitializationPriority( 0, 
        new List<ScriptInitAttributes> {
            //scripts and place ----------------------
            new ScriptInitAttributes(typeof(SettingsGlobal),                 new ExecuteOn[]{ ExecuteOn.Start }),
            new ScriptInitAttributes(typeof(MenuInitializerGlobal),          new ExecuteOn[]{ ExecuteOn.Menu }),
            new ScriptInitAttributes(typeof(LevelSelectorInitializerGlobal), new ExecuteOn[]{ ExecuteOn.LevelSelector }),
            new ScriptInitAttributes(typeof(LevelInitializerGlobal),         new ExecuteOn[]{ ExecuteOn.Level })
            //scripts and place ----------------------
        }),

        new InitializationPriority( 1, 
        new List<ScriptInitAttributes> {
            //scripts and place ----------------------
            new ScriptInitAttributes(typeof(PowerSwitchController), new ExecuteOn[]{ ExecuteOn.Level })
            //scripts and place ----------------------
        }),
    };

    
    public static List<InitializationPriority> saveList = new List<InitializationPriority> {
        new InitializationPriority( 0, 
        new List<ScriptInitAttributes> {
            //scripts and place ----------------------
            new ScriptInitAttributes(typeof(SettingsGlobal),         new ExecuteOn[]{ ExecuteOn.Quit }),
            new ScriptInitAttributes(typeof(LevelInitializerGlobal), new ExecuteOn[]{ ExecuteOn.Level, ExecuteOn.Quit })
            //scripts and place ----------------------
        }),

        new InitializationPriority( 1, 
        new List<ScriptInitAttributes> {
            //scripts and place ----------------------
            new ScriptInitAttributes(typeof(WinningControllerGlobal), new ExecuteOn[]{ ExecuteOn.Level, ExecuteOn.Quit })
            //scripts and place ----------------------
        }),
    };
}



public class InitializationPriority {
    private int priority;
    private List<ScriptInitAttributes> scriptInitAttributes;
    public InitializationPriority(int priority, List<ScriptInitAttributes> scriptInitAttributes) {
        this.priority = priority;
        this.scriptInitAttributes = scriptInitAttributes;
    }

    public int GetPriority() {
        return priority;
    }
    public List<ScriptInitAttributes> GetScriptInitAttributes() {
        return scriptInitAttributes;
    }
}

public class ScriptInitAttributes {
    private Type scriptType;
    private ExecuteOn[] scriptExecuteOn;
    public ScriptInitAttributes(Type scriptType, ExecuteOn[] scriptExecuteOn) {
        this.scriptType = scriptType;
        this.scriptExecuteOn = scriptExecuteOn;
    }

    public Type GetScriptType() {
        return scriptType;
    }
    public ExecuteOn[] GetScriptExecuteOn() {
        return scriptExecuteOn;
    }
}

public enum ExecuteOn {
    Quit,            //Will only execute when the game quits
    All,             //Will execute every time the scene is changed
    Start,           //Will only execute once at the start game launch
    Menu,            //Will execute every time the player enters the menu
    LevelSelector,   //Will execute every time the player enters the level selector
    Level            //Will execute every time the player enters any level
}