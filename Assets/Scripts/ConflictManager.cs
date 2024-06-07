using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConflictManager : MonoBehaviour
{
    [SerializeField]
    private MalwareController malwareController;
    [SerializeField]
    private PlayerManager playerManager;

    float[] weights = {1f, 0.2f, -0.8f, -0.1f};
    public void Interact(Attack attack, DataCenter dataCenter) {


        int[] values = malwareController.GetMalware()[attack.GetMalware()].GetAttributes();
        float chance = 0f;



        for (int i = 0; i < values.Length; i++)
            chance += values[i] * weights[i];

        chance = Math.Clamp(chance / 1.2f, 0.05f, 0.95f);

        Debug.Log("Chance: " + chance);

        Player p0 = playerManager.GetPlayer(attack.GetOwner());
        Player p1 = playerManager.GetPlayer(dataCenter.GetOwner());

        p0.SetAvailableResources(p0.GetAvailableResources() + attack.GetResourceRate());
        attack.SetResourceRate(0);


        System.Random random = new System.Random();
        float result = (float) random.NextDouble();

        if (chance > result) {
            Debug.Log("Attack " + attack.GetId() + " Successful :)");
            switch(attack.GetObjective()) {
                case "money":
                    p0.SetMoney(p0.GetMoney() + values[3] + 1);
                    p1.SetMoney(p1.GetMoney() - values[3] - 1);
                    break;
                default:
                    break;
            }
        } else {
            Debug.Log("Attack " + attack.GetId() + " Failed :(");
        }
    }
}
