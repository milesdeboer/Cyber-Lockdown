using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour, ISavable
{
    private static Dictionary<int, Player> players;

    [SerializeField]
    private GameObject resources;
    [SerializeField]
    private GameObject money;

    [SerializeField]
    private GameManager gameManager;

    public static void InitPlayers() {
        players = new Dictionary<int, Player>();
        for(int i = 0; i < GameManager.GetNumPlayers(); i++) {
            Player player = new Player(i);
            players.Add(i, player);
            player.SetMoney(10000);
        }
    }

    public static Dictionary<int, Player> GetPlayers() {
        return players;
    }

    public static Player GetPlayer(int id) {
        return players[id];
    }

    public static void SetPlayers(Dictionary<int, Player> newPlayers) {
        players = newPlayers;
    }

    public void Save() {
        PlayerDAO dao = new PlayerDAO();
        dao.Save(this);
    }

    public void Load() {
        PlayerDAO dao = new PlayerDAO();
        if (!dao.Load(this)) InitPlayers();
        UpdateDisplay();
    }

    public void Clear() {
        players = new Dictionary<int, Player>();
    }

    public void UpdateDisplay() {
        money.GetComponent<TextMeshProUGUI>().SetText("Money: " + GetPlayer(GameManager.GetTurnPlayer()).GetMoney());
        resources.GetComponent<TextMeshProUGUI>().SetText("Resources: " + GetPlayer(GameManager.GetTurnPlayer()).GetAvailableResources());
    }
}
