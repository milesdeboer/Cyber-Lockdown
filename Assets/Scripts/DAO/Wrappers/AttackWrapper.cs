using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackWrapper
{
    public int i;//i
    public int o;//o
    public int t;//t
    public int m;//m
    public string obj;//obj
    public string del;//del
    public int exp;//exp

    public int cur = 0;//cur
    public int req = 100;//req
    public int rat = 0;//rat

    public AttackWrapper(Attack attack) {
        i = attack.GetId();
        o = attack.GetOwner();
        t = attack.GetTarget();
        m = attack.GetMalware();
        obj = attack.GetObjective();
        del = attack.GetDelivery();
        exp = attack.GetExploit();
        cur = attack.GetWorkResources();
        req = attack.GetWorkRequirement();
        rat = attack.GetWorkRate();
    }

    public Attack Unwrap() {
        Attack attack = new Attack(i);

        attack.SetOwner(o);
        attack.SetTarget(t);
        attack.SetMalware(m);
        attack.SetObjective(obj);
        attack.SetDelivery(del);
        attack.SetExploit(exp);
        attack.SetWorkResources(cur);
        attack.SetWorkRequirement(req);
        attack.SetWorkRate(rat);

        return attack;
    }
}
