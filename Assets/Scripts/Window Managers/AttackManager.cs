using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class AttackManager : MonoBehaviour
{

    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private ConflictManager conflictManager;

    [SerializeField]
    private MalwareController malwareController;

    [SerializeField]
    private DataCenterManager dataCenterManager;

    [SerializeField]
    private GameObject selectionWindow;
    [SerializeField]
    private GameObject customizationWindow;

    [SerializeField]
    private GameObject dataCenterSubWindow;

    [SerializeField]
    private GameObject dataCenterButton;

    [SerializeField]
    private GameObject resourceDisplay;

    private Dictionary<int, Attack> attacks;

    private List<GameObject> dataCenters;

    private int activeAttack = 0;
    
    private bool initialized = false;

    public void Start() {
        if (attacks == null) Load();
        InitDataCenters();
        AssignListeners();
        resourceDisplay.GetComponent<TextMeshProUGUI>().SetText("0");
    }

    /**
     *  Initializes the Attack Objects 
     */
    public void InitAttacks_() {
        attacks = new Dictionary<int, Attack>();
        for(int i = 1; i <= gameManager.GetNumPlayers(); i++) {
            for (int j = 1; j <= GameManager.ATTACKS_PER_PLAYER; j++) {
                Attack a = new Attack(i * 100 + j);
                attacks.Add(i * 100 + j, a);
                a.SetOwner(i-1);
            }
        }
        Debug.Log("Attacks Initialized");
        initialized = true;
    }

    public Dictionary<int, Attack> GetAttacks() {
        return attacks;
    }

    public Attack GetAttack(int aid) {
        return attacks[aid];
    }

    public void SetAttacks(Dictionary<int, Attack> attacks) {
        this.attacks = attacks;
    }

    public bool IsInitialized() {
        return initialized;
    }

    /**
     *  Switches from selection window to customization window of attack i
     *  @param {int} i - the index of the button pressed
     */
    public void OnClick(int i) {
        // Set the customization window to active and set active attack
        customizationWindow.SetActive(true);
        activeAttack = (gameManager.GetTurnPlayer()+1) * 100 + i + 1;

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
        attacks[activeAttack].SetMalware((gameManager.GetTurnPlayer() + 1) * 100 + i);
        Debug.Log("Switching Malware of Attack " + (activeAttack+1) + " to " + attacks[activeAttack].GetMalware() + ".");
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
            dataCenter.name = dataCenterManager.GetDataCenter(i).GetId().ToString();
            dataCenter.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText((dataCenterManager.GetDataCenter(i).GetOwner()+1).ToString());

            // Set orientation of the button
            dataCenter.transform.SetParent(dataCenterSubWindow.transform, false);
            dataCenter.GetComponent<RectTransform>().localPosition.Set(coords.x, coords.y, 0);

            //dataCenter.GetComponent<Image>().color = (dataCenterManager.GetDataCenter(i).GetOwner() == -1) ? new Color(1.0f, 1.0f, 1.0f) : (gameManager.GetColors()[dataCenterManager.GetDataCenter(i).GetOwner()]);

            // Add to list of buttons
            dataCenters.Add(dataCenter);
        }
        dataCenterSubWindow.GetComponent<RadioButton>().Start();
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
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("DataCenterButton");
        buttons
            .Where(b => {
                b.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                return b.name == i.ToString();
            })
            .ToList()
            .ForEach(b => b.GetComponent<Image>().color = gameManager.selectionColor);
        attacks[activeAttack].SetTarget(dataCenterManager.GetDataCenters()[i].GetId());
        Debug.Log("Setting Target of Attack " + activeAttack + " to " + i + ".");
    }

    public void ResourceClick(int change) {
        Player player = playerManager.GetPlayer(gameManager.GetTurnPlayer());
        if (!(player.GetAvailableResources() - change > player.GetOverallResources()) &&
            !(player.GetAvailableResources() - change < 0) &&
            !(Int32.Parse(resourceDisplay.GetComponent<TextMeshProUGUI>().text) == 0 && change < 0)) {

            player.SetAvailableResources(player.GetAvailableResources() - change);
            resourceDisplay.GetComponent<TextMeshProUGUI>().SetText((Int32.Parse(resourceDisplay.GetComponent<TextMeshProUGUI>().text) + change).ToString());

            attacks[activeAttack].SetWorkRate(Int32.Parse(resourceDisplay.GetComponent<TextMeshProUGUI>().text));

            Debug.Log("Available: " + player.GetAvailableResources());
        } else {
            Debug.Log("Invalid Resources");
        }
        playerManager.UpdateDisplay();
    }

    private void Reset() {
        resourceDisplay.GetComponent<TextMeshProUGUI>().SetText(attacks[activeAttack].GetWorkRate().ToString());
    }

    public void Work() {
        attacks
            .Where(a => a.Value.GetOwner() == gameManager.GetTurnPlayer())
            .ToList()
            .ForEach(a => {
                a.Value.SetWorkResources(a.Value.GetWorkResources() + a.Value.GetWorkRate());
                if (a.Value.IsComplete()) {
                    conflictManager.Process(a.Value, dataCenterManager.GetDataCenter(a.Value.GetTarget()));
                    Player p = playerManager.GetPlayer(a.Value.GetOwner());
                    p.SetAvailableResources(p.GetAvailableResources() + a.Value.GetWorkRate());
                    a.Value.SetWorkRate(0);
                }
            });
    }

    /**
     *  onClick event listener to go back to attack selection window.
     */
    public void Back() {
        selectionWindow.SetActive(true);
        activeAttack = 0;
        customizationWindow.SetActive(false);
    }

    public void Save() {
        AttackDAO dao = new AttackDAO();
        dao.Save(this, gameManager.GetTurnPlayer());
    }

    public void Load() {
        AttackDAO dao = new AttackDAO();
        if (!dao.Load(this)) InitAttacks_();
    }
}
