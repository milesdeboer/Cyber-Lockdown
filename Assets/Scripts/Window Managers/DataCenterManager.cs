using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DataCenterManager : MonoBehaviour
{
    public static int BASE_DATA_CENTER_MONEY = 20;
    public static int BASE_DATA_CENTER_RESOURCES = 20;

    [SerializeField] 
    private GameManager gameManager;
    [SerializeField]
    private ConflictManager conflictManager;
    [SerializeField]
    private MalwareController malwareManager;
    [SerializeField]
    private AttackManager attackManager;
    [SerializeField]
    private PlayerManager playerManager;

    [SerializeField]
    private GameObject selectionWindow;
    [SerializeField]
    private GameObject customizationWindow;

    [SerializeField]
    private GameObject dataCenterButton;

    [SerializeField]
    private GameObject email;
    [SerializeField]
    private GameObject emailSubWindow;

    [SerializeField]
    private GameObject traffic;
    [SerializeField]
    private GameObject trafficSubWindow;

    private List<DataCenter> dataCenters;
    private List<GameObject> dataCenterButtons;


    private int currentDataCenter = 0;

    public void Start() {
        InitButtons();
        InitSelectionListeners();
        InitEmail();
        InitTraffic();
    }

    /**
     *  Initializes the Data Center Objects 
     */
    public void InitDataCenters() {
        dataCenters = new List<DataCenter>();
        for (int i = 0; i < GameManager.DATA_CENTERS_PER_PLAYER * gameManager.GetNumPlayers(); i++) {
            dataCenters.Add(new DataCenter(i));
            // Temperary
            dataCenters[i].SetOwner(i / GameManager.DATA_CENTERS_PER_PLAYER);
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

            // Set Color of button based on owner
            dataCenter.GetComponent<Image>().color = (dataCenters[i].GetOwner() == -1) ? new Color(1.0f, 1.0f, 1.0f) : (gameManager.GetColors()[dataCenters[i].GetOwner()]);

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
        ResetSetup();
        selectionWindow.SetActive(false);
        Debug.Log("Switching to Data Center " + i);
    }

    public void ResetSetup() {
        Debug.Log("Starting Setup");
        GameObject[] sliders = GameObject.FindGameObjectsWithTag("DataCenterAttribute");
        Slider slider = sliders[0].GetComponent<Slider>();
        if (slider != null) 
            slider.value = (float) dataCenters[currentDataCenter].GetFirewall() / GameManager.VALUE_SCALE;

        InitEmail();
    }

    /**
     *  onClick event listener for attribute buttons, increasing the value of the attribute by one.
     *  @param {string} attribute - The name of the attribute being changed.
     */
    public void AttributeClick(string attribute) {
        switch(attribute) {
            case "emailFiltering":
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
                Debug.Log("Invalid Attribute: " + attribute);
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
     *  Initialize email objects and distributes malware amongst them
     */
    public void InitEmail() {
        // Destroy existing emails
        GameObject[] emails_ = GameObject.FindGameObjectsWithTag("Email");
        foreach(GameObject email in emails_) Destroy(email);

        // Generate random email order
        System.Random random = new System.Random();
        int order = random.Next(0, 6);

        // Instantiate email objects
        GameObject[] emails = {
            Instantiate(email, new Vector2(207f, ((order < 2) ? 275f : (order % 2 == 0) ? 240f : 205f)), Quaternion.identity),
            Instantiate(email, new Vector2(207f, ((order == 2 || order == 3) ? 275f : (order == 0 || order == 5) ? 240f : 205f)), Quaternion.identity),
            Instantiate(email, new Vector2(207f, ((order > 3) ? 275f : (order % 2 == 1) ? 240f : 205f)), Quaternion.identity)
        };
        
        // Place emails under the email section in game scene
        foreach(GameObject emailObject in emails) {
            emailObject.transform.SetParent(emailSubWindow.transform, false);
            emailObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0.95f, 0.3f);
        }

        dataCenters[currentDataCenter].SetEmails(emails);

        // Declare malicious email array
        Attack[] malMail = new Attack[emails.Length];

        if (!attackManager.IsInitialized()) attackManager.Load();

        int i = 0;
        attackManager
            .GetAttacks()
            .Where(attack => dataCenters[currentDataCenter]
                .GetPhishes()
                .Any(a => a == attack.Value.GetId()))
            .Select(a => a.Value)
            .Take(emails.Length)
            .ToList()
            .ForEach(a => {
                emails[i].name = a.GetId().ToString();
                i++;
            });

        InitEmailListeners();
    }

    /**
     *  Initializes the event listeners for the buttons of each email object
     */
    public void InitEmailListeners() {
        // Iterate through all emails
        foreach(GameObject email in dataCenters[currentDataCenter].GetEmails()) {
            GameObject acceptButton = email.transform.GetChild(0).gameObject;
            GameObject declineButton = email.transform.GetChild(1).gameObject;

            // Add onClick listener to accept button
            acceptButton.GetComponent<Button>().onClick.AddListener(delegate {
                EmailClick(email, true);
            });

            // Add onClick listener to decline button
            declineButton.GetComponent<Button>().onClick.AddListener(delegate {
                EmailClick(email, false);
            });
        }
    }

    /**
     *  onClick listener for the email accept and confirm buttons
     *  @param {GameObject} self - The GameObject of the email entry
     *  @param {bool} accepted - boolean determining which button was pressed (accepted or declined)
     */
    public void EmailClick(GameObject self, bool accepted) {
        if (accepted) {
            int aid;
            if (Int32.TryParse(self.name, out aid)) {
                Attack phish = attackManager.GetAttack(aid);
                conflictManager.Infect(phish, dataCenters[currentDataCenter]);
                playerManager.UpdateDisplay();
                dataCenters[currentDataCenter].GetPhishes().Remove(phish.GetId());
            }
        }
        Destroy(self);
    }

    
    public void InitTraffic() {
        int index = 0;

        // Get random order for the traffic entries
        System.Random random = new System.Random();
        int order = random.Next(0, 6);

        // Declare traffic objects
        GameObject[] trafficObjects = {
            Instantiate(traffic, new Vector2(207f, ((order < 2) ? 155f : (order % 2 == 0) ? 120f : 85f)), Quaternion.identity),
            Instantiate(traffic, new Vector2(207f, ((order == 2 || order == 3) ? 155f : (order == 0 || order == 5) ? 120f : 85f)), Quaternion.identity),
            Instantiate(traffic, new Vector2(207f, ((order > 3) ? 155f : (order % 2 == 1) ? 120f : 85f)), Quaternion.identity)
        };

        // Orient each traffic obejct to correct position
        foreach(GameObject trafficObject in trafficObjects) {
            trafficObject.transform.SetParent(trafficSubWindow.transform, false);
            trafficObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0.95f, 0.3f);
        }

        dataCenters[currentDataCenter].SetTraffic(trafficObjects);

        // Declare malicous traffic array
        Attack[] malTraffic = new Attack[trafficObjects.Length];

        if (!attackManager.IsInitialized()) attackManager.Load();

        Attack[] attacks = attackManager
            .GetAttacks()
            .Where(attack => dataCenters[currentDataCenter]
                .GetAttacks()
                .Any(a => a == attack.Value.GetId()))
            .Select(attack => attack.Value)
            .ToArray();

        // Iterate through all attacks at data center
        foreach (Attack attack in attacks) {
            // Get the malware associated with the attack
            Malware malware = malwareManager.GetMalware(attack.GetMalware());
            // !!! - add chance the malware isnt discovered
            // If the malware is adware or botnet, add the attack to malicious traffic array
            if (malware.GetMalwareType() == "adware" || malware.GetMalwareType() == "botnet") {
                malTraffic[index] = attack;
                index++;
                // If more discovered target malware than available traffic, then don't continue
                if (index > trafficObjects.Length) break;
            }
        }
        dataCenters[currentDataCenter].SetMalTraffic(malTraffic.Select(attack => (attack == null) ? -1 : attack.GetId()).ToArray());

        // Initialize event listeners for the delete button on each traffic object
        InitTrafficListeners();
    }

    /**
     *  Initializes the event listeners for the buttons of each email object
     */
    public void InitTrafficListeners() {
        // Iterate through all traffic options
        foreach(GameObject traffic in dataCenters[currentDataCenter].GetTraffic()) {
            GameObject deleteButton = traffic.transform.GetChild(0).gameObject;
            // Add onClick listener to delete button
            deleteButton.GetComponent<Button>().onClick.AddListener(delegate {
                TrafficClick(traffic);
            });
        }
    }

    /**
     *  onClick listener for the traffic delete button
     *  @param {GameObject} self - The GameObject of the traffic entry
     */
    public void TrafficClick(GameObject self) {
        Debug.Log("traffic has been deleted");
        Destroy(self);
        // !!! - reduce production rate and if infected
    }

    /**
     *  Returns the data center at a given index.
     *  @param {int} i - The index of the data center.
     *  @returns {DataCenter} - The data center at the given index.
     */
    public DataCenter GetDataCenter(int i) {
        return dataCenters[i];
    }
    
    public List<DataCenter> GetDataCenters() {
        return dataCenters;
    }

    public void SetDataCenters(List<DataCenter> dataCenters) {
        this.dataCenters = dataCenters;
    }

    public void Work() {
        Player player = playerManager.GetPlayer(gameManager.GetTurnPlayer());

        dataCenters
            .Where(d => d.GetOwner() == player.GetId())
            .ToList()
            .ForEach(d => {
                if (d.IsActive())
                    player.SetMoney(player.GetMoney() + d.GetMoney());
                d.SetActive(Math.Max(d.GetActive()-1, 0));
            });
    }

    public void Save() {
        DataCenterDAO dao = new DataCenterDAO();
        dao.Save(this);
    }

    public void Load() {
        DataCenterDAO dao = new DataCenterDAO();
        if (!dao.Load(this)) InitDataCenters();
    }
}
