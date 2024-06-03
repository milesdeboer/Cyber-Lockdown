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

    public AttackWrapper(Attack attack) {
        aid = attack.GetId();
        owner = attack.GetOwner();
        target = attack.GetTarget();
        malware = attack.GetMalware();
        objective = attack.GetObjective();
        delivery = attack.GetDelivery();
        exploit = attack.GetExploit();
    }

    public Attack Unwrap() {
        Attack attack = new Attack(aid);

        attack.SetOwner(owner);
        attack.SetTarget(target);
        attack.SetMalware(malware);
        attack.SetObjective(objective);
        attack.SetDelivery(delivery);
        attack.SetExploit(exploit);

        return attack;
    }
}
