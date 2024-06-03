using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static int VALUE_SCALE = 100;
    public static int DATA_CENTERS_PER_PLAYER = 3;
    public static int MALWARE_PER_PLAYER = 8;
    public static int ATTACKS_PER_PLAYER = 8;

    public static bool READABLE_SAVE = true;
    
    [SerializeField]
    private int numPlayers = 2;

    private int turnPlayer = 0;

    private int turnNumber = 1;

    private int[] players;

    private List<Player> playerObjects;

    [SerializeField]
    private DataCenterManager dataCenterManager;
    [SerializeField]
    private MalwareController malwareManager;
    [SerializeField]
    private AttackManager attackManager;

    [SerializeField]
    private GameObject turnNumberObject;


    public void Start() {
        players = Enumerable.Range(0, numPlayers).ToArray();
        playerObjects = new List<Player>();
        foreach(int i in players) {
            playerObjects.Add(new Player(i));
        }
        Load();
        dataCenterManager.Load();
        malwareManager.Load();
        attackManager.Load();
        turnNumberObject.GetComponent<TextMeshProUGUI>().SetText("Turn: " + turnNumber.ToString());
    }

    /**
     *  Returns the number of players in the game.
     *  @returns {int} - The number of players in the game.
     */
    public int GetNumPlayers() {
        return numPlayers;
    }
    public void SetNumPlayers(int numPlayers) {
        this.numPlayers = numPlayers;
    }

    public int GetTurnPlayer() {
        return turnPlayer;
    }
    public void SetTurnPlayer(int turnPlayer) {
        this.turnPlayer = turnPlayer;
    }

    public int[] GetPlayers() {
        return players;
    }
    public void SetPlayers(int[] players) {
        this.players = players;
    }

    public int GetTurnNumber() {
        return turnNumber;
    }

    public void SetTurnNumber(int turnNumber) {
        this.turnNumber = turnNumber;
    }

    public void Save() {
        turnNumber++;
        turnPlayer = ((turnNumber + 1) % numPlayers);
        GameDAO dao = new GameDAO();
        dao.Save(this);
    }

    public void Load() {
        GameDAO dao = new GameDAO();
        dao.Load(this);
    }

    public void EndTurn() {
        SceneManager.LoadScene("PlayerScene");
    }
}
