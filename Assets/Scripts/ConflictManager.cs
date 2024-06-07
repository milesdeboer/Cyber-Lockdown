using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConflictManager : MonoBehaviour
{
    [SerializeField]
    private MalwareController malwareController;
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private DataCenterManager dataCenterManager;

    float[] weights = {1f, 0.2f, -0.8f, -0.1f};
    public void Interact(Attack attack, DataCenter dataCenter) {


        int[] values = malwareController.GetMalware()[attack.GetMalware()].GetAttributes();
        float chance = 0f;

        for (int i = 0; i < values.Length; i++)
            chance += values[i] * weights[i];

        chance = Math.Clamp(chance / 1.2f, 0.05f, 0.95f);

        Player p0 = playerManager.GetPlayer(attack.GetOwner());
        Player p1 = playerManager.GetPlayer(dataCenter.GetOwner());

        p0.SetAvailableResources(p0.GetAvailableResources() + attack.GetResourceRate());
        attack.SetResourceRate(0);


        System.Random random = new System.Random();
        float result = (float) random.NextDouble();

        if (chance > result) {
            Debug.Log("Attack " + attack.GetId() + " Successful :)");

            int amount = Math.Min(Math.Max(values[3] * p1.GetMoney() / 200, values[3] / 2), p1.GetMoney());

            p0.SetMoney(p0.GetMoney() + amount);//!!!
            p1.SetMoney(p1.GetMoney() - amount);

            switch(malwareController.GetMalware(attack.GetMalware()).GetMalwareType()) {
                case "worm":
                    DataCenter[] dataCenters =  dataCenterManager
                                                    .GetDataCenters()
                                                    .Where(d => (d.GetOwner() == dataCenter.GetOwner()))
                                                    .ToArray();
                    int idx = random.Next(0, dataCenters.Length);
                    Debug.Log("Spreading Worm.");
                    Interact(attack, dataCenters[idx]);
                    break;
                case "ransomware":

                    break;
                case "botnet":

                    break;
                case "adware":

                    break;
                default:
                    break;
            }
        } else {
            Debug.Log("Attack " + attack.GetId() + " Failed :(");
        }
    }

    private void WormAttack() {

    }
}
