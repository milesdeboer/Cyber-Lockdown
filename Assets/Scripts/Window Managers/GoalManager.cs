using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour, ISavable
{
    private Goal startGoal;
    private Goal endGoal;

    private List<Goal> goals;

    private int active;

    public void Start() {
        InitGoals();
    }

    public void GoalClick(int i) {
        active = i;
        Debug.Log("Goal " + active + " has been selected.");
    }

    public void InitGoals() {
        List<Goal> goals = new List<Goal>();
        goals.Add(new Goal(0, new List<string>(){"virus", "manual"}));//0

        goals.Add(new Goal(100, new List<string>(){"worm", "drain", "firewall"}));//1
        goals.Add(new Goal(100, new List<string>(){"recon", "rootkit", "encryption"}));//2
        goals.Add(new Goal(100, new List<string>(){"adware", "phishing"}));//3
        goals.Add(new Goal(100, new List<string>(){"structure", "apt"}));//4

        goals.Add(new Goal(200, new List<string>(){"polymorphism", "email-filter"}));//5
        goals.Add(new Goal(200, new List<string>(){"zero-day", "backdoor"}));//6
        goals.Add(new Goal(200, new List<string>(){"scan", "patch"}));//7

        goals.Add(new Goal(300, new List<string>(){"trojan"}));//8
        goals.Add(new Goal(300, new List<string>(){"ids"}));//9
        goals.Add(new Goal(300, new List<string>(){"sabotage", "dlp"}));//10

        goals.Add(new Goal(400, new List<string>(){"cuckoo-egg"}));//11
        goals.Add(new Goal(400, new List<string>(){"obfuscation"}));//12

        goals.Add(new Goal(500, new List<string>(){"botnet"}));//13
        goals.Add(new Goal(500, new List<string>(){"steganography"}));//14
        goals.Add(new Goal(500, new List<string>(){"disable"}));//15
        goals.Add(new Goal(500, new List<string>(){"ips"}));//16

        goals.Add(new Goal(600, new List<string>(){"ransomware"}));//17

        goals.Add(new Goal(1000, new List<string>(){"end-1"}));//18
        goals.Add(new Goal(1000, new List<string>(){"end-2"}));//19

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
