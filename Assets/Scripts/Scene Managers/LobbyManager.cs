using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using TMPro;

using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI lobbyCodeTextBox;

    [SerializeField]
    private GameObject lobbyEntry;
    [SerializeField]
    private Transform lobbyEntryContainer;

    private Lobby hostLobby;
    private float heartbeatTimer;
    private static string playerName;
    private static string companyName;
    private string lobbyId;

    private static int playerId;

    private async void Start()
    {   
        try {
            playerName = "Player " + UnityEngine.Random.Range(0, 100000).ToString("D5");
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () => {
                Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Unique Player Id: " + playerName);
        } catch (AuthenticationException e) {
            Debug.Log(e);
        }

        ViewLobbies();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// onClick listener for the create lobby button. Loads the view lobby scene and makes the user a host.
    /// </summary>
    public void CreateLobbyClick() {
        SceneManager.LoadScene("ViewLobby");
        LobbyViewer.SetHosting(true);
    }

    /// <summary>
    /// Refreshes the list of lobbies viewed.
    /// </summary>
    public async void ViewLobbies() {
        try {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            Debug.Log("Lobbies Found: " + queryResponse.Results.Count);

            int i = 0;
            foreach(Lobby lobby in queryResponse.Results) {
                if (i > 5) break;

                Vector3 position = new Vector3(0f, 360f - (float) i * 180f, 0f);
                GameObject entry = Instantiate(lobbyEntry, position, Quaternion.identity);
                entry.GetComponent<Button>().onClick.AddListener(delegate {
                    SelectLobby(lobby.Id);
                });
                entry.transform.SetParent(lobbyEntryContainer, false);
                entry.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText(lobby.Players.Count + "/" + lobby.MaxPlayers);
                entry.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().SetText("Lobby: " + lobby.Name);

                i++;
            }
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public void SelectLobby(string id) {
        Debug.Log("Changing selected lobby to " + id);
        lobbyId = id;
    }

    /// <summary>
    /// Joins a Lobby by the code in the lobby code text box.
    /// </summary>
    public async void JoinLobbyByCode() {
        try {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions {
                Player = GetPlayer()
            };

            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCodeTextBox.text, joinLobbyByCodeOptions);

            if (joinedLobby != null)
                SceneManager.LoadScene("ViewLobby");
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Joins a Lobby by the selected lobby button in the list of lobby entries.
    /// </summary>
    public async void JoinLobbyBySelect() {
        try {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions {
                Player = GetPlayer()
            };

            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
            LobbyViewer.SetLobby(joinedLobby);

            if (joinedLobby != null)
                SceneManager.LoadScene("ViewLobby");
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Joins a lobby and switches to view lobby scene
    /// </summary>
    /// <param name="code">the code of the lobby joined</param>
    private async void JoinLobby(string code) {
        try {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions {
                Player = GetPlayer()
            };

            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code, joinLobbyByCodeOptions);
            Debug.Log("right after");
            if (joinedLobby != null) {
                playerId = joinedLobby.Players.Count;
                LobbyViewer.SetPlayerId(joinedLobby.Players.Count);
                Debug.Log("Player Count: " + joinedLobby.Players.Count + ", " + LobbyViewer.GetPlayerId());
                SceneManager.LoadScene("ViewLobby");
            }
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
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

    public static string GetPlayerName() {
        return playerName;
    }

    public static int GetPlayerId() {
        return playerId;
    }

    public static void SetPlayerId(int id) {
        playerId = id;
    }
}
