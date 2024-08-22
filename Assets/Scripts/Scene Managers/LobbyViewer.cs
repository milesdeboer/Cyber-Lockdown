using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using TMPro;

using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyViewer : MonoBehaviour
{
    [SerializeField]
    private GameObject playerEntry;
    [SerializeField]
    private Transform playerEntryContainer;
    [SerializeField]
    private GameObject lobbyNameObject;

    [SerializeField]
    private TMP_InputField maxPlayersObject;
    [SerializeField]
    private TMP_InputField dataCentersPerPlayerObject;

    private static Lobby lobby;

    private static string lobbyId;
    private static int playerId;
    private static string playerName;
    private static string companyName;
    private static bool hosting = false;

    private int numPlayers = 0;

    private bool isPrivate = false;

    private float heartbeat = 0f;

    //-1 - pull
    //0 - none
    //1 - push
    private static int pushpull = 0;

    public void Start() {
        playerName = LobbyManager.GetPlayerName();
        playerId = LobbyManager.GetPlayerId();
        if (hosting) {
            lobbyNameObject.GetComponent<TMP_InputField>().interactable = true;
        } else {
            lobbyNameObject.GetComponent<TMP_InputField>().interactable = false;
            lobbyNameObject.transform.GetChild(1).gameObject.SetActive(false);
            lobbyNameObject.GetComponent<TMP_InputField>().text = lobby.Name;
            ViewPlayers(lobby);
        }
    }

    public void Update() {
        HandleHeartBeat();
    }

    /// <summary>
    /// Periodically (every 15 seconds) sends heartbeat to lobby and checks for new players/if game was started
    /// </summary>
    public async void HandleHeartBeat() {
        if (lobby != null) {
            if (playerId == 0) {
                Debug.Log("Setting Player Id");
                playerId = lobby.Players.Count;
            }
            heartbeat -= Time.deltaTime;
            if (heartbeat < 0f) {
                float heartbeatLimit = 15f;
                heartbeat = heartbeatLimit;

                Debug.Log("Starting Heartbeat " + playerId);

                if (lobby.HostId == AuthenticationService.Instance.PlayerId)
                    await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
                lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);


                Debug.Log("Ending Heartbeat " + playerId + " --- " + lobby.Data["turnNum"].Value);

                if (Int32.Parse(lobby.Data["turnNum"].Value) > 0) {
                    Debug.Log("Starting Host Game");
                    ClientStartGame();
                } else {
                    Debug.Log("Updating Players");
                    ViewPlayers(lobby);
                    numPlayers = lobby.Players.Count;
                }

                heartbeat = heartbeatLimit;
            }
        }
    }

    /// <summary>
    /// Refreshes the list of players displayed.
    /// </summary>
    /// <param name="lobby">Target lobby which has the players that are being viewed.</param>
    public void ViewPlayers(Lobby lobby) {

        // destory all players
        GameObject[] entries = GameObject.FindGameObjectsWithTag("PlayerEntry");
        foreach(GameObject entry in entries) Destroy(entry);

        int i = 0;
        lobby.Players.ForEach(player => {
            Vector3 position = new Vector3((float) Math.Pow(-1, i) * -305f, (float) (i / -2) * 170f + 340f, 0f);
            GameObject entry = Instantiate(playerEntry, position, Quaternion.identity);
            entry.transform.SetParent(playerEntryContainer, false);
            Debug.Log("Updating Player " + i + ": " + ((player.Data["CompanyName"].Value != null) ? player.Data["CompanyName"].Value : "N/A"));
            entry.transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().text = (player.Data["CompanyName"].Value != null) ? player.Data["CompanyName"].Value : "";

            var idx = i;
            var pid = AuthenticationService.Instance.PlayerId;
            entry.transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().onEndEdit.AddListener(async delegate {//
                //await NameChange(entry.transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().text, idx);
                string name = entry.transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().text;
                await Task.Run(async () => await UpdatePlayerData(pid, "CompanyName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, name)));
            });

            if (player.Id == AuthenticationService.Instance.PlayerId) {
                entry.transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().interactable = true;
            } else {
                entry.transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().interactable = false;
            }

            i++;
        });
    }

    /// <summary>
    /// Creates a lobby based on the information filled out in the scene.
    /// </summary>
    public async void CreateLobby() {
        Int32.TryParse(maxPlayersObject.text, out int maxPlayers);
        string dataCentersPerPlayer = dataCentersPerPlayerObject.text;
        string lobbyName = lobbyNameObject.GetComponent<TMP_InputField>().text;
        try {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions {
                IsPrivate = isPrivate,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>() {
                    {"dcpp", new DataObject(DataObject.VisibilityOptions.Member, dataCentersPerPlayer)},
                    {"turnNum", new DataObject(DataObject.VisibilityOptions.Member, "0")},
                    {"AttackSave", new DataObject(DataObject.VisibilityOptions.Member, "")},
                    {"DataCenterSave", new DataObject(DataObject.VisibilityOptions.Member, "")},
                    {"GameSave", new DataObject(DataObject.VisibilityOptions.Member, "")},
                    {"MalwareSave", new DataObject(DataObject.VisibilityOptions.Member, "")},
                    {"NotificationSave", new DataObject(DataObject.VisibilityOptions.Member, "")},
                    {"PlayerSave", new DataObject(DataObject.VisibilityOptions.Member, "")}
                }
            };

            playerId = 1;

            lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            Debug.Log("Successfully Created Lobby! " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);

            ViewPlayers(lobby);
        } catch(LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// onClick listener for the exit lobby button. removes player from the lobby and loads teh search lobbies scene.
    /// </summary>
    public async void ExitLobby() {
        try {
            string playerId = AuthenticationService.Instance.PlayerId;
            await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
        } catch(LobbyServiceException e) {
            Debug.Log(e);
        } catch(ArgumentNullException e) {
            Debug.Log(e);
        } finally {
            SceneManager.LoadScene("SearchLobbies");
        }
    }

    public static void SetHosting(bool isHosting) {
        hosting = isHosting;
    }
    public static bool IsHosting() {
        return hosting;
    }

    public void SetPrivate(bool isPrivate) {
        this.isPrivate = isPrivate;
    }
    public bool IsPrivate() {
        return isPrivate;
    }

    public static Lobby GetLobby() {
        return lobby;
    }

    public static void SetLobby(Lobby joinedLobby) {
        lobby = joinedLobby;
    }

    public async void HostStartGame() {
        //lobby.Data["turnNum"] = new DataObject(DataObject.VisibilityOptions.Member, "1");
        (new AttackDAO()).Erase();
        (new DataCenterDAO()).Erase();
        (new GameDAO()).Erase();
        (new MalwareDAO()).Erase();
        (new NotificationDAO()).Erase();
        (new PlayerDAO()).Erase();
        await UpdateLobbyData("turnNum", new DataObject(DataObject.VisibilityOptions.Member, "1"));
        InitGameManager();
        SceneManager.LoadScene("BetweenScene");
    }

    public void ClientStartGame() {
        (new AttackDAO()).Erase();
        (new DataCenterDAO()).Erase();
        (new GameDAO()).Erase();
        (new MalwareDAO()).Erase();
        (new NotificationDAO()).Erase();
        (new PlayerDAO()).Erase();
        InitGameManager();
        SceneManager.LoadScene("BetweenScene");
    }

    public static void InitGameManager() {
        GameManager.SetNumPlayers(lobby.Players.Count);
        GameManager.SetTurnNumber(Int32.Parse(lobby.Data["turnNum"].Value));
        GameManager.SetTurnPlayer((GameManager.GetTurnNumber()-1) % GameManager.GetNumPlayers());
        GameManager.UseLobby(true);
        PlayerManager.InitPlayers();

        int i = 0;
        lobby.Players
            .Select(player => player.Data["CompanyName"].Value)
            .ToList()
            .ForEach(name => {
                PlayerManager.GetPlayer(i).SetName(name);
                i++;
            });

        GameDAO dao = new GameDAO();
        dao.Save();
    }

    public static Unity.Services.Lobbies.Models.Player GetPlayer() {
        return new Unity.Services.Lobbies.Models.Player {
            Data = new Dictionary<string, PlayerDataObject>() {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)},
                {"CompanyName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, companyName)},
                {"Save", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "")}
            }
        };
    }

    /// <summary>
    /// Updates Player Data at key with the PlayerDataObject, value
    /// </summary>
    /// <param name="key">The key to the Data dictionary</param>
    /// <param name="value">the new value for the key in the dictionary</param>
    /// <returns>Task</returns>
    public async Task UpdatePlayerData(string id, string key, PlayerDataObject value) {
            try {
                UpdatePlayerOptions updatePlayerOptions = new UpdatePlayerOptions {
                    Data = new Dictionary<string, PlayerDataObject> {
                        {key, value}
                    }
                };

                lobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, id, updatePlayerOptions);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
    }

    public static void ListPlayers() {
        lobby.Players.ForEach(p => {
            Debug.Log(((lobby.HostId == p.Id) ? "H: " : ""));
            BetweenManager.WriteToConsole("| " + ((lobby.HostId == p.Id) ? "H: " : "") + p.Id);
        });
    }

    /// <summary>
    /// Updateas Lobby Data at key with the DataObject, value
    /// </summary>
    /// <param name="key">The key to the Data Dictionary</param>
    /// <param name="value">The new value for the key in the dictionary</param>
    /// <returns>Task</returns>
    public static async Task UpdateLobbyData(string key, DataObject value) {
        try {
            UpdateLobbyOptions updateLobbyOptions = new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    {key, value}
                }
            };

            lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, updateLobbyOptions);
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async static Task GetCurrentLobby() {
        try {
            lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async static void PullLobbyUpdate() {
            LoadManager.SetLoading(true);
            try {
                if (lobby.HostId == AuthenticationService.Instance.PlayerId) {
                    // Pull each save file
                    await PullFileFromLobby("/attacksave.json", "AttackSave");
                    System.Threading.Thread.Sleep(1000);
                    Debug.Log("Attack Pull");

                    await PullFileFromLobby("/datacenter.json", "DataCenterSave");
                    System.Threading.Thread.Sleep(1000);
                    Debug.Log("Data Center Pull");

                    await PullFileFromLobby("/gamesave.json", "GameSave");
                    System.Threading.Thread.Sleep(1000);
                    Debug.Log("Game Pull");

                    await PullFileFromLobby("/malwaresave.json", "MalwareSave");
                    System.Threading.Thread.Sleep(1000);
                    Debug.Log("Malware Pull");

                    await PullFileFromLobby("/notificationsave.json", "NotificationSave");
                    System.Threading.Thread.Sleep(1000);
                    Debug.Log("Notification Pull");

                    await PullFileFromLobby("/playersave.json", "PlayerSave");
                    System.Threading.Thread.Sleep(1000);
                    Debug.Log("Player Pull");

                    GameManager.SetTurnNumber(Int32.Parse(lobby.Data["turnNum"].Value));

                    if (SceneManager.GetActiveScene().name == "BetweenScene") BetweenManager.ForceHeartbeat();
                } else {
                    await GetCurrentLobby();
                    Debug.Log("This user is not a host.");
                }
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
            LoadManager.SetLoading(false);
    }

    /// <summary>
    /// Pulls a file from the current lobby and saves it locally
    /// </summary>
    /// <param name="fileName">The name of the local file to save the file to</param>
    /// <param name="key">The key for the lobby dictionary</param>
    /// <returns>Task</returns>
    public async static Task PullFileFromLobby(string fileName, string key) {
        await GetCurrentLobby();

        string compressedJson = lobby.Data[key].Value;
        //byte[] compressedData = Encoding.UTF8.GetBytes(compressedJson);
        //byte[] jsonData = Decompress(compressedData);
        //string json = Encoding.UTF8.GetString(jsonData);

        if (compressedJson.Length > 0)
            File.WriteAllText(Application.persistentDataPath + fileName, compressedJson);
    }

    public async static void PushLobbyUpdate() {
        LoadManager.SetLoading(true);
        try {
            await PushFileToLobby("/attacksave.json", "AttackSave");
            System.Threading.Thread.Sleep(1000);
            Debug.Log("Attack Save");

            await PushFileToLobby("/datacenter.json", "DataCenterSave");
            System.Threading.Thread.Sleep(1000);
            Debug.Log("Data Center Save");

            await PushFileToLobby("/gamesave.json", "GameSave");
            System.Threading.Thread.Sleep(1000);
            Debug.Log("Game Save");

            await PushFileToLobby("/malwaresave.json", "MalwareSave");
            System.Threading.Thread.Sleep(1000);
            Debug.Log("Malware Save");

            await PushFileToLobby("/notificationsave.json", "NotificationSave");
            System.Threading.Thread.Sleep(1000);
            Debug.Log("Notification Save");

            await PushFileToLobby("/playersave.json", "PlayerSave");
            System.Threading.Thread.Sleep(1000);
            Debug.Log("Player Save");

            await UpdateLobbyData("turnNum", new DataObject(DataObject.VisibilityOptions.Member, (Int32.Parse(lobby.Data["turnNum"].Value)+1).ToString()));
            Debug.Log("Turn Save");
            System.Threading.Thread.Sleep(500);

            ChangeLobbyHost();
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
        LoadManager.SetLoading(false);
    }

    /// <summary>
    /// Pushes a local file to the current lobby
    /// </summary>
    /// <param name="fileName">the name of the local file being pushed</param>
    /// <param name="key">the key for the lobby dictionary.</param>
    /// <returns>Task</returns>
    public async static Task PushFileToLobby(string fileName, string key) {
        if (File.Exists(Application.persistentDataPath + fileName)) {
            string json = File.ReadAllText(Application.persistentDataPath + fileName);
            //byte[] jsonData = Encoding.UTF8.GetBytes(json);
            //byte[] compressedData = Compress(jsonData);
            //string compressedJson = Encoding.UTF8.GetString(compressedData);

            await UpdateLobbyData(key, new DataObject(DataObject.VisibilityOptions.Member, json));
        }
    }

    public static async void ChangeLobbyHost() {
        int idx = lobby.Players
            .IndexOf(lobby.Players
                .Where(p => p.Id == AuthenticationService.Instance.PlayerId)
                .ToList()
                .Single());
        
        string host = lobby.Players[(idx+1) % lobby.Players.Count].Id;
        
        UpdateLobbyOptions updateLobbyOptions = new UpdateLobbyOptions {
            HostId = host
        };

        lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, updateLobbyOptions);
    }

    /// <summary>
    /// Compresses an array of bytes
    /// </summary>
    /// <param name="bytes">array of bytes</param>
    /// <returns>A more compressed array of bytes</returns>
    private static byte[] Compress(byte[] bytes) {
        using (var memoryStream = new MemoryStream()) { 
            using (var gzipStream = new GZipStream(memoryStream, System.IO.Compression.CompressionLevel.Optimal)) {
                gzipStream.Write(bytes, 0, bytes.Length);
            }
            return memoryStream.ToArray();
        }
    }

    /// <summary>
    /// Decompresses an array of bytes
    /// </summary>
    /// <param name="bytes">compressed array of bytes</param>
    /// <returns>Decompressed array of bytes</returns>
    private static byte[] Decompress(byte[] bytes) {
        using (var memoryStream = new MemoryStream(bytes)) {
            using (var outputStream = new MemoryStream()) {
                using (var decompressStream = new GZipStream(memoryStream, System.IO.Compression.CompressionMode.Decompress)) {
                    decompressStream.CopyTo(outputStream);
                }
                return outputStream.ToArray();
            }
        }
    }

    public static int GetPlayerId() {
        return playerId;
    }

    public static void SetPlayerId(int id) {
        playerId = id;
    }

    public static bool GetHostingStatus() {
        return lobby.HostId == AuthenticationService.Instance.PlayerId;
    }
}
