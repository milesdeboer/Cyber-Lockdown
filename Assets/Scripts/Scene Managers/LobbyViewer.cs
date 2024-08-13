using System;
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
    private static string playerName;
    private static bool hosting = false;

    private bool isPrivate = false;

    public void Start() {
        if (hosting) {
            lobbyNameObject.GetComponent<TMP_InputField>().interactable = true;
        } else {
            lobbyNameObject.GetComponent<TMP_InputField>().interactable = false;
            lobbyNameObject.transform.GetChild(1).gameObject.SetActive(false);
            lobbyNameObject.GetComponent<TMP_InputField>().text = lobby.Name;
            ViewPlayers(lobby);
        }
    }

    /// <summary>
    /// Refreshes the list of players displayed.
    /// </summary>
    /// <param name="lobby"></param>
    public void ViewPlayers(Lobby lobby) {
        // destory all players
        int i = 0;
        lobby.Players.ForEach(player => {
            Vector3 position = new Vector3((float) (i % 2) * -305f, (float) (i / -2) * 170f + 340f, 0f);
            GameObject entry = Instantiate(playerEntry, position, Quaternion.identity);
            entry.transform.SetParent(playerEntryContainer, false);

            entry.GetComponent<TextMeshProUGUI>().SetText(player.Data["CompanyName"].Value);

            if (player.Data["PlayerName"].Value == playerName) {
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
                Data = new Dictionary<string, DataObject>() {
                    {"dcpp", new DataObject(DataObject.VisibilityOptions.Member, dataCentersPerPlayer)}
                }
            };

            lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            Debug.Log("Successfully Created Lobby");

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
}
