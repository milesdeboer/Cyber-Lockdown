using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AttackManager : MonoBehaviour
{

    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private MalwareController malwareController;

    [SerializeField]
    private GameObject selectionWindow;
    [SerializeField]
    private GameObject customizationWindow;

    [SerializeField]
    private GameObject dataCenterSubWindow;

    [SerializeField]
    private GameObject dataCenterButton;

    private List<Attack> attacks;

    private List<GameObject> dataCenters;

    private int activeAttack = 0;
    
    public void Start() {
        InitAttacks();
        InitDataCenters();
        AssignListeners();

    }

    /**
     *  Initializes the Attack Objects 
     */
    private void InitAttacks() {
        attacks = new List<Attack>();
        for(int i = 0; i < 8; i++) {
            attacks.Add(new Attack(i));
        }
        Debug.Log("Attacks Initialized");
    }

    /**
     *  Switches from selection window to customization window of attack i
     *  @param {int} i - the index of the button pressed
     */
    public void OnClick(int i) {
        // Set the customization window to active and set active attack
        customizationWindow.SetActive(true);
        activeAttack = i;

        Debug.Log("Opening attack " + activeAttack + ".");

        // Reset the configuration of the customization window and set it to that of the active attack
        Reset();

        // Disable the selection window
        selectionWindow.SetActive(false);
    }

    /**
     *  Sets the malware settings of the current attack.
     *  @param {int} i - The index of the button pressed.
     */
    public void MalwareClick(int i) {
        attacks[activeAttack].SetMalware(malwareController.GetMalware(i));
        Debug.Log("Switching Malware of Attack " + (activeAttack+1) + " to " + attacks[activeAttack].GetMalware().GetId() + ".");
    }

    /**
     *  Sets the obejctive settings of the current attack.
     *  @param {string} objective - The name of the target objective.
     */
    public void ObjectiveClick(string objective) {
        attacks[activeAttack].SetObjective(objective);
        Debug.Log("Setting Objective of Attack " + activeAttack + " to " + objective + ".");
    }

    /**
     *  Sets the delivery settings of the current attack.
     *  @param {string} delivery - The name of teh delivery method.
     */
    public void DeliveryClick(string delivery) {
        attacks[activeAttack].SetDelivery(delivery);
        Debug.Log("Setting Delivery of Attack " + activeAttack + " to " + delivery + ".");
    }

    /**
     *  Instantiates the data center buttons.
     */
    private void InitDataCenters() {
        // Initialize the list of data center buttons
        dataCenters = new List<GameObject>();

        // Frame dimensions
        float xMin = 15f;
        float xMax = 105f;
        float yMin = 45f;
        float yMax = 135f;

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
            
            // Instantiate GameObject
            GameObject dataCenter = Instantiate(dataCenterButton, coords, Quaternion.identity);

            // Set orientation of the button
            dataCenter.transform.SetParent(dataCenterSubWindow.transform, false);
            dataCenter.GetComponent<RectTransform>().localPosition.Set(coords.x, coords.y, 0);

            // Add to list of buttons
            dataCenters.Add(dataCenter);
        }
    }

    /**
     *  Adds onClick event listeners for each data center button
     */
    public void AssignListeners() {
        // Iterate through data center buttons
        for(int i = 0; i < dataCenters.Count; i++) {
            // Avoid capturing error
            int x = i;
            // Add the DataCenterClick listener
            dataCenters[i].GetComponent<Button>().onClick.AddListener(delegate {
                DataCenterClick(x);
            });
        }
    }

    /**
     *  onClick event listener for data center button to change target data center.
     *  @param {int} i - The index of the data center button.
     */
    public void DataCenterClick(int i) {
        Debug.Log("attack " + activeAttack + "/" + attacks.Count);
        attacks[activeAttack].SetTarget(i);
        Debug.Log("Setting Target of Attack " + activeAttack + " to " + i + ".");
    }

    private void Reset() {
        ///!!!
    }

    /**
     *  onClick event listener to go back to attack selection window.
     */
    public void Back() {
        selectionWindow.SetActive(true);
        activeAttack = 0;
        customizationWindow.SetActive(false);
    }
}
