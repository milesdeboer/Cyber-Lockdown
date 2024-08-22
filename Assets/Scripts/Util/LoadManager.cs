using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    public static bool loading = false;
    private float refresh = 0f;

    public void Update() {
        if(refresh < 0f) {
            float refreshLimit = 1f;
            refresh = refreshLimit;
            SetLoading(loading);
        } else {
            refresh -= Time.deltaTime;
        }
    }
    public static void SetLoading(bool loading_) {
        GameObject[] loadingImages = GameObject.FindGameObjectsWithTag("loading");
        if (loadingImages.Length > 0) {
            loadingImages[0].transform.GetChild(0).gameObject.SetActive(loading_);
            loading = loading_;
        } else {
            Debug.Log("NO Loading object found :(");
        }
    }
}
