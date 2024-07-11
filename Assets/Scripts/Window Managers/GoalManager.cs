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
    private Color uncompletedColor;
    [SerializeField]
    private Color completedColor;
    [SerializeField]
    private Color workingColor;
    [SerializeField]
    private Color stoppedColor;

    private Goal startGoal;
    private Goal endGoal;

    private static List<Goal> goals;

    private int active;

    private static int workTarget;

    public void Start() {
        resourceDisplay.GetComponent<TextMeshProUGUI>().SetText("0");
        UpdateDisplay();
    }

    public void Load() {
        workTarget = PlayerManager.GetPlayer(GameManager.GetTurnPlayer()).GetWorkTarget();
        InitGoals();
        UpdateDisplay();
    }

    public void GoalClick(int i) {
        active = i;
        workTarget = i;
        PlayerManager.GetPlayer(GameManager.GetTurnPlayer()).SetWorkTarget(workTarget);
        Debug.Log("Goal " + active + " has been selected.");
        UpdateDisplay();
    }

    public void ResourceClick(int change) {
        Player player = PlayerManager.GetPlayer(GameManager.GetTurnPlayer());
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
            if (workDone >= workRequired) {
                goal.GetComponent<Image>().color = completedColor;
            } else if (workTarget == gid) {
                goal.GetComponent<Image>().color = workingColor;
            } else if (workDone > 0) {
                goal.GetComponent<Image>().color = stoppedColor;
            } else {
                goal.GetComponent<Image>().color = uncompletedColor;
            }
        }
    }

    public void InitGoals() {
        goals = new List<Goal>();
        goals.Add(new Goal(0, 0, new List<string>(){"virus", "manual"}));//0

        goals.Add(new Goal(1, 100, new List<string>(){"worm", "drain", "firewall"}));//1
        goals.Add(new Goal(2, 100, new List<string>(){"recon", "rootkit", "encryption"}));//2
        goals.Add(new Goal(3, 100, new List<string>(){"adware", "phishing"}));//3
        goals.Add(new Goal(4, 100, new List<string>(){"structure", "apt"}));//4

        goals.Add(new Goal(5, 200, new List<string>(){"polymorphism", "email-filter"}));//5
        goals.Add(new Goal(6, 200, new List<string>(){"zero-day", "backdoor"}));//6
        goals.Add(new Goal(7, 200, new List<string>(){"scan", "patch"}));//7

        goals.Add(new Goal(8, 300, new List<string>(){"trojan"}));//8
        goals.Add(new Goal(9, 300, new List<string>(){"ids"}));//9
        goals.Add(new Goal(10, 300, new List<string>(){"sabotage", "dlp"}));//10

        goals.Add(new Goal(11, 400, new List<string>(){"cuckoo-egg"}));//11
        goals.Add(new Goal(12, 400, new List<string>(){"obfuscation"}));//12

        goals.Add(new Goal(13, 500, new List<string>(){"botnet"}));//13
        goals.Add(new Goal(14, 500, new List<string>(){"steganography"}));//14
        goals.Add(new Goal(15, 500, new List<string>(){"disable"}));//15
        goals.Add(new Goal(16, 500, new List<string>(){"ips"}));//16

        goals.Add(new Goal(17, 600, new List<string>(){"ransomware"}));//17

        goals.Add(new Goal(18, 1000, new List<string>(){"end-1"}));//18
        goals.Add(new Goal(19, 1000, new List<string>(){"end-2"}));//19

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

        goals[12].AddChild(goals[18]);

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
