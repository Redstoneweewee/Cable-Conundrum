using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// |----------------------------------------------------------------------------------|
// |----------------------------------------------------------------------------------|
// |----- Save and LoadData does NOT work with PlugSelector dragged in plugs!!!! -----|
// |----------------------------------------------------------------------------------|
// |----------------------------------------------------------------------------------|
public class DataPersistenceManager : MonoBehaviour {
    [Header("File Storage Config")]
    [SerializeField] private string fileName;


    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }

    void Awake() {
        if(instance != null && instance != this) {
            Destroy(this.gameObject);
        }
        else {
            instance = this;

            this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
            LoadGame();
        }
    }

    private void OnApplicationQuit() {
        SaveGame();
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
            DebugC.Get()?.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }
        
        //Makes sure there's a spot to save the data at later
        while(this.gameData.levelsSavePlugs.Count <= Constants.numberOfLevels) {
            this.gameData.levelsSavePlugs.Add(new List<SavePlug>());
        }
        while(this.gameData.levelCompletion.Count <= Constants.numberOfLevels) {
            this.gameData.levelCompletion.Add(false);
        }
        
        //Push the loaded data to all other scripts that need it
        foreach(IDataPersistence dataPersistenceObject in dataPersistenceObjects) {
            StartCoroutine(dataPersistenceObject.LoadData(gameData));
        }
    }

    public void SaveGame() {
        //Pass the data to other scripts so they can update it
        foreach(IDataPersistence dataPersistenceObject in dataPersistenceObjects) {
            dataPersistenceObject.SaveData(gameData);
        }
        foreach(IDataPersistence dataPersistenceObject in dataPersistenceObjects) {
            dataPersistenceObject.SaveDataLate(gameData);
        }

        //Save that data to a file using the data handler
        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
