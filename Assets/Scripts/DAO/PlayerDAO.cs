using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerDAO
{
    public PlayerWrapper[] players;

    public void Save(PlayerManager manager) {
        players = manager.GetPlayers().Select(p => p.Value.Wrap()).ToArray();

        string json = JsonUtility.ToJson(this, GameManager.READABLE_SAVE);
        File.WriteAllText(Application.persistentDataPath + "/playersave.json", json);
    }

    public void Load(PlayerManager manager) {
        if(File.Exists(Application.persistentDataPath + "/playersave.json")) {
            string json = File.ReadAllText(Application.persistentDataPath + "/playersave.json");
            PlayerDAO temp = JsonUtility.FromJson<PlayerDAO>(json);

            Dictionary<int, Player> players_ = new Dictionary<int, Player>();
            foreach(PlayerWrapper wrapper in temp.players) {
                Player p = wrapper.Unwrap();
                players_.Add(p.GetId(), p);
            }
            manager.SetPlayers(players_);
        }
    }
}
