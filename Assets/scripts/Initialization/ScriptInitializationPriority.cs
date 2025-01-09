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
            new List<Type> {
                --------------------------
                >>> place scripts here <<<
                --------------------------
            }),
            ...
        };
    */

    public static List<InitializationPriority> list = new List<InitializationPriority> {
        new InitializationPriority( 0, 
        new List<Type> {
            //scripts ----------------------
            typeof(AdminToggles), typeof(ButtonsAttributes), typeof(ButtonsHandler), typeof(ButtonsOutlineLocal), typeof(ControlsData), typeof(SliderAttributes), typeof(SoundsController), typeof(TutorialController), typeof(SocketAttributes), typeof(SocketHandler), typeof(JointsOpacityGlobal), typeof(LevelSelectorBackgroundData), typeof(WinningMessageSizeGlobal), typeof(InitializeBackground)
            //scripts ----------------------
        }),

        new InitializationPriority( 1, 
        new List<Type> {
            //scripts ----------------------
            typeof(PlugSelectorData), typeof(ControlsController), typeof(CreditsGlobal), typeof(LevelSelectorInitializerGlobal), typeof(MenuInitializerGlobal), typeof(LevelSelectorBackgroundController), typeof(LevelResizeGlobal), typeof(ResizeGlobal)
            //scripts ----------------------
        }),

        new InitializationPriority( 2, 
        new List<Type> {
            //scripts ----------------------
            typeof(PlugSelectorController), typeof(CableHandler), typeof(CableParentAttributes), typeof(ElectricalStripController), typeof(ElectricalStripData), typeof(GridsModifier), typeof(JointsController), typeof(JointsData), typeof(WinningControllerGlobal)
            //scripts ----------------------
        }),

        new InitializationPriority( 3, 
        new List<Type> {
            //scripts ----------------------
            typeof(PlugAttributes), typeof(PlugHandler)
            //scripts ----------------------
        }),

        new InitializationPriority( 4, 
        new List<Type> {
            //scripts ----------------------
            typeof(IntersectionController)
            //scripts ----------------------
        }),

        new InitializationPriority( 5, 
        new List<Type> {
            //scripts ----------------------
            typeof(ObstacleAttributes), typeof(ObstacleHandler)
            //scripts ----------------------
        }),

        new InitializationPriority( 6, 
        new List<Type> {
            //scripts ----------------------
            typeof(GridsController)
            //scripts ----------------------
        }),

        new InitializationPriority( 7, 
        new List<Type> {
            //scripts ----------------------
            typeof(LevelInitializerGlobal)
            //scripts ----------------------
        }),

        new InitializationPriority( 8, 
        new List<Type> {
            //scripts ----------------------
            typeof(LevelTitleGlobal), typeof(PowerSwitchController), typeof(ScenesController)
            //scripts ----------------------
        })
    };

}



public class InitializationPriority {
    private int priority;
    private List<Type> types;
    public InitializationPriority(int priority, List<Type> types) {
        this.priority = priority;
        this.types = types;
    }

    public int GetPriority() {
        return priority;
    }
    public List<Type> GetTypes() {
        return types;
    }
}