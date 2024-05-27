using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    private Goal finalGoal;

    private void LoadGoals() {
        List<Goal> goals = new List<Goal>();
        for (int i = 0; i < 11; i++) {
            goals.Add(new Goal(i * GameManager.VALUE_SCALE));
        }
        finalGoal = goals[10];

        goals[0].AddChild(goals[1]);
        goals[0].AddChild(goals[2]);

        goals[1].AddChild(goals[3]);
        goals[1].AddChild(goals[4]);
        goals[1].AddChild(goals[5]);

        goals[2].AddChild(goals[3]);
        goals[2].AddChild(goals[4]);
        goals[2].AddChild(goals[5]);

        goals[3].AddChild(goals[7]);
        goals[3].AddChild(goals[8]);

        goals[4].AddChild(goals[6]);
        goals[4].AddChild(goals[8]);
        goals[4].AddChild(goals[9]);

        goals[5].AddChild(goals[6]);
        goals[5].AddChild(goals[10]);

        goals[6].AddChild(goals[7]);

        goals[7].AddChild(goals[10]);

        goals[8].AddChild(goals[10]);

        goals[9].AddChild(goals[10]);
    }
}
