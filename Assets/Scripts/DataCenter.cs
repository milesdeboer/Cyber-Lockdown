using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCenter
{
    private int id = -1;

    private int emailFilter = 0;
    private int dlp = 0;
    private int hiddenStructure = 0;
    private int encryption = 0;

    private int ids = 0;
    private int ips = 0;

    private int firewall = 0;

    public DataCenter(int id) {
        this.id = id;
    }

    public int GetEmailFilter() {
        return emailFilter;
    }
    public void SetEmailFilter(int emailFilter) {
        this.emailFilter = emailFilter;
    }

    public int GetDLP() {
        return dlp;
    }
    public void SetDLP(int dlp) {
        this.dlp = dlp;
    }

    public int GetHiddenStructure() {
        return hiddenStructure;
    }
    public void SetHiddenStructure(int hiddenStructure) {
        this.hiddenStructure = hiddenStructure;
    }

    public int GetEncryption() {
        return encryption;
    }
    public void SetEncryption(int encryption) {
        this.encryption = encryption;
    }

    public int GetIDS() {
        return ids;
    }
    public void SetIDS(int ids) {
        this.ids = ids;
    }

    public int GetIPS() {
        return ips;
    }
    public void SetIPS(int ips) {
        this.ips = ips;
    }

    public int GetFirewall() {
        return firewall;
    }
    public void SetFirewall(int firewall) {
        this.firewall = firewall;
    }

    /**
     *  Returns the identification number of the data center
     */
    public int GetId() {
        return id;
    }
}
