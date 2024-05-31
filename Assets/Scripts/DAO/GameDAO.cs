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

    public void Save(GameManager manager) {
        numPlayers = manager.GetNumPlayers();
        turnPlayer = manager.GetTurnPlayer();
        turnNum = manager.GetTurnNumber();
        players = manager.GetPlayers();

        string json = JsonUtility.ToJson(this, GameManager.READABLE_SAVE);
        Debug.Log("JSON: " + json);

        File.WriteAllText(Application.persistentDataPath + "/gamesave.json", json);
        Debug.Log("Save Location: " + Application.persistentDataPath + "/gamesave.json");
    }
}
