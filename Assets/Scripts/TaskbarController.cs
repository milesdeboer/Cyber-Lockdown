using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskbarController : MonoBehaviour
{
    /**
     *  onClick listener for switching from window to window.
     *  @param {GameObject} targetWindow - The window that is being switched to.
     */
    public void OnClick(GameObject targetWindow) {
        Debug.Log("Switching Windows");

        // Set all windows to inactive.
        GameObject[] windows = GameObject.FindGameObjectsWithTag("Window");
        foreach(GameObject window in windows) {
            window.SetActive(false);
        }

        // Set target window to active.
        targetWindow.SetActive(true);
    }
}
