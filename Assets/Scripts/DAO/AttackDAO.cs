using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class AttackDAO 
{
    public AttackWrapper[] attacks;

    public void Save(AttackManager manager, int pid) {
        attacks = manager.GetAttacks().Select(a => a.Wrap()).ToArray();

        string json = JsonUtility.ToJson(this, GameManager.READABLE_SAVE);
        File.WriteAllText(Application.persistentDataPath + "/attacksave.json", json);

        Debug.Log("json: " + json);
        Debug.Log("Save Location: " + Application.persistentDataPath + "/attacksave.json");
    }

    public void Load(AttackManager manager) {
        if(File.Exists(Application.persistentDataPath + "/attacksave.json")) {
            string json = File.ReadAllText(Application.persistentDataPath + "/attacksave.json");
            AttackDAO temp = JsonUtility.FromJson<AttackDAO>(json);

            List<Attack> attacks_ = new List<Attack>();
            foreach(AttackWrapper wrapper in temp.attacks) {
                attacks_.Add(wrapper.Unwrap());
            }

            manager.SetAttacks(attacks_);
        }
    }
}
