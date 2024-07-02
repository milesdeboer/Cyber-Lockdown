using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NewGameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private GameObject playerEntry;

    private List<GameObject> players;
    

    void Start() {
        players = new List<GameObject>();
        AddPlayer();
        AddPlayer();
    }

    public void AddPlayer() {
        if (players.Count < 6) {
            int y = 300 - 120 * players.Count; // [-300, 300]
            GameObject player = Instantiate(playerEntry, new Vector2(-500, y), Quaternion.identity);
            player.transform.SetParent(canvas.transform, false);
            player.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(delegate {

                RemovePlayer(player);
            });
            players.Add(player);
        }
    }

    public void RemovePlayer(GameObject player) {
        if (players.Count > 0) {
            players.Remove(player);
            GameObject.Destroy(player);
        }
        UpdatePositions();
    }

    public void UpdatePositions() {
        for (int i = 0; i < players.Count; i++) {
            int y = 300 - 120 * i;
            players[i].GetComponent<RectTransform>().localPosition = new Vector2(-500, y);
        }
    }

    public void StartGame() {
        GameManager.SetNumPlayers(players.Count);
        GameManager.SetTurnPlayer(0);
        GameManager.SetTurnNumber(1);
        PlayerManager.InitPlayers();

        foreach(KeyValuePair<int, Player> kvp in PlayerManager.GetPlayers()) {
            string name = players[kvp.Key].transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text;
            Debug.Log(name);
            kvp.Value.SetName(name);
        }

        SceneManager.LoadScene("BetweenScene");
    }
}
