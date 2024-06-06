using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Dictionary<int, Player> players;

    [SerializeField]
    private GameManager gameManager;

    public void Start() {
        players = new Dictionary<int, Player>();
        Load();
        
    }

    public void InitPlayers() {
        players = new Dictionary<int, Player>();
        for(int i = 0; i < gameManager.GetNumPlayers(); i++) {
            Player player = new Player(i);
            players.Add(i, player);
            player.SetMoney(100);
        }
    }

    public Dictionary<int, Player> GetPlayers() {
        return players;
    }

    public Player GetPlayer(int id) {
        return players[id];
    }

    public void SetPlayers(Dictionary<int, Player> players) {
        this.players = players;
    }

    public void Save() {
        PlayerDAO dao = new PlayerDAO();
        dao.Save(this);
    }

    public void Load() {
        PlayerDAO dao = new PlayerDAO();
        if (!dao.Load(this)) InitPlayers();
    }
}
