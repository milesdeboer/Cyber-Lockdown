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

    public void Start() {
        GetTurnData();
        playerNum.GetComponent<TextMeshProUGUI>().SetText("Player " + (GameManager.GetTurnPlayer()+1).ToString());
        turnNum.GetComponent<TextMeshProUGUI>().SetText("Turn Number " + GameManager.GetTurnNumber().ToString());
    }

    public void StartTurn() {
        SceneManager.LoadScene("PlayerScene");
    }

    public void GetTurnData() {
        GameDAO dao = new GameDAO();
        dao.Load();
    }
}
