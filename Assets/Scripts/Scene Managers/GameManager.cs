using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour, ISavable
{
    public static int VALUE_SCALE = 100;
    public static int DATA_CENTERS_PER_PLAYER = 4;// 2 - 4 
    public static int MALWARE_PER_PLAYER = 8;
    public static int ATTACKS_PER_PLAYER = 8;

    public static bool READABLE_SAVE = false;
    public static Vector2 SCREEN_DIMENSION = new Vector2(1920, 1080);

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

    private static bool usingLobby = false;

    private bool loadLimiter = false;

    public Color selectionColor;


    public void Start() {
        // Download from Lobby
        LoadAll();
        turnNumberObject.GetComponent<TextMeshProUGUI>().SetText("Turn: " + turnNumber.ToString());
        playerNameObject.GetComponent<TextMeshProUGUI>().SetText(PlayerManager.GetPlayer(turnPlayer).GetName());
    }

    public void LoadAll() {
        if (usingLobby) {
            LobbyViewer.PullLobbyUpdate();
            LobbyViewer.InitGameManager();
        }

        Load();
        if (turnNumber > 1) playerManager.Load();
        else playerManager.UpdateDisplay();
        InitColors();
        goalManager.Load();
        dataCenterManager.Load();
        dataCenterManager.Start_();
        malwareManager.Load();
        attackManager.Load();
        notificationManager.Load();
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

        // Lobby Upload
        if (usingLobby) LobbyViewer.PushLobbyUpdate();

        StartCoroutine(LoadSceneDelay("BetweenScene"));
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
            StartCoroutine(LoadSceneDelay("EndGameScene"));
            return true;
        } else {
            return false;
        }
    }

    public void MainMenu() {
        StartCoroutine(LoadSceneDelay("TitleScene"));
    }
    public void ExitGame() {
        StartCoroutine(ExitDelay());
    }

    IEnumerator LoadSceneDelay(string sceneName) {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator ExitDelay() {
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }

    public static void UseLobby(bool useLobby) {
        usingLobby = useLobby;
    }
    public static bool IsUsingLobby() {
        return usingLobby;
    }
}
