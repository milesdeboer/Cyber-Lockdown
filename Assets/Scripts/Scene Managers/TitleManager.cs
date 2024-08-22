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

        StartCoroutine(LoadSceneDelay("NewGameScene"));
    }

    public void LoadGame() {
        StartCoroutine(LoadSceneDelay("BetweenScene"));
    }

    public void LobbyPlay() {
        StartCoroutine(LoadSceneDelay("SearchLobbies"));
    }

    public void Exit() {
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
}
