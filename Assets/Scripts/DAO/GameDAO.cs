using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class GameDAO
{
    public int numPlayers;
    public int turnPlayer;
    public int turnNum;
    public int[] players;

    public bool Save() {
        numPlayers = GameManager.GetNumPlayers();
        turnPlayer = GameManager.GetTurnPlayer();
        turnNum = GameManager.GetTurnNumber();

        string json = JsonUtility.ToJson(this, GameManager.READABLE_SAVE);
        File.WriteAllText(Application.persistentDataPath + "/gamesave.json", json);

        return true;
    }

    public bool Load() {
        if (File.Exists(Application.persistentDataPath + "/gamesave.json")) {
            string json = File.ReadAllText(Application.persistentDataPath + "/gamesave.json");
            GameDAO temp = JsonUtility.FromJson<GameDAO>(json);

            GameManager.SetNumPlayers(temp.numPlayers);
            GameManager.SetTurnPlayer(temp.turnPlayer);
            GameManager.SetTurnNumber(temp.turnNum);
            return true;
        } else return false;
    }

    public bool Erase() {
        if(File.Exists(Application.persistentDataPath + "/gamesave.json")) {
            File.Delete(Application.persistentDataPath + "/gamesave.json");
            return true;
        } else return false;
    }
}
