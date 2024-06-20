using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackWrapper
{
    public int aid;
    public int owner;
    public int target;
    public int malware;
    public string objective;
    public string delivery;
    public int exploit;

    public int currentResources = 0;
    public int requiredResources = 100;
    public int resourceRate = 0;

    public AttackWrapper(Attack attack) {
        aid = attack.GetId();
        owner = attack.GetOwner();
        target = attack.GetTarget();
        malware = attack.GetMalware();
        objective = attack.GetObjective();
        delivery = attack.GetDelivery();
        exploit = attack.GetExploit();
        currentResources = attack.GetWorkResources();
        requiredResources = attack.GetWorkRequirement();
        resourceRate = attack.GetWorkRate();
    }

    public Attack Unwrap() {
        Attack attack = new Attack(aid);

        attack.SetOwner(owner);
        attack.SetTarget(target);
        attack.SetMalware(malware);
        attack.SetObjective(objective);
        attack.SetDelivery(delivery);
        attack.SetExploit(exploit);
        attack.SetWorkResources(currentResources);
        attack.SetWorkRequirement(requiredResources);
        attack.SetWorkRate(resourceRate);

        return attack;
    }
}
