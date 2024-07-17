using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour, ISavable
{
    public static int VALUE_SCALE = 100;
    public static int DATA_CENTERS_PER_PLAYER = 3;
    public static int MALWARE_PER_PLAYER = 8;
    public static int ATTACKS_PER_PLAYER = 8;

    public static bool READABLE_SAVE = true;

    private static int numPlayers = 2;

    private static int turnPlayer = 0;

    private static int turnNumber = 1;

    private List<Color> colors;

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
    private GoalManager goalManager;

    [SerializeField]
    private GameObject turnNumberObject;
    [SerializeField]
    private GameObject playerNameObject;

    public Color selectionColor;


    public void Start() {
        Load();
        if (turnNumber > 1) playerManager.Load();
        else playerManager.UpdateDisplay();
        goalManager.Load();
        dataCenterManager.Load();
        malwareManager.Load();
        attackManager.Load();
        notificationManager.Load();
        turnNumberObject.GetComponent<TextMeshProUGUI>().SetText("Turn: " + turnNumber.ToString());
        playerNameObject.GetComponent<TextMeshProUGUI>().SetText(PlayerManager.GetPlayer(turnPlayer).GetName());
        InitColors();
    }

    /**
     *  Returns the number of players in the game.
     *  @returns {int} - The number of players in the game.
     */
    public static int GetNumPlayers() {
        return numPlayers;
    }
    public static void SetNumPlayers(int newNumPlayers) {
        numPlayers = newNumPlayers;
    }

    public static int GetTurnPlayer() {
        return turnPlayer;
    }
    public static void SetTurnPlayer(int player) {
        turnPlayer = player;
    }

    public static int GetTurnNumber() {
        return turnNumber;
    }

    public static void SetTurnNumber(int newTurnNumber) {
        turnNumber = newTurnNumber;
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
        turnPlayer = ((turnNumber-1) % numPlayers);
        GameDAO dao = new GameDAO();
        dao.Save();
    }

    public void Load() {
        GameDAO dao = new GameDAO();
        dao.Load();
    }

    public void EndTurn() {
        malwareManager.Work();
        attackManager.Work();
        dataCenterManager.Work();

        Player player = PlayerManager.GetPlayer(GameManager.GetTurnPlayer());
        player.Work();
        if (player.GetUnlock(GoalManager.GetWorkTarget()) >= goalManager.GetGoal(GoalManager.GetWorkTarget()).GetWorkRequired()) {
            player.SetAvailableResources(player.GetAvailableResources() + player.GetWorkRate());
            player.SetWorkRate(0);
        }
        
        if (WinCheck()) return;
        Save();
        playerManager.Save();
        dataCenterManager.Save();
        malwareManager.Save();
        attackManager.Save();
        notificationManager.Save();
        SceneManager.LoadScene("BetweenScene");
    }

    private bool WinCheck() {
        string winner = null;
        int target = goalManager.GetGoals().ToArray()[^1].Value.GetWorkRequired();

        PlayerManager
            .GetPlayers()
            .ToList()
            .Select(kvp => kvp.Value)
            .Where(p => p.GetUnlocks()[^1] >= target)
            .Take(1)
            .ToList()
            .ForEach(p => winner = p.GetName());

        if (winner != null) {
            EndGameManager.SetWinner(winner);
            SceneManager.LoadScene("EndGameScene");
            return true;
        } else {
            return false;
        }
    }

    public void MainMenu() {
        SceneManager.LoadScene("TitleScene");
    }
    public void ExitGame() {
        Application.Quit();
    }
}
