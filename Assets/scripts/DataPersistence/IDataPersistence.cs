using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence {
    
    //wait until everything in the scriptis done initializing before loading data
    IEnumerator LoadData(GameData data);

    void SaveData(GameData data);
    void SaveDataLate(GameData data);
}
