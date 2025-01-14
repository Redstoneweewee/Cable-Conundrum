using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;


// |----------------------------------------------------------------------------------|
// |----------------------------------------------------------------------------------|
// |----- Save and LoadData does NOT work with PlugSelector dragged in plugs!!!! -----|
// |----------------------------------------------------------------------------------|
// |----------------------------------------------------------------------------------|
public class DataPersistenceManager : Singleton<DataPersistenceManager> {
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    public bool FinishedLoading { get; private set; } = false;
    public bool FinishedSaving { get; private set; } = false;


    private GameData gameData;
    private List<DataPersistentSingleton> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public override IEnumerator Initialize() {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        yield return null;
    }

    private void OnApplicationQuit() {
        SaveGame(true);
    }


    public void NewGame() {
        this.gameData = new GameData();
    }

    public void LoadGame() {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        //Load any saved data from a file using the data handler
        this.gameData = dataHandler.Load();

    
        //If no data can be loaded, initialize to a new game
        if(this.gameData == null) {
            DebugC.Instance.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }
        
        //Makes sure there's a spot to save the data at later
        while(this.gameData.levelsSavePlugs.Count < Constants.numberOfLevels) {
            this.gameData.levelsSavePlugs.Add(new List<SavePlug>());
        }
        while(this.gameData.levelCompletion.Count < Constants.numberOfLevels) {
            this.gameData.levelCompletion.Add(false);
        }
        

        FinishedLoading = false;
        int startFrame = Time.frameCount;
        float startTime = Time.time;
        string debugText = "Loading Log for Scene ["+SceneManager.GetActiveScene().name+"]:\n";
        foreach(InitializationPriority priority in ScriptInitializationPriority.loadList) {
            foreach(ScriptInitAttributes ScriptInitAttributes in priority.GetScriptInitAttributes()) {
                
                if(!Utilities.ShouldExecute(ScriptInitAttributes.GetScriptExecuteOn())) { continue; }
                List<UnityEngine.Object> objs = FindObjectsOfType(ScriptInitAttributes.GetScriptType()).ToList();
                List<bool> isEachLoaded = new List<bool>( new bool[objs.Count] );
                
                int deltaFrame = Time.frameCount - startFrame;
                float deltaTime = Time.time - startTime;
                debugText += "\n["+deltaFrame+"/"+deltaTime+" frm] Found "+objs.Count+" instance(s) of ["+ScriptInitAttributes.GetScriptType().Name+"]\n";
                //Debug.Log("["+deltaFrame+"/"+deltaTime+" frm] Found "+objs.Count+" instance(s) of ["+ScriptInitAttributes.GetScriptType().Name+"]");
            
                for(int i=objs.Count-1; i>=0; i--) {
                    DataPersistentSingleton dataPersistentSingleton = (DataPersistentSingleton)objs[i];
                    //StartCoroutine(dataPersistentSingleton.TrackLoadCoroutine(dataPersistentSingleton.LoadData(gameData)));
                    dataPersistentSingleton.LoadData(gameData);
                    deltaFrame  = Time.frameCount - startFrame;
                    deltaTime = Time.time - startTime;
                    debugText += "["+deltaFrame+"/"+deltaTime+" frm] Finished Loading #"+(i+1)+" ["+objs[i].name+"]\n";
                }
            }
            debugText += "\n-------------------------------------------\n>>>>> Loaded priority "+priority.GetPriority()+" finished <<<<<\n-------------------------------------------\n";
            //Debug.Log(">>>>> Initialized priority "+priority.GetPriority()+" finished <<<<<");
        }
        FinishedLoading = true;
        Debug.Log(debugText);








        /*
        //Push the loaded data to all other scripts that need it
        foreach(DataPersistentSingleton dataPersistenceObject in dataPersistenceObjects) {
            StartCoroutine(dataPersistenceObject.LoadData(gameData));
        }
        */
    }

    public void SaveGame(bool isQuitting = false) {
        //Pass the data to other scripts so they can update it
        /*
        foreach(DataPersistentSingleton dataPersistenceObject in dataPersistenceObjects) {
            dataPersistenceObject.SaveData(gameData);
        }
        foreach(DataPersistentSingleton dataPersistenceObject in dataPersistenceObjects) {
            dataPersistenceObject.SaveDataLate(gameData);
        }
        */

        
        FinishedSaving = false;
        int startFrame = Time.frameCount;
        float startTime = Time.time;
        string debugText = "Saving Log for Scene ["+SceneManager.GetActiveScene().name+"]:\n";
        foreach(InitializationPriority priority in ScriptInitializationPriority.saveList) {
            foreach(ScriptInitAttributes ScriptInitAttributes in priority.GetScriptInitAttributes()) {
                
                if(!Utilities.ShouldExecute(ScriptInitAttributes.GetScriptExecuteOn(), isQuitting)) { continue; }
                List<UnityEngine.Object> objs = FindObjectsOfType(ScriptInitAttributes.GetScriptType()).ToList();
                List<bool> isEachLoaded = new List<bool>( new bool[objs.Count] );
                
                int deltaFrame = Time.frameCount - startFrame;
                float deltaTime = Time.time - startTime;
                debugText += "\n["+deltaFrame+"/"+deltaTime+" frm] Found "+objs.Count+" instance(s) of ["+ScriptInitAttributes.GetScriptType().Name+"]\n";
                //Debug.Log("["+deltaFrame+"/"+deltaTime+" frm] Found "+objs.Count+" instance(s) of ["+ScriptInitAttributes.GetScriptType().Name+"]");
            
                for(int i=objs.Count-1; i>=0; i--) {
                    DataPersistentSingleton dataPersistentSingleton = (DataPersistentSingleton)objs[i];
                    //StartCoroutine(dataPersistentSingleton.TrackLoadCoroutine(dataPersistentSingleton.LoadData(gameData)));
                    dataPersistentSingleton.SaveData(gameData);
                    deltaFrame  = Time.frameCount - startFrame;
                    deltaTime = Time.time - startTime;
                    debugText += "["+deltaFrame+"/"+deltaTime+" frm] Finished Loading #"+(i+1)+" ["+objs[i].name+"]\n";
                }
            }
            debugText += "\n-------------------------------------------\n>>>>> Loaded priority "+priority.GetPriority()+" finished <<<<<\n-------------------------------------------\n";
            //Debug.Log(">>>>> Initialized priority "+priority.GetPriority()+" finished <<<<<");
        }
        FinishedSaving = true;
        Debug.Log(debugText);

        //Save that data to a file using the data handler
        dataHandler.Save(gameData);
    }





    private List<DataPersistentSingleton> FindAllDataPersistenceObjects() {
        IEnumerable<DataPersistentSingleton> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<DataPersistentSingleton>();
        return new List<DataPersistentSingleton>(dataPersistenceObjects);
    }

    /*
    private bool AllLoaded(List<UnityEngine.Object> objs, ref string debugText, int startFrame = 0, float startTime = 0, List<bool> isEachLoaded = null) {
        //Debugging part - not needed in the actual function
        if(isEachLoaded != null) {
            for(int i=0; i<objs.Count; i++) {
                DataPersistentSingleton dataPersistentSingleton = (DataPersistentSingleton)objs[i];
                if(!isEachLoaded[i] && dataPersistentSingleton.Loaded) {
                    int deltaFrame  = Time.frameCount - startFrame;
                    float deltaTime = Time.time - startTime;
                    debugText += "["+deltaFrame+"/"+deltaTime+" frm] Finished Loading #"+(i+1)+" ["+objs[i].name+"]\n";
                }
                isEachLoaded[i] = dataPersistentSingleton.Loaded;
            }
            
        }
        //Optimized part
        for(int i=0; i<objs.Count; i++) {
            DataPersistentSingleton dataPersistentSingleton = (DataPersistentSingleton)objs[i];
            if(!dataPersistentSingleton.Loaded) {
                return false;
            }
        }
        return true;
    }
    */
}
