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
 

    public void Process(Attack a, DataCenter dc) {
        if (a.GetDelivery() == "phishing") {
            dc.AddPhish(a.GetId());
            return;
        }

        // Get Random Number in [0, 1]
        System.Random random = new System.Random();
        float result = (float) random.NextDouble();

        do {
            if (a.GetDelivery() == "backdoor") {
                if (!dc.GetExploits().ContainsKey(a.GetOwner())) break;
                else if (dc.GetExploit(a.GetOwner()) == 10) break;
                else if (0.005 * dc.GetExploit(a.GetOwner()) + 0.5 > result) {
                    Debug.Log("Success: " + result + "----" + (0.005 * dc.GetExploit(a.GetOwner()) + 0.5).ToString());
                    Infect(a, dc);
                    return;
                } else {
                    Debug.Log("Failure: " + result + "----" + (0.005 * dc.GetExploit(a.GetOwner()) + 0.5).ToString());
                    FinishAttack(a, dc);
                    return;
                }
            }
        } while (false);

        // Check if Recon
        if ((a.GetMalware() % 100) == 0 && a.GetMalware() > 0) {
            if ((0.95 - dc.GetHiddenStructure() * 0.09) > result) {
                dc.AddExploit(a.GetOwner(), 10);
                Debug.Log("Recon Successful");
            } else Debug.Log("Recon Failed");
            a.Reset();
            return;
        }

        Malware m = malwareController.GetMalware(a.GetMalware());

        // Get Malware Attributes
        int[] attr = m.GetAttributes();

        // Calculate Attack Chance
        float offenceScore = Math.Max(attr.Zip(weights, (a,w) => a * w / 100f).Sum() / 1.2f, 0f);

        // Apply Thresholds
        if (!(m.GetMalwareType() == "rootkit" || m.GetMalwareType() == "trojan")) {
            // Apply IPS Threshold
            if (offenceScore < dc.GetIPS() * 0.15) {
                Debug.Log("Attack Caught by IPS");
                FinishAttack(a, dc);
                return;
            }

            // Apply IDS Threshold
            if (offenceScore < dc.GetIDS() * 0.15) {
                Debug.Log("Attack Caught by IDS");
            }
        }

        Debug.Log("Offence0: " + offenceScore + " - " + result);
        if (dc.GetExploit(a.GetOwner()) > 0) 
            offenceScore = (100*offenceScore + 2*dc.GetExploit(a.GetOwner())) / (100 + 2*dc.GetExploit(a.GetOwner()));
        Debug.Log("OffenceE: " + offenceScore + " - " + result);
        if (dc.GetRecord().Contains(m.GetTime())) 
            offenceScore /= 2;
        Debug.Log("OffenceER: " + offenceScore + " - " + result);
        if (!(m.GetMalwareType() == "trojan")) 
            offenceScore /= (float) dc.GetFirewall() * 3f + 1f;
        Debug.Log("OffenceERF: " + offenceScore + " - " + result);

        if (offenceScore < result) {
            Debug.Log("Attack Failed - " + offenceScore + " < " + result);
            FinishAttack(a, dc);
            return;
        } else {
            Debug.Log("Attack Successful - " + offenceScore + " > " + result);
            Infect(a, dc);
        }
    }

    public void Infect(Attack a, DataCenter dc) {
        Malware m = malwareController.GetMalware(a.GetMalware());
        if (m.GetMalwareType() == "botnet" || m.GetMalwareType() == "adware" || m.GetMalwareType() == "ransomware") 
            dc.AddAttack(a.GetId());
        
        int[] attr = m.GetAttributes();

        Player attacker = playerManager.GetPlayer(a.GetOwner());
        Player defender = playerManager.GetPlayer(dc.GetOwner());

        switch(a.GetObjective()) {
            case "money":
                int amount = Math.Min(Math.Max(attr[3] * defender.GetMoney() / 200, attr[3] / 2), defender.GetMoney());
                attacker.SetMoney(attacker.GetMoney() + amount);
                defender.SetMoney(defender.GetMoney() - amount);
                break;

            case "research":// get random research from owner of dc and set true on attacker. better intrusion = better goals
                //requires goals
                break;

            case "sabotage":
                // look through all of the owner's attacks/malware/datacenter and pick one at random if work is being done and set work done to 0
                // workable interface - work target (self)
                break;
            
            case "disable":
                dc.SetActive(attr[3] / 25 + 1);
                break;

            case "backdoor":
                dc.AddExploit(a.GetOwner(), 80 * attr[3] / 100 + 20);
                break;

            default:
                Debug.Log("Invalid Objective");
                break;
        }

        if (m.GetMalwareType() == "worm") {
            if (m.GetSpread() == -1) m.SetSpread(5);
            m.SetSpread(m.GetSpread() - 1);
            if (m.GetSpread() > 0) {
                DataCenter[] dataCenters =  dataCenterManager
                                            .GetDataCenters()
                                            .Where(d => (d.GetOwner() == dc.GetOwner() && 
                                                d.GetId() != dc.GetId()))
                                            .ToArray();
                System.Random random = new System.Random();
                int idx = random.Next(0, dataCenters.Length);
                Debug.Log("Spreading Worm.");
                Process(a, dataCenters[idx]);
            } else {
                FinishAttack(a, dc);
            }
        } else {
            FinishAttack(a, dc);
        }

    }

    private void FinishAttack(Attack a, DataCenter dc) {
        dc.AddRecord(malwareController.GetMalware(a.GetMalware()).GetTime());
        a.Reset();
    }

    private void ReconAttack(Attack attack, DataCenter dataCenter) {
        //dataCenter.AddAttack(attack.GetMalware()GetId());
    }
}
