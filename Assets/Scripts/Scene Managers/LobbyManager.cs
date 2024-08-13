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
    private string playerName;
    private string companyName;
    private string lobbyCode;

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
                    SelectLobby(lobby.LobbyCode);
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

    public void SelectLobby(string code) {
        lobbyCode = code;
    }

    /// <summary>
    /// Joins a Lobby by the code in the lobby code text box.
    /// </summary>
    public async void JoinLobbyByCode() {
        await Task.Run(() => JoinLobby(lobbyCodeTextBox.text));
    }

    /// <summary>
    /// Joins a Lobby by the selected lobby button in the list of lobby entries.
    /// </summary>
    public async void JoinLobbyBySelect() {
        await Task.Run(() => JoinLobby(lobbyCode));
    }

    /// <summary>
    /// Joins a lobby
    /// </summary>
    /// <param name="code">the code of the lobby joined</param>
    private async void JoinLobby(string code) {
        try {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions {
                Player = GetPlayer()
            };

            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code, joinLobbyByCodeOptions);

        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    private Unity.Services.Lobbies.Models.Player GetPlayer() {
        return null;
    }
}
