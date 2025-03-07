using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;

public class FileDataHandler {
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName) {
        this.dataDirPath  = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load() {
        //Use Path.Combine to account for different OS's having different path separators.
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if(File.Exists(fullPath)) {
            try {
                //Load the serialized data from the file
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open)) {
                    using(StreamReader reader = new StreamReader(stream)) {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //Deserialize the data from Json back into the C# object
                ////loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                loadedData = JsonConvert.DeserializeObject<GameData>(dataToLoad);
            }
            catch(Exception e) {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(GameData data) {
        //Use Path.Combine to account for different OS's having different path separators.
        Debug.Log("Savedd:: ");
        data.Log();
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try {
            //Create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //Serialize the C# game data object into Json
            //string dataToStore = JsonUtility.ToJson(gameDataSerializable, true);


            ////string dataToStore = JsonConvert.SerializeObject(data, Formatting.Indented);
            string dataToStore = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            Debug.Log($"dataToStore: {dataToStore}");
            //Write the serialized data to the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create)) {
                using(StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(dataToStore);
                }
            }
        }
        catch(Exception e) {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
}
