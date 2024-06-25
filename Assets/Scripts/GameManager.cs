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

    private List<Color> colors;

    private int[] players;

    private List<Player> playerObjects;

    [SerializeField]
    private DataCenterManager dataCenterManager;
    [SerializeField]
    private MalwareController malwareManager;
    [SerializeField]
    private AttackManager attackManager;
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private NotificationManager notificationManager;

    [SerializeField]
    private GameObject turnNumberObject;

    public Color selectionColor;


    public void Start() {
        players = Enumerable.Range(0, numPlayers).ToArray();
        Load();
        playerManager.Load();
        dataCenterManager.Load();
        malwareManager.Load();
        attackManager.Load();
        notificationManager.Load();
        turnNumberObject.GetComponent<TextMeshProUGUI>().SetText("Turn: " + turnNumber.ToString());
        InitColors();
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

    public List<Color> GetColors() {
        return colors;
    }

    private void InitColors() {
        colors = new List<Color>();
        for(int i = 0; i < numPlayers; i++) {
            colors.Add(Color.HSVToRGB((float) i / (numPlayers+1), 0.75f, 0.75f));
        }
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
