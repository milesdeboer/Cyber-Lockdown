using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DataCenterManager : MonoBehaviour, ISavable
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
    private NotificationManager notificationManager;

    [SerializeField]
    private GameObject selectionWindow;
    [SerializeField]
    private GameObject customizationWindow;
    [SerializeField]
    private GameObject ransomwareWindow;

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

    [SerializeField]
    private GameObject resourceDisplay;

    [SerializeField]
    private GameObject ransomOffer;

    private List<DataCenter> dataCenters;
    private List<GameObject> dataCenterButtons;


    private int currentDataCenter = 0;

    private int ransomId = -1;

    public void Start() {
        InitButtons();
        InitSelectionListeners();
        InitTraffic();
    }

    /**
     *  Initializes the Data Center Objects 
     */
    public void InitDataCenters() {
        dataCenters = new List<DataCenter>();
        for (int i = 0; i < GameManager.DATA_CENTERS_PER_PLAYER * GameManager.GetNumPlayers(); i++) {
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
        for (int i = 0; i < GameManager.DATA_CENTERS_PER_PLAYER * GameManager.GetNumPlayers(); i++) {
            // Calculate coordinates for the current button on the circle.
            Vector2 coords = new Vector2();
            double theta = (-2f * Math.PI * i / (GameManager.DATA_CENTERS_PER_PLAYER * GameManager.GetNumPlayers())) + angleOffset;
            coords.x = rx * (float) Math.Cos(theta) + cx;
            coords.y = ry * (float) Math.Sin(theta) + cy;

            // Instantiate GameObject.
            GameObject dataCenter = Instantiate(dataCenterButton, coords, Quaternion.identity);
            dataCenter.name = dataCenters[i].GetId().ToString();
            dataCenter.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText((dataCenters[i].GetOwner()+1).ToString());

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

    /// <summary>
    /// onClick event listener for data center button to change target data center.
    /// </summary>
    /// <param name="i">The index of the data center button</param>
    public void SelectionClick(int i) {
        if (dataCenters[i].GetOwner() == GameManager.GetTurnPlayer()) {
            
            currentDataCenter = i;
            ransomId = RansomCheck();

            if (ransomId == -1) {
                customizationWindow.SetActive(true);
                ResetSetup();
            } else {
                UpdateRansom();
            }

            selectionWindow.SetActive(false);
        }
    }

    /// <summary>
    /// Resets the setup of the attribute displays for the current data center to match the current data center.
    /// </summary>
    public void ResetSetup() {
        GameObject[] sliders = GameObject.FindGameObjectsWithTag("DataCenterAttribute").Where(g => g.GetComponent<Slider>() != null).ToArray();
        Slider slider = sliders[0].GetComponent<Slider>();
        if (slider != null) 
            slider.value = (float) dataCenters[currentDataCenter].GetFirewall() / GameManager.VALUE_SCALE;
        resourceDisplay.GetComponent<TextMeshProUGUI>().SetText(dataCenters[currentDataCenter].GetWorkRate().ToString());
        InitTraffic();
        UpdateAttributes();
    }

    /// <summary>
    /// Checks if the current data center is infected with ransomware and opens ransomware window if true.
    /// </summary>
    /// <returns>The id of the attack associated with the ransomware.</returns>
    public int RansomCheck() {
        int hasRansomware = -1;
        ransomwareWindow.SetActive(false);

        dataCenters[currentDataCenter]
            .GetAttacks()
            .Select(a => attackManager.GetAttack(a))
            .Where(a => malwareManager.GetMalware(a.GetMalware()).GetMalwareType() == "ransomware")
            .ToList()
            .ForEach(a => {
                ransomwareWindow.SetActive(true);
                hasRansomware = a.GetId();
            });

        return hasRansomware;
    }

    /// <summary>
    /// Updates the ransomware window with current data.
    /// </summary>
    public void UpdateRansom() {//intrusion and objective
        Attack a = attackManager.GetAttack(ransomId);
        int intrusion = conflictManager.ScaleIntrusion(malwareManager.GetMalware(a.GetMalware()).GetIntrusion(), dataCenters[currentDataCenter].GetEncryption(), dataCenters[currentDataCenter].GetDLP());
        
        ransomOffer.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().SetText("Objective: " + a.GetObjective() + "\n\nIntrusion Value: " + intrusion);
    }

    /// <summary>
    /// onClick event listener for the accept ransom button. It allows the attack to go forward.
    /// </summary>
    public void AcceptRansom() {
        string t1, b1, t2, b2;
        Attack a = attackManager.GetAttack(ransomId);
        int intrusion = 2 * conflictManager.ScaleIntrusion(malwareManager.GetMalware(a.GetMalware()).GetIntrusion(), dataCenters[currentDataCenter].GetEncryption(), dataCenters[currentDataCenter].GetDLP());
        
        bool notify = conflictManager.Damage(a, dataCenters[currentDataCenter], intrusion, 
            out t1, out b1, out t2, out b2);

        notificationManager.AddNotification(t1, b1, a.GetOwner());
        if (notify) notificationManager.AddNotification(t2, b2, dataCenters[currentDataCenter].GetOwner());
    }

    /// <summary>
    /// onClick event listener for attribute buttons, increasing the value of the attribute by one.
    /// </summary>
    /// <param name="attribute">The name of the attribute being changed.</param>
    public void AttributeClick(string attribute) {
        int[] levels = new int[8];

        Dictionary<string, int> attrIdx= new Dictionary<string, int>(){
            {"emailFiltering", 0},
            {"dlp", 1},
            {"hiddenStructure", 2},
            {"encryption", 3},
            {"ids", 4},
            {"ips", 5},
            {"money", 6},
            {"production", 7}
        };
        dataCenters[currentDataCenter]
            .GetWorkTarget()
            .Split("/")
            .Skip(1)
            .ToList()
            .ForEach(t => {
                levels[attrIdx[t]]++;
            });

        switch(attribute) {
            case "emailFiltering":
                levels[0] += dataCenters[currentDataCenter].GetEmailFilter();
                break;
            case "dlp":
                levels[1] += dataCenters[currentDataCenter].GetDLP();
                break;
            case "hiddenStructure":
                levels[2] += dataCenters[currentDataCenter].GetHiddenStructure();
                break;
            case "encryption":
                levels[3] += dataCenters[currentDataCenter].GetEncryption();
                break;
            case "ids":
                levels[4] += dataCenters[currentDataCenter].GetIDS();
                break;
            case "ips":
                levels[5] += dataCenters[currentDataCenter].GetIPS();
                break;
            case "money":
                levels[6] += dataCenters[currentDataCenter].GetMoney();
                break;
            case "resources":
                levels[7] += dataCenters[currentDataCenter].GetResources();
                break;
            default: break;
        }

        if (levels[attrIdx[attribute]] < 5) {
            dataCenters[currentDataCenter].AddTarget(attribute);
            dataCenters[currentDataCenter].SetWorkRequirement(dataCenters[currentDataCenter].GetWorkRequirement() + 20);
            Debug.Log("Increased " + attribute + " of data center " + currentDataCenter + " by one.");
            UpdateAttributes();
        } 
    }

    public void CancelWorkClick(string attribute) {
        string work = dataCenters[currentDataCenter].GetWorkTarget();
        List<string> targets = work.Split("/").Skip(1).ToList();
        targets.Remove(attribute);
        dataCenters[currentDataCenter].SetWorkTarget(string.Join("/", targets));
        UpdateAttributes();
    }

    public void UpdateAttributes() {
        Dictionary<string, int> attrIdx= new Dictionary<string, int>(){
            {"emailFiltering", 0},
            {"dlp", 1},
            {"hiddenStructure", 2},
            {"encryption", 3},
            {"ids", 4},
            {"ips", 5},
            {"money", 6},
            {"production", 7}
        };
        GameObject[] attributes = GameObject.FindGameObjectsWithTag("DataCenterAttribute");
        List<string> targets = dataCenters[currentDataCenter].GetWorkTarget().Split("/").Skip(1).ToList();
        int[] levels = new int[8];
        dataCenters[currentDataCenter]
            .GetWorkTarget()
            .Split("/")
            .Skip(1)
            .ToList()
            .ForEach(t => {
                levels[attrIdx[t]]++;
            });

        foreach(GameObject g in attributes) {
            string aLevel = "";
            switch(g.name) {
                case "Email Filtering":
                    aLevel = dataCenters[currentDataCenter].GetEmailFilter().ToString() + ((levels[0] > 0) ? ("→" + (dataCenters[currentDataCenter].GetEmailFilter() + levels[0]).ToString()) : "");
                    break;
                case "Data Loss Prevention":
                    aLevel = dataCenters[currentDataCenter].GetDLP().ToString() + ((levels[1] > 0) ? "→" + (dataCenters[currentDataCenter].GetDLP() + levels[1].ToString()) : "");
                    break;
                case "Hide Structure":
                    aLevel = dataCenters[currentDataCenter].GetHiddenStructure().ToString() + ((levels[2] > 0) ? "→" + (dataCenters[currentDataCenter].GetHiddenStructure() + levels[2]).ToString() : "");
                    break;
                case "Encryption":
                    aLevel = dataCenters[currentDataCenter].GetEncryption().ToString() + ((levels[3] > 0) ? "→" + (dataCenters[currentDataCenter].GetEncryption() + levels[3]).ToString() : "");
                    break;
                case "IDS":
                    aLevel = dataCenters[currentDataCenter].GetIDS().ToString() + ((levels[4] > 0) ? "→" + (dataCenters[currentDataCenter].GetIDS() + levels[4]).ToString() : "");
                    break;
                case "IPS":
                    aLevel = dataCenters[currentDataCenter].GetIPS().ToString() + ((levels[5] > 0) ? "→" + (dataCenters[currentDataCenter].GetIPS() + levels[5]).ToString() : "");
                    break;
                case "Money":

                    break;
                case "Resources":

                    break;
                default:break;
            }
            if (aLevel != "") g.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText(aLevel.ToString());
        }
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
        GameObject[] traffic_ = GameObject.FindGameObjectsWithTag("Traffic");
        foreach(GameObject traffic in traffic_) Destroy(traffic);

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

        //TODO !!! - add chance of undetected malware
        int i = 0;

        dataCenters[currentDataCenter]
            .GetAttacks()
            .Select(a => attackManager.GetAttack(a))
            .Where(a =>
                (malwareManager.GetMalware(a.GetMalware()).GetMalwareType() == "adware") ||
                (malwareManager.GetMalware(a.GetMalware()).GetMalwareType() == "botnet"))
            .Where(a => !malwareManager.GetMalware(a.GetMalware()).HasFeature(MalwareFeature.Steganography))
            .Select(a => a.GetId())
            .ToList()
            .Concat(dataCenters[currentDataCenter]
                .GetExploits()
                .Select(e => e.Key)
                .ToList())
            .Take(trafficObjects.Length)
            .ToList()
            .ForEach(a => {
                trafficObjects[i].name  = a.ToString();
                i++;
            });
            
        // Initialize event listeners for the delete button on each traffic object
        InitTrafficListeners(trafficObjects);
    }

    /**
     *  Initializes the event listeners for the buttons of each email object
     */
    public void InitTrafficListeners(GameObject[] traffic) {
        // Iterate through all traffic options
        traffic
            .ToList()
            .ForEach(t => {
                GameObject deleteButton = t.transform.GetChild(0).gameObject;
                deleteButton.GetComponent<Button>().onClick.AddListener(delegate {
                    TrafficClick(t);
                });
            });
    }

    /**
     *  onClick listener for the traffic delete button
     *  @param {GameObject} self - The GameObject of the traffic entry
     */
    public void TrafficClick(GameObject self) {
        int aid;
        if (Int32.TryParse(self.name, out aid)) {
            if (aid < 100) {
                dataCenters[currentDataCenter].RemoveExploit(aid);
                notificationManager.AddNotification(
                    ("Successfully removed backdoor from Data Center " + currentDataCenter.ToString()),
                    ("Your system was infected with a backdoor belonging to Player" + (aid+1)),
                    GameManager.GetTurnPlayer()
                );
            } else {
                dataCenters[currentDataCenter].RemoveAttack(aid);
                notificationManager.AddNotification(
                    ("Successfully removed malware from Data Center " + currentDataCenter.ToString()),
                    ("Your system was infected with a " + malwareManager.GetMalware(attackManager.GetAttack(aid).GetMalware()).GetMalwareType()),
                    GameManager.GetTurnPlayer()
                );
            }
        }
        // Decrease production rate
        Destroy(self);
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

    public void ResourceClick(int change) {
        Player player = PlayerManager.GetPlayer(GameManager.GetTurnPlayer());
        if (!(player.GetAvailableResources() - change > player.GetOverallResources()) &&
            !(player.GetAvailableResources() - change < 0) &&
            !(Int32.Parse(resourceDisplay.GetComponent<TextMeshProUGUI>().text) == 0 && change < 0)) {

            player.SetAvailableResources(player.GetAvailableResources() - change);
            resourceDisplay.GetComponent<TextMeshProUGUI>().SetText((Int32.Parse(resourceDisplay.GetComponent<TextMeshProUGUI>().text) + change).ToString());

            dataCenters[currentDataCenter].SetWorkRate(Int32.Parse(resourceDisplay.GetComponent<TextMeshProUGUI>().text));

            Debug.Log("Available: " + player.GetAvailableResources());
        } else {
            Debug.Log("Invalid Resources");
        }
        playerManager.UpdateDisplay();
    }

    public void Work() {
        Player player = PlayerManager.GetPlayer(GameManager.GetTurnPlayer());

        dataCenters
            .Where(d => d.GetOwner() == player.GetId())
            .ToList()
            .ForEach(d => {
                if (d.IsActive()) {
                    d.AddWorkResources(d.GetWorkRate());
                    if (d.IsComplete()) {
                        d.GetWorkTarget().Split("/").Skip(1).ToList()
                            .ForEach(t => d.IncrementAttribute(t.ToString()));
                        player.SetAvailableResources(player.GetAvailableResources() + d.GetWorkRate());
                        d.SetWorkTarget("");
                        d.SetWorkResources(0);
                        d.SetWorkRequirement(0);
                        d.SetWorkRate(0);
                        Debug.Log("Work on Data Center is Complete");
                    }
                    player.SetMoney(player.GetMoney() + d.GetMoney());
                }
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
