using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DataCenterDAO
{
    public DataCenterWrapper[] dataCenters;

    public void Save(DataCenterManager manager) {
        dataCenters = manager.GetDataCenters().Select(dc => dc.Wrap()).ToArray();

        string json = JsonUtility.ToJson(this, GameManager.READABLE_SAVE);
        File.WriteAllText(Application.persistentDataPath + "/datacentersave.json", json);
        
        Debug.Log("json: " + json);
        Debug.Log("Save Location: " + Application.persistentDataPath + "/datacentersave.json");
    }
}
