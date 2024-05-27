using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
    private int id = -1;

    private Malware malware;
    private string objective;
    private string delivery;
    private int exploit;

    public Attack(int id) {
        this.id = id;
    }

    public int GetId() {
        return id;
    }

    public Malware GetMalware() {
        return malware;
    }
    public void SetMalware(Malware malware) {
        this.malware = malware;
    }

    public string GetObjective() {
        return objective;
    }
    public void SetObjective(string objective) {
        this.objective = objective;
    }

    public string GetDelivery() {
        return delivery;
    }
    public void SetDelivery(string delivery) {
        this.delivery = delivery;
    }

    public int GetExploit() {
        return exploit;
    }
    public void SetExploit(int exploit) {
        this.exploit = exploit;
    }
}
