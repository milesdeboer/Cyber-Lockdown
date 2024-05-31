using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int VALUE_SCALE = 100;
    public static int DATA_CENTERS_PER_PLAYER = 3;

    public static bool READABLE_SAVE = true;
    
    [SerializeField]
    private int numPlayers = 2;

    private int turnPlayer = 0;

    private int turnNumber = 1;

    private int[] players;

    private List<Player> playerObjects;

    [SerializeField]
    private DataCenterManager dataCenterManager;


    public void Start() {
        players = Enumerable.Range(0, 2).ToArray();
        playerObjects = new List<Player>();
        foreach(int i in players) {
            playerObjects.Add(new Player(i));
        }
    }

    /**
     *  Returns the number of players in the game.
     *  @returns {int} - The number of players in the game.
     */
    public int GetNumPlayers() {
        return numPlayers;
    }

    public int GetTurnPlayer() {
        return turnPlayer;
    }

    public int[] GetPlayers() {
        return players;
    }

    public int GetTurnNumber() {
        return turnNumber;
    }

    public void Save() {
        GameDAO gameDAO = new GameDAO();
        gameDAO.Save(this);

        DataCenterDAO dataDAO = new DataCenterDAO();
        dataDAO.Save(dataCenterManager);
    }
}
