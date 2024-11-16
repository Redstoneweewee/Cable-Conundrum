using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour {
    [Header("File Storage Config")]
    [SerializeField] private string fileName;


    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }

    void Awake() {
        if(instance != null) {
            Debug.LogError("Found more than one Data Persistence Manager in the scene.");
            Destroy(this);
        }
        instance = this;

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        LoadGame();
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
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }
        //Push the loaded data to all other scripts that need it
        foreach(IDataPersistence dataPersistenceObject in dataPersistenceObjects) {
            StartCoroutine(dataPersistenceObject.LoadData(gameData));
        }
    }

    public void SaveGame() {
        //Pass the data to other scripts so they can update it
        foreach(IDataPersistence dataPersistenceObject in dataPersistenceObjects) {
            dataPersistenceObject.SaveData(ref gameData);
        }

        //Save that data to a file using the data handler
        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
