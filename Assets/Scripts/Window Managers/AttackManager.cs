using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class AttackManager : MonoBehaviour, ISavable
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
    private GameObject[] selectionButtons;

    [SerializeField]
    private Color completedColor;
    [SerializeField]
    private Color workingColor;
    [SerializeField]
    private Color workedColor;
    [SerializeField]
    private Color emptyColor;

    [SerializeField]
    private GameObject dataCenterSubWindow;

    [SerializeField]
    private GameObject dataCenterButton;

    [SerializeField]
    private GameObject resourceDisplay;
    [SerializeField]
    private GameObject requirementDisplay;

    [SerializeField]
    private GameObject locks;

    [SerializeField]
    private GameObject deliveryGroup;
    [SerializeField]
    private GameObject objectiveGroup;
    [SerializeField]
    private GameObject malwareGroup;
    [SerializeField]
    private GameObject dataCenterGroup;


    private Dictionary<int, Attack> attacks;

    private List<GameObject> dataCenters;

    private int activeAttack = 0;
    
    private bool initialized = false;

    public void Start() {
        if (attacks == null) Load();
        //InitDataCenters();
        InitMap();
        //AssignListeners();
        resourceDisplay.GetComponent<TextMeshProUGUI>().SetText("0");
    }

    /**
     *  Initializes the Attack Objects 
     */
    public void InitAttacks_() {
        attacks = new Dictionary<int, Attack>();
        for(int i = 1; i <= GameManager.GetNumPlayers(); i++) {
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
        activeAttack = (GameManager.GetTurnPlayer()+1) * 100 + i + 1;

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
        attacks[activeAttack].SetMalware((GameManager.GetTurnPlayer() + 1) * 100 + i);
    }

    /**
     *  Sets the obejctive settings of the current attack.
     *  @param {string} objective - The name of the target objective.
     */
    public void ObjectiveClick(string objective) {
        attacks[activeAttack].SetObjective(objective);
        attacks[activeAttack].UpdateRequirement();
        requirementDisplay.GetComponent<TextMeshProUGUI>().SetText(attacks[activeAttack].GetWorkRequirement().ToString());
    }

    /**
     *  Sets the delivery settings of the current attack.
     *  @param {string} delivery - The name of teh delivery method.
     */
    public void DeliveryClick(string delivery) {
        attacks[activeAttack].SetDelivery(delivery);
        attacks[activeAttack].UpdateRequirement();
        requirementDisplay.GetComponent<TextMeshProUGUI>().SetText(attacks[activeAttack].GetWorkRequirement().ToString());
    
    }

    /// <summary>
    /// Instantiates the data center buttons.
    /// </summary>
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
        for (int i = 0; i < GameManager.DATA_CENTERS_PER_PLAYER * GameManager.GetNumPlayers(); i++) {
            // Calculate coordinates for the current button on the circle.
            Vector2 coords = new Vector2();
            double theta = (-2f * Math.PI * i / (GameManager.DATA_CENTERS_PER_PLAYER * GameManager.GetNumPlayers())) + angleOffset;
            coords.x = rx * (float) Math.Cos(theta) + cx;
            coords.y = ry * (float) Math.Sin(theta) + cy;
            
            // Instantiate GameObject
            GameObject dataCenter = Instantiate(dataCenterButton, coords, Quaternion.identity);
            dataCenter.name = dataCenterManager.GetDataCenter(i).GetId().ToString();
            dataCenter.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText((dataCenterManager.GetDataCenter(i).GetOwner()+1).ToString());
            dataCenter.name = i.ToString();

            // Set orientation of the button
            dataCenter.transform.SetParent(dataCenterSubWindow.transform, false);
            dataCenter.GetComponent<RectTransform>().localPosition.Set(coords.x, coords.y, 0);

            // Add to list of buttons
            dataCenters.Add(dataCenter);
        }
        InitMap();
        dataCenterSubWindow.GetComponent<RadioButton>().Start();
    }

    private void InitMap() {
        GameObject[] dataCenters = dataCenterManager.GetDataCenterButtons().ToArray();
        Debug.Log("Data Centers: " + dataCenters.Length);
        for(int i = 0; i < dataCenters.Length; i++) {
            Vector3 pos = dataCenters[i].GetComponent<RectTransform>().localPosition;
            Vector2 scale = new Vector2(0.4545454545f, 0.5543478261f);
            Vector3 coords = new Vector3(pos.x * scale.x, pos.y * scale.y, 0f);

            GameObject dataCenter = Instantiate(dataCenterButton, coords, Quaternion.identity);
            dataCenter.name = dataCenterManager.GetDataCenter(i).GetId().ToString();
            dataCenter.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText((dataCenterManager.GetDataCenter(i).GetOwner()+1).ToString());
            dataCenter.transform.SetParent(dataCenterSubWindow.transform, false);

            int x = i;
            // Add the DataCenterClick listener
            dataCenter.GetComponent<Button>().onClick.AddListener(delegate {
                DataCenterClick(x);
            });
        }
    }

    /// <summary>
    /// Adds onClick event listeners for each data center button
    /// </summary>
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

    /// <summary>
    /// onClick event listener for data center button to change target data center.
    /// </summary>
    /// <param name="i">The index of the data center button.</param>
    public void DataCenterClick(int i) {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("DataCenterButton");
        /*buttons
            .Where(b => {
                b.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                return b.name == i.ToString();
            })
            .ToList()
            .ForEach(b => b.GetComponent<Image>().color = gameManager.selectionColor);*/
        attacks[activeAttack].SetTarget(dataCenterManager.GetDataCenters()[i].GetId());
        Debug.Log("Setting Target of Attack " + activeAttack + " to " + i + ".");
    }

    /// <summary>
    /// onClick event listener for the buttons controlling the allocated resources for production
    /// </summary>
    /// <param name="change">The amount of resources that is being added to the allocated resources usually [-1, 1]</param>
    public void ResourceClick(int change) {
        Player player = PlayerManager.GetPlayer(GameManager.GetTurnPlayer());
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

    /// <summary>
    /// onClick listener for the delete button for an attack which clears all information stored in the attack
    /// </summary>
    /// <param name="i">The index of the attack</param>
    public void Delete(int i) {
        Attack a = attacks[100 * (GameManager.GetTurnPlayer() + 1) + i + 1];
        a.Reset();
        a.SetWorkResources(0);
        a.SetWorkRate(0);
        UpdateStatusColors();
    }

    /// <summary>
    /// Resets the display elements of the attack customization window
    /// </summary>
    private void Reset() {
        // Update Resource Display
        resourceDisplay.GetComponent<TextMeshProUGUI>().SetText(attacks[activeAttack].GetWorkRate().ToString());

        Attack attack = attacks[activeAttack];

        // Update Radio Buttons
        GameObject[] delivery = GameObject.FindGameObjectsWithTag("DeliveryButton").ToList().Where(d => d.name == attack.GetDelivery()).ToArray();
        deliveryGroup.GetComponent<RadioButton>().Start();
        deliveryGroup.GetComponent<RadioButton>().OnClick((delivery.Length > 0) ? delivery[0] : null);

        GameObject[] objective = GameObject.FindGameObjectsWithTag("ObjectiveButton").ToList().Where(o => o.name == attack.GetObjective()).ToArray();
        objectiveGroup.GetComponent<RadioButton>().Start();
        objectiveGroup.GetComponent<RadioButton>().OnClick((objective.Length > 0) ? objective[0] : null);
        

        GameObject[] malware = GameObject.FindGameObjectsWithTag("MalwareButton").ToList().Where(m => m.name == (attack.GetMalware() % 100).ToString()).ToArray();
        malwareGroup.GetComponent<RadioButton>().Start();
        malwareGroup.GetComponent<RadioButton>().OnClick((malware.Length > 0) ? malware[0] : null);

        GameObject[] dataCenter = GameObject.FindGameObjectsWithTag("DataCenterButton").ToList().Where(dc => dc.name == attack.GetTarget().ToString()).ToArray();
        dataCenterGroup.GetComponent<RadioButton>().Start();
        dataCenterGroup.GetComponent<RadioButton>().OnClick((dataCenter.Length > 0) ? dataCenter[0] : null);

        // Update Lock Display
        if (attacks[activeAttack].GetWorkResources() > 0) locks.SetActive(true);
        else locks.SetActive(false);
    }

    public void Work() {
        attacks
            .Where(a => a.Value.GetOwner() == GameManager.GetTurnPlayer())
            .Select(kvp => kvp.Value)
            .ToList()
            .ForEach(a => {
                a.SetWorkResources(a.GetWorkResources() + a.GetWorkRate());
                if (a.IsComplete() && !dataCenterManager.GetDataCenter(a.GetTarget()).GetAttacks().Contains(a.GetId())) {
                    conflictManager.Process(a, dataCenterManager.GetDataCenter(a.GetTarget()));
                    Player p = PlayerManager.GetPlayer(a.GetOwner());
                    p.SetAvailableResources(p.GetAvailableResources() + a.GetWorkRate());
                    a.SetWorkRate(0);
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
        UpdateStatusColors();
    }

    /// <summary>
    /// Checks the status of each attack (completed, uncompleted, etc.) and assigns the button associated with that malware to a specific color
    /// </summary>
    public void UpdateStatusColors() {
        for (int i = 0; i < selectionButtons.Length; i++) {
            Color statusColor = (attacks[100 * (GameManager.GetTurnPlayer()+1) + i + 1].IsComplete()) ? completedColor : 
                (attacks[100 * (GameManager.GetTurnPlayer()+1) + i + 1].GetWorkRate() > 0) ? workingColor : 
                (attacks[100 * (GameManager.GetTurnPlayer()+1) + i + 1].GetWorkResources() > 0) ? workedColor :
                emptyColor;
            selectionButtons[i].GetComponent<Image>().color = statusColor;
        }
    }

    public void Save() {
        AttackDAO dao = new AttackDAO();
        dao.Save(this);
    }

    public void Load() {
        AttackDAO dao = new AttackDAO();
        if (!dao.Load(this)) InitAttacks_();
        UpdateStatusColors();
    }
}
