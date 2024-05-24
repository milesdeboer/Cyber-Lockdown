using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskbarController : MonoBehaviour
{
    public void OnClick(GameObject targetWindow) {
        Debug.Log("Switching Windows");
        GameObject[] windows = GameObject.FindGameObjectsWithTag("Window");
        foreach(GameObject window in windows) {
            window.SetActive(false);
        }
        targetWindow.SetActive(true);
    }
}
