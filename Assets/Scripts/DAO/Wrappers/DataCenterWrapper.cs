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

        attacks = dataCenter.GetAttacks().ToArray();
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
        output.SetAttacks(attacks.ToList());

        return output;
    }
}
