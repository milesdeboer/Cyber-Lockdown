using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                    dc.AddRecord(m.GetTime());
                    Infect(a, dc);
                    return;
                } else {
                    dc.AddRecord(m.GetTime());
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
        float offenceScore = ((attr.Zip(weights, (a,w) => a * w / 100f).Sum() + 0.9f) / 2.1f * 0.9f) + 0.05f;
        // Apply Thresholds
        if (!(m.GetMalwareType() == "rootkit" || m.GetMalwareType() == "trojan")) {
            // Apply IPS Threshold
            if (offenceScore < dc.GetIPS() * 0.15) {
                string title = "Data Center " + dc.GetId() + " Attacked";
                string body = "Player " + a.GetOwner() + " has tried to infect your data center (data center " + dc.GetId() + ") with a " + m.GetMalwareType() + ". This attack was seen and prevented by your Intrusion Detection System and Intrusion Prevention System.";
                notificationManager.AddNotification(new Notification(title, body, dc.GetOwner()));
                dc.AddRecord(m.GetTime());
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
        Debug.Log("Offence2: " + offenceScore);
        int exploit = (m.HasFeature(MalwareFeature.ZeroDayExploit)) ? dc
            .GetExploits()
            .Select(kvp => kvp.Value)
            .Where(e => e > 10)
            .ToList()
            .Concat(new List<int>{dc.GetExploit(a.GetOwner()), 0})
            .Max(e => e) : dc.GetExploit(a.GetOwner());
        Debug.Log("Exploit Strength: " + exploit.ToString());

        if (exploit > 0) 
            offenceScore = (100*offenceScore + 2*((float) exploit)) / (100f + 2*((float) exploit));
        if (dc.GetRecord().Contains(m.GetTime())) 
            offenceScore /= 2f;
        if (!(m.GetMalwareType() == "trojan")) 
            offenceScore /= (float) dc.GetFirewall() * 3f + 1f;

        dc.AddRecord(m.GetTime());

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
        
        //apply encryption and dlp
        int[] attr = m.GetAttributes();
        
        attr[3] = ScaleIntrusion(attr[3], dc.GetEncryption(), dc.GetDLP());
        Debug.Log("Before Damage");
        notify = Damage(a, dc, attr[3], out title1, out body1, out title2, out body2);
        Debug.Log("After Damage");

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

    /// <summary>
    /// Scales the intrusion atribute of the attack to a customized function
    /// </summary>
    /// <param name="intrusion">the initial intrusion attribute [0, 100]</param>
    /// <param name="encryption">the encryption level of the target data center [0, 5]</param>
    /// <param name="dlp">the data loss prevension level of the target data center [0, 5]</param>
    /// <returns>The scaled intrusion value</returns>
    public int ScaleIntrusion(int intrusion, int encryption, int dlp) {
        return (int) (10f + 0.9f * (Math.Min(intrusion, 1f/8f*Math.Pow(dlp,5) - 5f/3f*Math.Pow(dlp,4) + 55f/8f*Math.Pow(dlp,3) - 35f/6f*Math.Pow(dlp,2) - 59f/2f*dlp + 100f)) * 
            (5/24*Math.Pow(encryption,5f) - 35f/12f*Math.Pow(encryption,4f) + 335f/24f*Math.Pow(encryption,3) - 295f/12f*Math.Pow(encryption,2) - 20f/3f*encryption + 100f) / 100f);
    }

    /// <summary>
    /// Inflicts damage upon data center given the attributes of the attack and creates title and body for notification(s).
    /// </summary>
    /// <param name="a">The Attack Object involved in the interaction</param>
    /// <param name="dc">The DataCenter Object involved in the interaction</param>
    /// <param name="intrusion">The scaled intrusion value from the malware associated with the attack.</param>
    /// <param name="t1">out: The title of the first notification to be sent.</param>
    /// <param name="b1">out: The body of the first notification to be sent.</param>
    /// <param name="t2">out: The title of the second notification to be sent.</param>
    /// <param name="b2">out: The body of the second notification to be sent.</param>
    /// <returns>true if two notifications are to be sent and false otherwise.</returns>
    public bool Damage(Attack a, DataCenter dc, int intrusion, out string t1, out string b1, out string t2, out string b2) {
        Player attacker = PlayerManager.GetPlayer(a.GetOwner());
        Player defender = PlayerManager.GetPlayer(dc.GetOwner());

        t1 = ""; b1 = ""; t2 = ""; b2 = "";

        Debug.Log("During Damage");
        
        switch(a.GetObjective()) {
            case "money":
                int amount = Math.Min(Math.Max(intrusion * defender.GetMoney() / 200, intrusion / 2), defender.GetMoney());
                attacker.SetMoney(attacker.GetMoney() + amount);
                defender.SetMoney(defender.GetMoney() - amount);

                t1 = "You have Stolen Money from another Player :D";
                b1 = "Attack " + a.GetId() + " was successful and you have gained $" + amount + " from player " + (dc.GetOwner()+1);
                t2 = "A Player has Stolen Money from you :(";
                b2 = "One of your data centers has been attacked by another player. They stole $" + amount + " from you.";
                return true;
            
            case "research":
                int i = -1;
                int j = 0;
                List<int> idx = new List<int>();
                List<int> options = attacker.GetUnlocks()
                    .ToList()
                    .Select(u => {
                        i++;
                        return defender.GetUnlocks()[i] - u;
                    })
                    .Select(u => Math.Max(u, 0))
                    .Select(u => {
                        if (u > 0) idx.Add(j);
                        j++;
                        return (int) (u * ((float) intrusion / 100f));
                    })
                    .ToList();

                t1 = "You have stolen research from another player :D";
                b1 = "Attack " + a.GetId() + " was successful and you have gained " + options[idx[i]] + " research points in towards a category";

                System.Random random = new System.Random();
                i = random.Next(0, idx.Count);
                attacker.SetUnlock(i, options[idx[i]]);

                return false;

            case "sabotage":
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
                int id = rand.Next(0, works.Count);

                works[id].SetWorkResources(Math.Max(works[id].GetWorkResources() - (intrusion * works[id].GetWorkRequirement()) / 100, 0));

                t1 = "You sabotaged another Player's Project :D";
                b1 = "Attack " + a.GetId() + " was successful and you have set the resources spent on another player's projects to zero.";
                t2 = "One of your projects was sabotaged :(";
                b2 = "Another player has Sabotaged one of your projects.";

                return true;

            case "disable":
                dc.SetActive(intrusion / 25 + 1);
                t1 = "You disabled another Player's Data Center";
                b1 = "Attack " + a.GetId() + " was successful and you have disabled data center " + dc.GetId() + ".";
                t2 = "Your Data Center was Disabled :(";
                b2 = "Another player has disabled data center " + dc.GetId() + " which you own.";
                return true;

            case "backdoor":
                dc.AddExploit(attacker.GetId(), 80 * intrusion / 100 + 20);
                t1 = "You Created a Backdoor";
                b1 = "Attack " + a.GetId() + " was successful and you created a backdoor into data center " + dc.GetId() + ". The strength of the backdoor is " + (80 * intrusion / 100 + 20) + ".";
                return false;

            default:
                Debug.Log("Invalid Objective");
                return false;
        }
    }

    private void FinishAttack(Attack a, DataCenter dc) {
        Malware m = malwareController.GetMalware(a.GetMalware());
        if (!m.HasFeature(MalwareFeature.Polymorphism))
        a.Reset();
    }
}
