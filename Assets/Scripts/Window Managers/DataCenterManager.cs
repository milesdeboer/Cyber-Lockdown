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
    public static int DATA_CENTER_COST = 20000;

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
    private GameObject traffic;
    [SerializeField]
    private GameObject trafficSubWindow;

    [SerializeField]
    private GameObject resourceDisplay;
    [SerializeField]
    private GameObject requirementDisplay;

    [SerializeField]
    private GameObject ransomOffer;

    [SerializeField]
    private GameObject popUpCanvas;
    [SerializeField]
    private GameObject ad;

    private List<DataCenter> dataCenters;
    private List<GameObject> dataCenterButtons;

    private Queue<int> patchQueue;
    private bool patching;
    private bool scanning;


    private int currentDataCenter = 0;

    private int ransomId = -1;

    public void Start_() {
        //InitButtons();
        InitMap(GameManager.DATA_CENTERS_PER_PLAYER, GameManager.GetNumPlayers());
        //InitSelectionListeners();
        //InitTraffic();
    }

    /// <summary>
    /// Initializes the Data Center Objects 
    /// </summary>
    public void InitDataCenters() {
        dataCenters = new List<DataCenter>();
        for (int i = 0; i < GameManager.DATA_CENTERS_PER_PLAYER * GameManager.GetNumPlayers(); i++) {
            dataCenters.Add(new DataCenter(i));
            if (i % GameManager.DATA_CENTERS_PER_PLAYER == 0) dataCenters[i].SetOwner(i / GameManager.DATA_CENTERS_PER_PLAYER);
        }
    }

    /// <summary>
    /// Instantiates the data center selection buttons.
    /// </summary>
    public void InitButtons() {
        GameObject.FindGameObjectsWithTag("DataCenterButton").ToList().ForEach(dc => Destroy(dc));
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
            dataCenter.transform.GetChild(1).gameObject.SetActive(false);
            if (dataCenters[i].GetOwner() == -1) {
                var x = i;
                dataCenter.transform.GetChild(1).gameObject.SetActive(true);
                dataCenter.GetComponent<Button>().onClick.AddListener(delegate {
                    Purchase(x);
                });
            }

            // Add to list of buttons.
            dataCenterButtons.Add(dataCenter);
        }
        InitMap(GameManager.DATA_CENTERS_PER_PLAYER, GameManager.GetNumPlayers());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dcpp">Data Centers Per Player</param>
    /// <param name="p">The number of Players</param>
    public void InitMap(int dcpp, int p) {
        GameObject.FindGameObjectsWithTag("DataCenterButton").ToList().ForEach(dc => Destroy(dc));
        // Initialize the list of data center buttons
        dataCenterButtons = new List<GameObject>();

        float padding = 20f;
        float[] xRange = new float[2]{  - (float) GameManager.SCREEN_DIMENSION.x / 2f, 
                                        (float) GameManager.SCREEN_DIMENSION.x / 2f};
        float[] yRange = new float[2]{  -460f, 
                                        460f};
        //The width and height of the area a data center is allowed to be intantiated in
        float width = (xRange[1] - xRange[0] - padding * (float) (p+1)) / p;
        float height = (yRange[1] - yRange[0] - padding * (float) (dcpp+1)) / dcpp;

        for(int i = 0; i < p; i++) {
            float xMin = xRange[0] + padding + (float) i * (width + padding);
            float xMax = xMin + width;

            for (int j = 0; j < dcpp; j++) {
                float yMin = yRange[0] + padding + (float) j * (height + padding);
                float yMax = yMin + height;

                float x = UnityEngine.Random.Range(xMin + 25f, xMax - 25f);
                float y = UnityEngine.Random.Range(yMin + 25f, yMax - 25f);

                GameObject dataCenter = Instantiate(dataCenterButton, new Vector3(x, y, 0f), Quaternion.identity);
                dataCenter.transform.SetParent(selectionWindow.transform, false);
                dataCenter.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);

                int idx = (int) (GameManager.DATA_CENTERS_PER_PLAYER*i + j);

                dataCenter.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText((dataCenters[idx].GetOwner()+1).ToString());

                if (dataCenters[idx].GetOwner() == -1) {
                    dataCenter.transform.GetChild(1).gameObject.SetActive(true);
                    dataCenter.GetComponent<Button>().onClick.AddListener(delegate {
                        Purchase(idx);
                    });
                } else if (dataCenters[idx].GetOwner() == GameManager.GetTurnPlayer()) {
                    dataCenter.GetComponent<Button>().onClick.AddListener(delegate {
                        SelectionClick(idx);
                    });
                }
                dataCenterButtons.Add(dataCenter);
            }
        }

    }

    public List<GameObject> GetDataCenterButtons() {
        return dataCenterButtons;
    }

    /// <summary>
    /// onClick listener for unowned data center button
    /// </summary>
    /// <param name="dcid">The index of the target data center</param>
    public void Purchase(int dcid) {
        Debug.Log("Purchasing " + dcid);
        Debug.Log("Total DataCenters: " + dataCenters.Count);
        Player player = PlayerManager.GetPlayer(GameManager.GetTurnPlayer());
        if (player.GetMoney() >= DATA_CENTER_COST) {
            player.AddMoney(-DATA_CENTER_COST);
            dataCenters[dcid].SetOwner(player.GetId());
            //InitButtons();
            InitMap(GameManager.DATA_CENTERS_PER_PLAYER, GameManager.GetNumPlayers());
            playerManager.UpdateDisplay();
        }
    }

    /// <summary>
    /// Adds onClick event listeners for each data center button
    /// </summary>
    public void InitSelectionListeners() {/////////!!!
        // Iterate through data center buttons
        for (int i = 0; i < dataCenterButtons.Count; i++) {
            // Avoid capturing error
            int x = i;

            // Add the DataCenterClick listener
            if (dataCenters[x].GetOwner() == GameManager.GetTurnPlayer()) 
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
        Debug.Log("Selection CLick: " + i);
        if (dataCenters[i].GetOwner() == GameManager.GetTurnPlayer()) {
            
            currentDataCenter = i;
            AdCheck();
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

        if (sliders.Length > 0)
            sliders[0].GetComponent<Slider>().value = (float) dataCenters[currentDataCenter].GetFirewall() / GameManager.VALUE_SCALE;

        // reset checkbox
        GameObject scanpatch = GameObject.FindGameObjectsWithTag("scanpatch").ToList().Single();
        Checkbox checkbox = scanpatch.GetComponent<Checkbox>();
        checkbox.UpdateDisplay();
        if (dataCenters[currentDataCenter].IsScanning()) checkbox.OnClick(scanpatch.transform.GetChild(0).gameObject);
        if (dataCenters[currentDataCenter].IsPatching()) checkbox.OnClick(scanpatch.transform.GetChild(1).gameObject);

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
            .Where(a => a < 1000)
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
    /// Checks target dataCenter for adware and creates Ads
    /// </summary>
    public void AdCheck() {;
        dataCenters[currentDataCenter]
            .GetAttacks()
            .Where(a => a < 1000)
            .Select(a => attackManager.GetAttack(a))
            .Where(a => malwareManager.GetMalware(a.GetMalware()).GetMalwareType() == "adware")
            .ToList()
            .ForEach(a => {
                CreateAds(4);
            });
    }

    /// <summary>
    /// Creates N Ad Objects
    /// </summary>
    /// <param name="N">The number of objects to instantiate</param>
    public void CreateAds(int N) {
        System.Random random = new System.Random();
        Vector2 location = new Vector2(0f, 0f);
        for(int i = 0; i < N; i++) {
            location.x = (float) random.Next(-584, 584);
            location.y = (float) random.Next(-75, 296);

            GameObject adObject = Instantiate(ad, location, Quaternion.identity);
            adObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(delegate {
                Destroy(adObject);
            });
            adObject.transform.SetParent(popUpCanvas.transform, false);
        }
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
            requirementDisplay.GetComponent<TextMeshProUGUI>().SetText(dataCenters[currentDataCenter].GetWorkRequirement().ToString());
            Debug.Log("Increased " + attribute + " of data center " + currentDataCenter + " by one.");
            UpdateAttributes();
        } 
    }

    /// <summary>
    /// Cancels all work on the target dataCenter given an attribute
    /// </summary>
    /// <param name="attribute">The attribute that the work is being canceled</param>
    public void CancelWorkClick(string attribute) {
        string work = dataCenters[currentDataCenter].GetWorkTarget();
        List<string> targets = work.Split("/").Skip(1).ToList();
        targets.Remove(attribute);
        dataCenters[currentDataCenter].SetWorkTarget(string.Join("/", targets));
        UpdateAttributes();
    }

    /// <summary>
    /// Updates the labels on the attribute buttons
    /// </summary>
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
                    aLevel = dataCenters[currentDataCenter].GetDLP().ToString() + ((levels[1] > 0) ? "→" + (dataCenters[currentDataCenter].GetDLP() + levels[1]).ToString() : "");
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
            if (aLevel != "") g.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText("Level: " + aLevel.ToString());
        }
    }

    /// <summary>
    /// onChange event listener which changes the firewall variable of target dataCenter depending on value from slider
    /// </summary>
    /// <param name="value">the current value of the slider</param>
    public void FirewallChange(float value) {
        dataCenters[currentDataCenter].SetFirewall((int) (value * GameManager.VALUE_SCALE));
        Debug.Log("Updated Firewall Strength to " + ((int) (value * GameManager.VALUE_SCALE)) + ".");
    }

    /// <summary>
    /// onClick event listener to go back to data center selection window.
    /// </summary>
    public void Back() {
        selectionWindow.SetActive(true);
        currentDataCenter = 0;
        customizationWindow.SetActive(false);
    }

    /// <summary>
    /// Initializes the traffic objects and gives them names depending on what malware infected the dataCenter
    /// </summary>
    public void InitTraffic() {
        GameObject[] traffic_ = GameObject.FindGameObjectsWithTag("Traffic");
        foreach(GameObject traffic in traffic_) Destroy(traffic);

        // Get random order for the traffic entries
        System.Random random = new System.Random();
        int order = random.Next(0, 4);
        Vector3[] coords = {
            new Vector3(0f, (172.5f + (float)(Math.Max(order-1,0) % 3) * 115f - (float)((order+3)/4) * 345f), 0f),
            new Vector3(0f, ((float) (order % 2) * 115f + 57.5f - (float) (order / 2) * 230f), 0f),
            new Vector3(0f, ((float) (order % 3) * 115f - 57.5f - (float) (order / 3) * 115f), 0f),
            new Vector3(0f, ((float) order * 115f - 172.5f), 0f)
        };
        foreach(Vector3 vec in coords) {
            Debug.Log("Vector: " + vec.ToString());
        }
        // Declare traffic objects
        GameObject[] trafficObjects = {
            Instantiate(traffic, Vector2.zero, Quaternion.identity),
            Instantiate(traffic, Vector2.zero, Quaternion.identity),
            Instantiate(traffic, Vector2.zero, Quaternion.identity),
            Instantiate(traffic, Vector2.zero, Quaternion.identity)
        };
        Debug.Log("order " + order);
        // Orient each traffic obejct to correct position
        for (int j = 0; j < trafficObjects.Length; j++) {
            trafficObjects[j].transform.SetParent(trafficSubWindow.transform, false);
            trafficObjects[j].GetComponent<RectTransform>().anchoredPosition = coords[j];
        }

        //TODO !!! - add chance of undetected malware
        int i = 0;

        Shuffle<int>(
            dataCenters[currentDataCenter]
                .GetAttacks()
                .Where(a => a < 1000)// filter out egg
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
                    .ToList())// add eggs and shuffle
                .Concat(dataCenters[currentDataCenter]
                    .GetAttacks()
                    .Where(a => a > 1000))
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

    private List<T> Shuffle<T>(List<T> list) {
        List<T> l = list.ToList();
        List<T> list_ = new List<T>();
        while (l.Count > 0) {
            int i = UnityEngine.Random.Range(0, l.Count - 1);
            list_.Add(l[i]);
            l.Remove(l[i]);
        }
        return list_;
    }

    /// <summary>
    /// Initializes the event listeners for the buttons of each email object
    /// </summary>
    /// <param name="traffic">Array of the traffic objects</param>
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

    /// <summary>
    /// onClick listener for the traffic delete button
    /// </summary>
    /// <param name="self">The GameObject of the traffic entry</param>
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
                string body = (aid > 1000) ? "You found a decoy virus" : ("Your system was infected with a " + malwareManager.GetMalware(attackManager.GetAttack(aid).GetMalware()).GetMalwareType());
                notificationManager.AddNotification(
                    ("Successfully removed malware from Data Center " + currentDataCenter.ToString()),
                    body,
                    GameManager.GetTurnPlayer()
                );
            }
        }
        // Decrease production rate
        Destroy(self);
    }

    /// <summary>
    /// Returns the data center at a given index.
    /// </summary>
    /// <param name="i">The index of the data center.</param>
    /// <returns>The data center at the given index.</returns>
    public DataCenter GetDataCenter(int i) {
        return dataCenters[i];
    }
    
    /// <summary>
    /// returns the list of DataCenters
    /// </summary>
    /// <returns>List of DataCenters</returns>
    public List<DataCenter> GetDataCenters() {
        return dataCenters;
    }

    /// <summary>
    /// Sets the dataCenters list
    /// </summary>
    /// <param name="dataCenters">new dataCenters list</param>
    public void SetDataCenters(List<DataCenter> dataCenters) {
        this.dataCenters = dataCenters;
    }

    /// <summary>
    /// onClick listener for the resource buttons which increases the allocated resources.
    /// </summary>
    /// <param name="change">the amount added to allocated resources. usually -1 or 1</param>
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

    /// <summary>
    /// Performs work on data center tasks, adds money to players controlling adware and decreases the amount of turns a data center is disabled
    /// </summary>
    public void Work() {
        Player player = PlayerManager.GetPlayer(GameManager.GetTurnPlayer());

        dataCenters
            .Where(d => d.GetOwner() == player.GetId())
            .ToList()
            .ForEach(d => {
                if (d.IsActive()) {
                    d.AddWorkResources(d.GetWorkRate());
                    if (d.IsPatching()) Patch(d.GetId());
                    if (d.IsScanning()) Scan(d.GetId());
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
                d.GetAttacks()
                    .Where(a => a < 1000)
                    .Select(a => malwareManager.GetMalware(attackManager.GetAttack(a).GetMalware()))
                    .Where(m => m.GetMalwareType() == "adware")
                    .ToList()
                    .ForEach(m => PlayerManager.GetPlayer(m.GetOwner()).AddMoney(9*m.GetIntrusion() + 100));
                d.SetActive(Math.Max(d.GetActive()-1, 0));
            });
    }

    public void ToggleScan() {
        dataCenters[currentDataCenter].EnableScan(!dataCenters[currentDataCenter].IsScanning());
    }

    /// <summary>
    /// Scans target data center for infected malware and exploits then adds them to the patch queue before sending a notification to the player.
    /// </summary>
    /// <param name="dcid">The id of the target data center.</param>
    public void Scan(int dcid) {
        DataCenter dc = dataCenters[dcid];
        int scanCost = dc.GetSize();
        if (dc.GetWorkResources() - scanCost > 0) {
            dc.SetWorkResources(dc.GetWorkResources() - scanCost);
            int count = 0;
            dc
                .GetAttacks()
                .Concat(dc
                    .GetExploits()
                    .Select(e => e.Key)
                    .ToList())
                .ToList()
                .ForEach(v => {
                    dc.AddToPatchQueue(v);
                    count++;
                });
            string title = "DataCenter Scan Complete";
            string body = ("You have scanned " + count + " pieces of malware that have infected data center " + dcid + ".");
            notificationManager.AddNotification(title, body, dc.GetOwner());
        }
    }

    public void TogglePatch() {
        dataCenters[currentDataCenter].EnablePatch(!dataCenters[currentDataCenter].IsPatching());
    }

    /// <summary>
    /// Removes PATCH_COST resources from the allocated resources for the data center and removes one exploit/attack from the target data center.
    /// </summary>
    /// <param name="dcid">The id of the target data center.</param>
    public void Patch(int dcid) {
        DataCenter dc = dataCenters[dcid];
        while(dc.GetWorkResources() - DataCenter.PATCH_COST > 0 && dc.GetPatchQueue().Count > 0) {
            dc.SetWorkResources(dc.GetWorkResources() - DataCenter.PATCH_COST);
            int target = dc.GetFromPatchQueue();
            if (target < 100) dc.RemoveExploit(target);
            else dc.RemoveAttack(target);
            string title = "Patched Malware/Exploit";
            string body = "Removed a piece of malaware/exploit from data center " + dcid + ".";
            notificationManager.AddNotification(title, body, dc.GetOwner());
        }
    }

    /// <summary>
    /// Saves dataCenters
    /// </summary>
    public void Save() {
        DataCenterDAO dao = new DataCenterDAO();
        dao.Save(this);
    }

    /// <summary>
    /// Loads dataCenters
    /// </summary>
    public void Load() {
        DataCenterDAO dao = new DataCenterDAO();
        if (!dao.Load(this)) InitDataCenters();
    }
}
