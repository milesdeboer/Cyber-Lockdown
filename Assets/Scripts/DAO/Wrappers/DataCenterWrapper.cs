using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DataCenterWrapper
{
    public int did;
    public int owner;
    public int emailFilter;
    public int dlp;
    public int structure;
    public int encryption;
    public int ids;
    public int ips;
    public double firewall;

    public int[] attacks;
    public int[] phishes;

    public int active;
    public int money;
    public int resources;
    public List<int> exploits = new List<int>();
    public List<string> record = new List<string>();

    public bool scanning = false;
    public bool patching = false;
    public int[] patches;

    public string target;
    public int workResources;
    public int workRequirement;
    public int workRate;

    public DataCenterWrapper(DataCenter dataCenter) {
        did = dataCenter.GetId();
        owner = dataCenter.GetOwner();

        emailFilter = dataCenter.GetEmailFilter();
        dlp = dataCenter.GetDLP();
        structure = dataCenter.GetHiddenStructure();
        encryption = dataCenter.GetEncryption();
        ids = dataCenter.GetIDS();
        ips = dataCenter.GetIPS();

        firewall = dataCenter.GetFirewall();

        attacks = new int[dataCenter.GetAttacks().Count];
        dataCenter.GetAttacks().CopyTo(attacks);
        phishes = new int[dataCenter.GetPhishes().Count];
        dataCenter.GetPhishes().CopyTo(phishes);

        active = dataCenter.GetActive();
        money = dataCenter.GetMoney();
        resources = dataCenter.GetResources();

        foreach(KeyValuePair<int, int> pair in dataCenter.GetExploits()) {
            exploits.Add(pair.Key);
            exploits.Add(pair.Value);
        }
        foreach(DateTime time in dataCenter.GetRecord())
            record.Add(time.ToString());

        target = dataCenter.GetWorkTarget();
        workResources = dataCenter.GetWorkResources();
        workRequirement = dataCenter.GetWorkRequirement();
        workRate = dataCenter.GetWorkRate();

        scanning = dataCenter.IsScanning();
        patching = dataCenter.IsPatching();
        patches = dataCenter.GetPatchQueue().ToArray();
    }   

    public DataCenter Unwrap() {
        DataCenter output = new DataCenter(did);

        output.SetOwner(owner);
        output.SetEmailFilter(emailFilter);
        output.SetDLP(dlp);
        output.SetHiddenStructure(structure);
        output.SetEncryption(encryption);
        output.SetIDS(ids);
        output.SetIPS(ips);
        output.SetFirewall(firewall);
        output.SetAttacks(new HashSet<int>(attacks.ToList()));
        output.SetPhishes(new HashSet<int>(phishes.ToList()));

        output.SetActive(active);
        output.SetMoney(money);
        output.SetResources(resources);

        Dictionary<int, int> exploits_ = new Dictionary<int, int>();
        for (int i = 0; i < exploits.Count; i+=2)
            exploits_.Add(exploits[i], exploits[i+1]);
        output.SetExploits(exploits_);

        HashSet<DateTime> record_ = new HashSet<DateTime>();
        foreach(string time in record)
            record_.Add(DateTime.Parse(time));
        output.SetRecord(record_);

        output.SetWorkTarget(target);
        output.SetWorkResources(workResources);
        output.SetWorkRequirement(workRequirement);
        output.SetWorkRate(workRate);

        output.EnableScan(scanning);
        output.EnablePatch(patching);
        output.SetPatchQueue(new Queue<int>(patches.ToList()));

        return output;
    }
}
