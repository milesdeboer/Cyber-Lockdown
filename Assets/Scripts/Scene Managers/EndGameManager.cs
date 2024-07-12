using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    private static string winner = "<player-name>";

    [SerializeField]
    private GameObject winText;
    
    void Start()
    {
        winText.GetComponent<TextMeshProUGUI>().SetText("The Winner is " + winner);
    }

    public void OnClick() {
        SceneManager.LoadScene("TitleScene");
    }

    public static string GetWinner() {
        return winner;
    }

    public static void SetWinner(string newWinner) {
        winner = newWinner;
    }
}
