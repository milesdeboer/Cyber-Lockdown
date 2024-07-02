using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerDAO : IDAO
{
    public PlayerWrapper[] players;

    public bool Save(ISavable savable) {
        PlayerManager manager = (PlayerManager) savable; 
        players = PlayerManager.GetPlayers().Select(p => p.Value.Wrap()).ToArray();

        string json = JsonUtility.ToJson(this, GameManager.READABLE_SAVE);
        File.WriteAllText(Application.persistentDataPath + "/playersave.json", json);
        
        return true;
    }

    public bool Load(ISavable savable) {
        PlayerManager manager = (PlayerManager) savable; 
        if(File.Exists(Application.persistentDataPath + "/playersave.json")) {
            string json = File.ReadAllText(Application.persistentDataPath + "/playersave.json");
            PlayerDAO temp = JsonUtility.FromJson<PlayerDAO>(json);
            
            Dictionary<int, Player> players_ = new Dictionary<int, Player>();
            foreach(PlayerWrapper wrapper in temp.players) {
                Player p = wrapper.Unwrap();
                players_.Add(p.GetId(), p);
            }
            PlayerManager.SetPlayers(players_);

            return true;
        } else {
            return false;
        }
    }

    public bool Erase() {
        if(File.Exists(Application.persistentDataPath + "/playersave.json")) {
            File.Delete(Application.persistentDataPath + "/playersave.json");
            return true;
        } else return false;
    }
}
