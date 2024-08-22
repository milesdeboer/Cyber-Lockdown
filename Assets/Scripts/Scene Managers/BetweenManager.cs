using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BetweenManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerNum;
    [SerializeField]
    private GameObject turnNum;

    [SerializeField]
    private GameObject turnPlayerDisplay;
    [SerializeField]
    private GameObject notTurnPlayerDisplay;

    [SerializeField]
    private GameObject loadingImage;

    private static float heartbeat = 0f;

    public void Start() {
        GetTurnData();
        if (GameManager.IsUsingLobby()) {
            playerNum.GetComponent<TextMeshProUGUI>().SetText("HPlayer " + ((Int32.Parse(LobbyViewer.GetLobby().Data["turnNum"].Value)-1) % LobbyViewer.GetLobby().Players.Count + 1));
            turnNum.GetComponent<TextMeshProUGUI>().SetText("HTurn Number " + LobbyViewer.GetLobby().Data["turnNum"].Value);
        } else {
            playerNum.GetComponent<TextMeshProUGUI>().SetText("Player " + (GameManager.GetTurnPlayer()+1).ToString());
            turnNum.GetComponent<TextMeshProUGUI>().SetText("Turn Number " + GameManager.GetTurnNumber().ToString());
        }
    }

    public void Update() {
        HandleHeartbeat();
    }

    public async void HandleHeartbeat() {
        heartbeat -= Time.deltaTime;
        if (heartbeat < 0f) {
            float heartbeatLimit = 5f;
            heartbeat = heartbeatLimit;
            
            if (GameManager.IsUsingLobby()) {
                await LobbyViewer.GetCurrentLobby();
                LobbyViewer.ListPlayers();

                if (LobbyViewer.GetHostingStatus()) {
                    turnPlayerDisplay.SetActive(true);
                    notTurnPlayerDisplay.SetActive(false);
                } else {
                    turnPlayerDisplay.SetActive(false);
                    notTurnPlayerDisplay.SetActive(true);
                }
            }
        }
    }

    public static void ForceHeartbeat() {
        heartbeat = -1f;
    }

    public void StartTurn() {
        SceneManager.LoadScene("PlayerScene");
    }

    public void GetTurnData() {
        GameDAO dao = new GameDAO();
        dao.Load();
    }


    public static void WriteToConsole(string str) {
        try {
            GameObject console = GameObject.FindGameObjectsWithTag("Console")[0];
            console.GetComponent<TextMeshProUGUI>().SetText(console.GetComponent<TextMeshProUGUI>().text + " " + str);
        } catch (Exception e) {
            Debug.Log(e);
        }
    }
}
