using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.UI;


public class GoalManager : MonoBehaviour
{    
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private GameObject objectContainer;
    [SerializeField]
    private GameObject resourceDisplay;
    [SerializeField]
    private GameObject requirementDisplay;

    [SerializeField]
    private GameObject goalEdges;

    [SerializeField]
    private Color uncompletedColor;
    [SerializeField]
    private Color completedColor;
    [SerializeField]
    private Color workingColor;
    [SerializeField]
    private Color stoppedColor;

    [SerializeField]
    private Sprite uncompleted;
    [SerializeField]
    private Sprite completed;
    [SerializeField]
    private Sprite working;
    [SerializeField]
    private Material greenMat;

    private Goal startGoal;
    private Goal endGoal;

    private static Dictionary<int, Goal> goals;

    private static int workTarget;

    public void Start() {
        resourceDisplay.GetComponent<TextMeshProUGUI>().SetText("0");
        UpdateDisplay();
    }

    public void OnEnable() {
        if (goalEdges != null) goalEdges.SetActive(true);
    }
    public void OnDisable() {
        if (goalEdges != null) goalEdges.SetActive(false);
    }

    public void Load() {
        workTarget = PlayerManager.GetPlayer(GameManager.GetTurnPlayer()).GetWorkTarget();
        InitGoals();
        UpdateDisplay();
    }

    public Dictionary<int, Goal> GetGoals() {
        return goals;
    }

    /// <summary>
    /// Gets a specific Goal object given its Id
    /// </summary>
    /// <param name="i">the index of the Goal in the dictionary</param>
    /// <returns>the Goal at index i</returns>
    public Goal GetGoal(int i) {
        return goals[i];
    }  

    public void GoalClick(int i) {
        if (goals[i].GetParents()
            .Select(p => p.GetId())
            .All(idx => PlayerManager.GetPlayer(GameManager.GetTurnPlayer()).GetUnlock(idx) >= goals[idx].GetWorkRequired())) {
                workTarget = i;
                PlayerManager.GetPlayer(GameManager.GetTurnPlayer()).SetWorkTarget(workTarget);
                UpdateDisplay();
                requirementDisplay.GetComponent<TextMeshProUGUI>().SetText(goals[i].GetWorkRequired().ToString());
            }
    }

    public void ResourceClick(int change) {
        Player player = PlayerManager.GetPlayer(GameManager.GetTurnPlayer());
        Debug.Log("Click");
        if (!(player.GetAvailableResources() - change > player.GetOverallResources()) &&
            !(player.GetAvailableResources() - change < 0) &&
            !(Int32.Parse(resourceDisplay.GetComponent<TextMeshProUGUI>().text) == 0 && change < 0)) {

            player.SetAvailableResources(player.GetAvailableResources() - change);
            resourceDisplay.GetComponent<TextMeshProUGUI>().SetText((Int32.Parse(resourceDisplay.GetComponent<TextMeshProUGUI>().text) + change).ToString());

            player.SetWorkRate(Int32.Parse(resourceDisplay.GetComponent<TextMeshProUGUI>().text));

            Debug.Log("Available: " + player.GetAvailableResources());
        } else {
            Debug.Log("Invalid Resources");
        }
        playerManager.UpdateDisplay();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unlockable">The name of the unlockable</param>
    /// <returns>The Id of the <b>Goal</b> associated with the unlockable</returns>
    public static int UnlockableToGId(string unlockable) {
        return goals
            .Select(kvp => kvp.Value)
            .Where(g => g.HasUnlockable(unlockable))
            .ToList()
            .Single()
            .GetId();
    }

    /// <summary>
    /// Returns Work Target
    /// </summary>
    /// <returns>the id of the target unlockable that is being worked on</returns>
    public static int GetWorkTarget() {
        return workTarget;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="workTarget"></param>
    public static void SetWorkTarget(int newWorkTarget) {
        workTarget = newWorkTarget;
    }

    /// <summary>
    /// Updates the color of each of the goal buttons.
    /// </summary>
    public void UpdateDisplay() {
        Debug.Log("Work Rate: " + PlayerManager.GetPlayer(GameManager.GetTurnPlayer()).GetWorkRate());
        resourceDisplay.GetComponent<TextMeshProUGUI>().SetText(PlayerManager.GetPlayer(GameManager.GetTurnPlayer()).GetWorkRate().ToString());
        // iterate through game objects and assign a color depending on status
        foreach(Transform gt in objectContainer.transform) {
            GameObject goal = gt.gameObject;
            // get id
            int gid = Int32.Parse(goal.name);
            // get progress
            int workDone = PlayerManager.GetPlayer(GameManager.GetTurnPlayer()).GetUnlocks()[gid];
            int workRequired = goals[gid].GetWorkRequired();
            // assign color
           /* if (workDone >= workRequired) {
                goal.GetComponent<Image>().color = completedColor;
            } else if (workTarget == gid) {
                goal.GetComponent<Image>().color = workingColor;
            } else if (workDone > 0) {
                goal.GetComponent<Image>().color = stoppedColor;
            } else {
                goal.GetComponent<Image>().color = uncompletedColor;
            }*/
            if (workDone >= workRequired) {
                goal.GetComponent<Image>().sprite = completed;
                goal.transform.GetChild(0).GetComponent<Image>().material = null;
            } else if (workTarget == gid) {
                goal.GetComponent<Image>().sprite = working;
                goal.transform.GetChild(0).GetComponent<Image>().material = greenMat;
            } else {
                goal.GetComponent<Image>().sprite = uncompleted;
                goal.transform.GetChild(0).GetComponent<Image>().material = greenMat;
            }
        }
    }

    public void InitGoals() {
        goals = new Dictionary<int, Goal>();
        goals.Add(0, new Goal(0, 0, new List<string>(){"virus", "manual"}));//0 - image 1

        goals.Add(1, new Goal(1, 100, new List<string>(){"worm", "drain", "firewall"}));//1 - image 2
        goals.Add(2, new Goal(2, 100, new List<string>(){"recon", "rootkit", "encryption"}));//2 - image 3
        goals.Add(3, new Goal(3, 100, new List<string>(){"adware", "phishing"}));//3 - image 4
        goals.Add(4, new Goal(4, 100, new List<string>(){"structure", "apt"}));//image 5

        goals.Add(5, new Goal(5, 200, new List<string>(){"polymorphism", "email-filter"}));//5 - notification window icon
        goals.Add(6, new Goal(6, 200, new List<string>(){"zero-day", "backdoor"}));//6 - image 6
        goals.Add(7, new Goal(7, 200, new List<string>(){"scan", "patch"}));//7 - image 7

        goals.Add(8, new Goal(8, 300, new List<string>(){"trojan"}));//8 - trojan icon
        goals.Add(9, new Goal(9, 300, new List<string>(){"ids"}));//9 - image 8
        goals.Add(10, new Goal(10, 300, new List<string>(){"sabotage", "dlp"}));//10------------------------------------------------

        goals.Add(11, new Goal(11, 400, new List<string>(){"cuckoo-egg"}));//11 - image 10
        goals.Add(12, new Goal(12, 400, new List<string>(){"obfuscation"}));//12 -- image 11

        goals.Add(13, new Goal(13, 500, new List<string>(){"botnet"}));//13 - botnet icon
        goals.Add(14, new Goal(14, 500, new List<string>(){"steganography"}));//14------------------------------------------------
        goals.Add(15, new Goal(15, 500, new List<string>(){"disable"}));//15------------------------------------------------
        goals.Add(16, new Goal(16, 500, new List<string>(){"ips"}));//16

        goals.Add(17, new Goal(17, 600, new List<string>(){"ransomware"}));//17

        goals.Add(18, new Goal(18, 1000, new List<string>(){"end-1"}));//18
        goals.Add(19, new Goal(19, 1000, new List<string>(){"end-2"}));//19

        // (Virus/Manual) -> {(worm/drain/firewall), (recon/rootkit/firewall), (adware/phishing), (structure/apt)}
        goals[0].AddChild(goals[1]);
        goals[0].AddChild(goals[2]);
        goals[0].AddChild(goals[3]);
        goals[0].AddChild(goals[4]);

        // (worm/drain/firewall) -> 
        goals[1].AddChild(goals[5]);
        goals[1].AddChild(goals[8]);

        goals[2].AddChild(goals[6]);
        goals[2].AddChild(goals[8]);
        goals[2].AddChild(goals[9]);
        goals[2].AddChild(goals[10]);

        goals[3].AddChild(goals[10]);

        goals[4].AddChild(goals[7]);

        goals[5].AddChild(goals[11]);

        goals[6].AddChild(goals[12]);
        goals[6].AddChild(goals[14]);
        goals[6].AddChild(goals[15]);

        goals[7].AddChild(goals[16]);

        goals[8].AddChild(goals[13]);
        goals[8].AddChild(goals[14]);

        goals[9].AddChild(goals[16]);

        goals[10].AddChild(goals[15]);

        goals[11].AddChild(goals[18]);

        goals[12].AddChild(goals[13]);

        goals[13].AddChild(goals[18]);

        goals[14].AddChild(goals[18]);

        goals[15].AddChild(goals[17]);

        goals[16].AddChild(goals[18]);

        goals[17].AddChild(goals[18]);

        goals[18].AddChild(goals[19]);

        startGoal = goals[0];
        endGoal = goals[19];
    }
}
