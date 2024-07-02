using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

    public void NewGame() {
        (new AttackDAO()).Erase();
        (new DataCenterDAO()).Erase();
        (new GameDAO()).Erase();
        (new MalwareDAO()).Erase();
        (new NotificationDAO()).Erase();
        (new PlayerDAO()).Erase();

        SceneManager.LoadScene("NewGameScene");
    }

    public void LoadGame() {
        SceneManager.LoadScene("BetweenScene");
    }

    public void Exit() {
        Debug.Log("Exiting Game");
        Application.Quit();
    }
}
