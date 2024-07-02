using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class AttackDAO : IDAO
{
    public AttackWrapper[] attacks;

    public bool Save(ISavable savable) {
        AttackManager manager = (AttackManager) savable;
        attacks = manager.GetAttacks().Select(a => a.Value.Wrap()).ToArray();

        string json = JsonUtility.ToJson(this, GameManager.READABLE_SAVE);
        File.WriteAllText(Application.persistentDataPath + "/attacksave.json", json);
        return true;
    }

    public bool Load(ISavable savable) {
        AttackManager manager = (AttackManager) savable;
        if(File.Exists(Application.persistentDataPath + "/attacksave.json")) {
            string json = File.ReadAllText(Application.persistentDataPath + "/attacksave.json");
            AttackDAO temp = JsonUtility.FromJson<AttackDAO>(json);

            Dictionary<int, Attack> attacks_ = new Dictionary<int, Attack>();
            foreach(AttackWrapper wrapper in temp.attacks) {
                Attack attack = wrapper.Unwrap();
                attacks_.Add(attack.GetId(), attack);
            }

            manager.SetAttacks(attacks_);

            return true;
        } else return false;
    }

    public bool Erase() {
        if(File.Exists(Application.persistentDataPath + "/attacksave.json")) {
            File.Delete(Application.persistentDataPath + "/attacksave.json");
            return true;
        } else return false;
    }
}
