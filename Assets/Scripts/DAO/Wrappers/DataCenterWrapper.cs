using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DataCenterWrapper
{
    public int i;//i
    public int o;//o
    public int ef;//email filter
    public int d;//dlp
    public int s;//structure
    public int en;//en encryption
    public int id;//ids
    public int ip;//ips
    public double f;//firewall

    public int[] a;//attacks
    public int[] p;//phishes

    public int act;//act - active
    public int mon;//mon - money
    public int r;//r - resources
    public List<int> exp = new List<int>();//exp - exploits
    public List<string> rec = new List<string>();//rec - record

    public bool sc = false;//sc - scanning
    public bool pa = false;//pa - patching
    public int[] pq;//pq - patches

    public string t;//t - target
    public int cur;//cur - current resources
    public int req;//req - required resources
    public int rat;//rat - resource rate

    public DataCenterWrapper(DataCenter dataCenter) {
        i = dataCenter.GetId();
        o = dataCenter.GetOwner();

        ef = dataCenter.GetEmailFilter();
        d = dataCenter.GetDLP();
        s = dataCenter.GetHiddenStructure();
        en = dataCenter.GetEncryption();
        id = dataCenter.GetIDS();
        ip = dataCenter.GetIPS();

        f = dataCenter.GetFirewall();

        a = new int[dataCenter.GetAttacks().Count];
        dataCenter.GetAttacks().CopyTo(a);
        p = new int[dataCenter.GetPhishes().Count];
        dataCenter.GetPhishes().CopyTo(p);

        act = dataCenter.GetActive();
        mon = dataCenter.GetMoney();
        r = dataCenter.GetResources();

        foreach(KeyValuePair<int, int> pair in dataCenter.GetExploits()) {
            exp.Add(pair.Key);
            exp.Add(pair.Value);
        }
        foreach(DateTime time in dataCenter.GetRecord())
            rec.Add(time.ToString());

        t = dataCenter.GetWorkTarget();
        cur = dataCenter.GetWorkResources();
        req = dataCenter.GetWorkRequirement();
        rat = dataCenter.GetWorkRate();

        sc = dataCenter.IsScanning();
        pa = dataCenter.IsPatching();
        pq = dataCenter.GetPatchQueue().ToArray();
    }   

    public DataCenter Unwrap() {
        DataCenter output = new DataCenter(i);

        output.SetOwner(o);
        output.SetEmailFilter(ef);
        output.SetDLP(d);
        output.SetHiddenStructure(s);
        output.SetEncryption(en);
        output.SetIDS(id);
        output.SetIPS(ip);
        output.SetFirewall(f);
        output.SetAttacks(new HashSet<int>(a.ToList()));
        output.SetPhishes(new HashSet<int>(p.ToList()));

        output.SetActive(act);
        output.SetMoney(mon);
        output.SetResources(r);

        Dictionary<int, int> exploits_ = new Dictionary<int, int>();
        for (int i = 0; i < exp.Count; i+=2)
            exploits_.Add(exp[i], exp[i+1]);
        output.SetExploits(exploits_);

        HashSet<DateTime> record_ = new HashSet<DateTime>();
        foreach(string time in rec)
            record_.Add(DateTime.Parse(time));
        output.SetRecord(record_);

        output.SetWorkTarget(t);
        output.SetWorkResources(cur);
        output.SetWorkRequirement(req);
        output.SetWorkRate(rat);

        output.EnableScan(sc);
        output.EnablePatch(pa);
        output.SetPatchQueue(new Queue<int>(pq.ToList()));

        return output;
    }
}
