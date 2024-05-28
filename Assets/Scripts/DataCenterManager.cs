using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataCenterManager : MonoBehaviour
{
    [SerializeField] 
    private GameManager gameManager;

    [SerializeField]
    private GameObject selectionWindow;
    [SerializeField]
    private GameObject customizationWindow;

    [SerializeField]
    private GameObject dataCenterButton;

    private List<DataCenter> dataCenters;
    private List<GameObject> dataCenterButtons;

    private int currentDataCenter = 0;

    public void Start() {
        InitDataCenters();
        InitButtons();
        InitSelectionListeners();
    }

    /**
     *  Initializes the Data Center Objects 
     */
    public void InitDataCenters() {
        dataCenters = new List<DataCenter>();
        for (int i = 0; i < GameManager.DATA_CENTERS_PER_PLAYER * gameManager.GetNumPlayers(); i++) {
            dataCenters.Add(new DataCenter(i));
        }
    }

    /**
     *  Instantiates the data center selection buttons.
     */
    public void InitButtons() {
        // Initialize the list of data center buttons
        dataCenterButtons = new List<GameObject>();

        // Frame dimensions
        float xMin = 6.5f;
        float xMax = 100.5f;
        float yMin = 36f;
        float yMax = 126f;

        // Center of Frame
        float cx = (xMax - xMin) / 2f + xMin;
        float cy = (yMax - yMin) / 2f + yMin;

        // Radius of Circle
        float rx = 0.4f * (xMax - xMin);
        float ry = 0.4f * (yMax - yMin);

        // Angle Offset
        float angleOffset = (float) Math.PI / 4f;

        // Iterate through the number of data centers there should be based on number of players and data centers per player.
        for (int i = 0; i < GameManager.DATA_CENTERS_PER_PLAYER * gameManager.GetNumPlayers(); i++) {
            // Calculate coordinates for the current button on the circle.
            Vector2 coords = new Vector2();
            double theta = (-2f * Math.PI * i / (GameManager.DATA_CENTERS_PER_PLAYER * gameManager.GetNumPlayers())) + angleOffset;
            coords.x = rx * (float) Math.Cos(theta) + cx;
            coords.y = ry * (float) Math.Sin(theta) + cy;

            // Instantiate GameObject.
            GameObject dataCenter = Instantiate(dataCenterButton, coords, Quaternion.identity);

            // Set orientation and size of the button.
            dataCenter.transform.SetParent(selectionWindow.transform, false);
            dataCenter.GetComponent<RectTransform>().localPosition.Set(coords.x, coords.y, 0);
            dataCenter.GetComponent<RectTransform>().sizeDelta = new Vector2(15f, 20f);

            // Add to list of buttons.
            dataCenterButtons.Add(dataCenter);
        }
    }

    /**
     *  Adds onClick event listeners for each data center button
     */
    public void InitSelectionListeners() {
        // Iterate through data center buttons
        for (int i = 0; i < dataCenterButtons.Count; i++) {
            // Avoid capturing error
            int x = i;

            // Add the DataCenterClick listener
            dataCenterButtons[x].GetComponent<Button>().onClick.AddListener(delegate {
                SelectionClick(x);
            });

        }
    }

    /**
     *  onClick event listener for data center button to change target data center.
     *  @param {int} i - The index of the data center button.
     */
    public void SelectionClick(int i) {
        customizationWindow.SetActive(true);
        currentDataCenter = i;
        selectionWindow.SetActive(false);
        Debug.Log("Switching to Data Center " + i);
    }

    /**
     *  onClick event listener for attribute buttons, increasing the value of the attribute by one.
     *  @param {string} attribute - The name of the attribute being changed.
     */
    public void AttributeClick(string attribute) {
        switch(attribute) {
            case "emailFilter":
                // Increase the Email Filtering level of the current data center by one
                dataCenters[currentDataCenter].SetEmailFilter(dataCenters[currentDataCenter].GetEmailFilter()+1);
                break;
            case "dlp":
                // Increase the Data Loss Prevention level of the current data center by one
                dataCenters[currentDataCenter].SetDLP(dataCenters[currentDataCenter].GetDLP()+1);
                break;
            case "hiddenStructure":
                // Increase the Hidden Structure level of the current data center by one
                dataCenters[currentDataCenter].SetHiddenStructure(dataCenters[currentDataCenter].GetHiddenStructure()+1);
                break;
            case "encryption":
                // Increase the Encryption level of the current data center by one
                dataCenters[currentDataCenter].SetEncryption(dataCenters[currentDataCenter].GetEncryption()+1);
                break;
            case "ids":
                // Increase the Intrusion Detection System level of the current data center by one
                dataCenters[currentDataCenter].SetIDS(dataCenters[currentDataCenter].GetIDS()+1);
                break;
            case "ips":
                // Increase the Intrusion Prevention System level of the current data center by one
                dataCenters[currentDataCenter].SetIPS(dataCenters[currentDataCenter].GetIPS()+1);
                break;
            default:
                // Set all attributes of current data center to -1 if incorrect attribute passed
                dataCenters[currentDataCenter].SetEmailFilter(-1);
                dataCenters[currentDataCenter].SetDLP(-1);
                dataCenters[currentDataCenter].SetHiddenStructure(-1);
                dataCenters[currentDataCenter].SetEncryption(-1);
                dataCenters[currentDataCenter].SetIDS(-1);
                dataCenters[currentDataCenter].SetIPS(-1);
                break;
        }
        Debug.Log("Increased " + attribute + " of data center " + currentDataCenter + " by one.");
    }

    public void FirewallChange(float value) {
        dataCenters[currentDataCenter].SetFirewall((int) (value * GameManager.VALUE_SCALE));
        Debug.Log("Updated Firewall Strength to " + ((int) (value * GameManager.VALUE_SCALE)) + ".");
    }

    /**
     *  onClick event listener to go back to data center selection window.
     */
    public void Back() {
        selectionWindow.SetActive(true);
        currentDataCenter = 0;
        customizationWindow.SetActive(false);
    }

    /**
     *  Returns the data center at a given index.
     *  @param {int} i - The index of the data center.
     *  @returns {DataCenter} - The data center at the given index.
     */
    public DataCenter GetDataCenter(int i) {
        return dataCenters[i];
    }
}
