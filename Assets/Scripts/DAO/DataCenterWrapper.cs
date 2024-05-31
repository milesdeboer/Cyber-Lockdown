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
        owner = (dataCenter.GetOwner() == null) ? -1 : dataCenter.GetOwner().GetId();

        emailFilter = dataCenter.GetEmailFilter();
        dlp = dataCenter.GetDLP();
        structure = dataCenter.GetHiddenStructure();
        encryption = dataCenter.GetEncryption();
        ids = dataCenter.GetIDS();
        ips = dataCenter.GetIPS();

        firewall = dataCenter.GetFirewall();

        attacks = dataCenter.GetAttacks().Select(a => a.GetId()).ToArray();
    }   
}
