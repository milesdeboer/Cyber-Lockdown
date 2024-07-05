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
    private AttackManager attackManager;
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private DataCenterManager dataCenterManager;
    [SerializeField]
    private NotificationManager notificationManager;

    float[] weights = {1f, 0.2f, -0.8f, -0.1f};
 

    public void Process(Attack a, DataCenter dc) {
        // Get Random Number in [0, 1]
        System.Random random = new System.Random();
        float r = (float) random.NextDouble();

        Malware m = ((a.GetMalware() % 100) == 0) ? new Malware(0) : malwareController.GetMalware(a.GetMalware());
        // Get Malware Attributes
        int[] attr = m.GetAttributes();

        if (a.GetDelivery() == "phishing") {
            int k = -attr[3] + 50;
            int x = dc.GetEmailFilter();
            float alpha = 0.45f;
            float beta = 5f;
            float score = (k == 0) ? -0.18f * x + 1f : 
                (alpha * (float) (1f - (x / beta) + (Math.Pow(Math.E, -k*x/beta) - Math.Pow(Math.E, -k))/(1f - Math.Pow(Math.E, -k))) + 0.1f);
            if (score >= r) {
                dc.AddPhish(a.GetId());
                string title = "Phish Successfully Sent :D";
                string body = "Attack No. " + a.GetId().ToString() + "has been successfully placed in Player " + (dc.GetOwner()+1).ToString() + "'s inbox.";
                notificationManager.AddNotification(new Notification(title, body, a.GetOwner()));
            }
            return;
        }

        do {
            if (a.GetDelivery() == "backdoor") {
                if (!dc.GetExploits().ContainsKey(a.GetOwner())) break;
                else if (dc.GetExploit(a.GetOwner()) == 10) break;
                else if (0.005 * dc.GetExploit(a.GetOwner()) + 0.5 > r) {
                    Infect(a, dc);
                    return;
                } else {
                    FinishAttack(a, dc);
                    return;
                }
            }
        } while (false);

        // Check if Recon
        if ((a.GetMalware() % 100) == 0 && a.GetMalware() > 0) {
            string title;
            string body;
            if ((0.95 - dc.GetHiddenStructure() * 0.09) > r) {
                dc.AddExploit(a.GetOwner(), 10);
                title = "Recon Successful :D";
                body = "Your reconnaissance against data center " + dc.GetId() + " has been successful. You will now have a greater chance infecting this data center.";
            } else {
                title = "Recon Failed :(";
                body = "Your reconnaissance against data center " + dc.GetId() + " has failed.";
            }
            notificationManager.AddNotification(new Notification(title, body, a.GetOwner()));
            a.Reset();
            return;
        }

        // Calculate Attack Chance
        float offenceScore = Math.Max(attr.Zip(weights, (a,w) => a * w / 100f).Sum() / 1.2f, 0f);

        // Apply Thresholds
        if (!(m.GetMalwareType() == "rootkit" || m.GetMalwareType() == "trojan")) {
            // Apply IPS Threshold
            if (offenceScore < dc.GetIPS() * 0.15) {
                string title = "Data Center " + dc.GetId() + " Attacked";
                string body = "Player " + a.GetOwner() + " has tried to infect your data center (data center " + dc.GetId() + ") with a " + m.GetMalwareType() + ". This attack was seen and prevented by your Intrusion Detection System and Intrusion Prevention System.";
                notificationManager.AddNotification(new Notification(title, body, dc.GetOwner()));
                FinishAttack(a, dc);
                return;
            }

            // Apply IDS Threshold
            if (offenceScore < dc.GetIDS() * 0.15) {
                string title = "Data Center " + dc.GetId() + " Possibly Infected";
                string body = "Player " + a.GetOwner() + " has tried to infect your data center (data center " + dc.GetId() + ") with a " + m.GetMalwareType() + ". This attack was seen by your Intrusion Detection System.";
                notificationManager.AddNotification(new Notification(title, body, dc.GetOwner()));
            }
        }

        int exploit = (m.HasFeature(MalwareFeature.ZeroDayExploit)) ? dc
            .GetExploits()
            .Select(kvp => kvp.Value)
            .Where(e => e > 10)
            .ToList()
            .Concat(new List<int>{dc.GetExploit(a.GetOwner()), 0})
            .Max(e => e) : dc.GetExploit(a.GetOwner());
        Debug.Log("Exploit Strength: " + exploit.ToString());

        if (exploit > 0) 
            offenceScore = (100*offenceScore + 2*exploit) / (100 + 2*exploit);
        if (dc.GetRecord().Contains(m.GetTime())) 
            offenceScore /= 2;
        if (!(m.GetMalwareType() == "trojan")) 
            offenceScore /= (float) dc.GetFirewall() * 3f + 1f;

        if (offenceScore < r) {
            Debug.Log("Attack Failed - " + offenceScore + " < " + r);
            string title = "Attack " + a.GetId() + " Failed :(";
            string body = "Your attack against data center " + dc.GetOwner() + " has failed.";
            notificationManager.AddNotification(new Notification(title, body, a.GetOwner()));
            FinishAttack(a, dc);
            return;
        } else {
            Debug.Log("Attack Successful - " + offenceScore + " > " + r);
            Infect(a, dc);
        }
    }

    public void Infect(Attack a, DataCenter dc) {
        Malware m = malwareController.GetMalware(a.GetMalware());
        string title1 = "", 
            body1 = "", 
            title2 = "", 
            body2 = "";
        bool notify = false;
        if (m.GetMalwareType() == "botnet" || m.GetMalwareType() == "adware" || m.GetMalwareType() == "ransomware") {
            dc.AddAttack(a.GetId());
            return;
        }
        
        int[] attr = m.GetAttributes();
        attr[3] -= (int) (attr[3] * 0.15f * dc.GetDLP());

        Player attacker = PlayerManager.GetPlayer(a.GetOwner());
        Player defender = PlayerManager.GetPlayer(dc.GetOwner());

        switch(a.GetObjective()) {
            case "money":
                int amount = Math.Min(Math.Max(attr[3] * defender.GetMoney() / 200, attr[3] / 2), defender.GetMoney());
                attacker.SetMoney(attacker.GetMoney() + amount);
                defender.SetMoney(defender.GetMoney() - amount);
                title1 = "You have Stolen Money from another Player :D";
                body1 = "Attack " + a.GetId() + " was successful and you have gained $" + amount + " from player " + (dc.GetOwner()+1);
                title2 = "A Player has Stolen Money from you :(";
                body2 = "One of your data centers has been attacked by another player. They stole $" + amount + " from you.";
                notify = true;
                break;

            case "research":// get random research from owner of dc and set true on attacker. better intrusion = better goals
                //requires goals
                break;

            case "sabotage"://implement intrusion. keep goin until intrusion is 0
                // customize notification to stage what was sabotaged
                List<Workable> works = new List<Workable>();

                works.AddRange(malwareController.GetMalware().Values
                    .Where(w => w.GetOwner() == dc.GetOwner())
                    .Where(w => w.GetWorkResources() > 0)
                    .ToList());
                works.AddRange(attackManager.GetAttacks().Values
                    .Where(w => w.GetOwner() == dc.GetOwner())
                    .Where(w => w.GetWorkResources() > 0)
                    .ToList());
                works.AddRange(dataCenterManager.GetDataCenters()
                    .Where(w => w.GetOwner() == dc.GetOwner())
                    .Where(w => w.GetWorkResources() > 0)
                    .ToList());

                System.Random rand = new System.Random();
                int i = rand.Next(0, works.Count);

                works[i].SetWorkResources(0);

                title1 = "You sabotaged another Player's Project :D";
                body1 = "Attack " + a.GetId() + " was successful and you have set the resources spent on another player's projects to zero.";
                title2 = "ON eof your project was sabotaged :()";
                body2 = "Another player has Sabotaged one of your projects.";
                notify = true;

                break;
            
            case "disable":
                dc.SetActive(attr[3] / 25 + 1);
                title1 = "You disabled another Player's Data Center";
                body1 = "Attack " + a.GetId() + " was successful and you have disabled data center " + dc.GetId() + ".";
                title2 = "Your Data Center was Disabled :(";
                body2 = "Another player has disabled data center " + dc.GetId() + " which you own.";
                notify = true;
                break;

            case "backdoor":
                dc.AddExploit(a.GetOwner(), 80 * attr[3] / 100 + 20);
                title1 = "You Created a Backdoor";
                body1 = "Attack " + a.GetId() + " was successful and you created a backdoor into data center " + dc.GetId() + ". The strength of the backdoor is " + (80 * attr[3] / 100 + 20) + ".";
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
                Process(a, dataCenters[idx]);
            } else {
                notificationManager.AddNotification(title1, body1, a.GetOwner());
                if (notify) notificationManager.AddNotification(title2, body2, dc.GetOwner());
                FinishAttack(a, dc);
            }
        } else {
            notificationManager.AddNotification(title1, body1, a.GetOwner());
            if (notify) notificationManager.AddNotification(title2, body2, dc.GetOwner());
            FinishAttack(a, dc);
        }
    }

    private void FinishAttack(Attack a, DataCenter dc) {
        Malware m = malwareController.GetMalware(a.GetMalware());
        if (!m.HasFeature(MalwareFeature.Polymorphism))
            dc.AddRecord(m.GetTime());
        a.Reset();
    }
}
