using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DataCenterDAO : IDAO
{
    public DataCenterWrapper[] dataCenters;

    public bool Save(ISavable savable) {
        DataCenterManager manager = (DataCenterManager) savable;
        dataCenters = manager.GetDataCenters().Select(dc => dc.Wrap()).ToArray();

        string json = JsonUtility.ToJson(this, GameManager.READABLE_SAVE);
        File.WriteAllText(Application.persistentDataPath + "/datacentersave.json", json);
        
        return true;
    }

    public bool Load(ISavable savable) {
        DataCenterManager manager = (DataCenterManager) savable;
        if (File.Exists(Application.persistentDataPath + "/datacentersave.json")) {
            string json = File.ReadAllText(Application.persistentDataPath + "/datacentersave.json");
            DataCenterDAO temp = JsonUtility.FromJson<DataCenterDAO>(json);

            List<DataCenter> dataCenters_ = new List<DataCenter>();
            foreach(DataCenterWrapper wrapper in temp.dataCenters) {
                dataCenters_.Add(wrapper.Unwrap());
            }

            manager.SetDataCenters(dataCenters_);

            return true;
        } else return false;
    }

    public bool Erase() {
        if(File.Exists(Application.persistentDataPath + "/datacentersave.json")) {
            File.Delete(Application.persistentDataPath + "/datacentersave.json");
            return true;
        } else return false;
    }
}
