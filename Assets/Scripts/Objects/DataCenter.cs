using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCenter
{
    private int id = -1;

    private Player owner;

    private int emailFilter = 0;
    private int dlp = 0;
    private int hiddenStructure = 0;
    private int encryption = 0;

    private int ids = 0;
    private int ips = 0;

    private double firewall = 0;    

    private List<Attack> attacks;

    private GameObject[] emails;
    private Attack[] malMail;

    private GameObject[] traffic;
    private Attack[] malTraffic;

    public DataCenter(int id) {
        this.id = id;
        attacks = new List<Attack>();
    }

    public Player GetOwner() {
        return owner;
    }

    public void SetOwner(Player owner) {
        this.owner = owner;
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

    public double GetFirewall() {
        return firewall;
    }
    public void SetFirewall(double firewall) {
        this.firewall = firewall;
    }

    public List<Attack> GetAttacks() {
        return attacks;
    }

    public GameObject[] GetEmails() {
        return emails;
    }
    public void SetEmails(GameObject[] emails) {
        this.emails = emails;
    }

    public Attack[] GetMalMail() {
        return malMail;
    }

    public void SetMalMail(Attack[] malMail) {
        this.malMail = malMail;
    }

    public GameObject[] GetTraffic() {
        return traffic;
    }
    public void SetTraffic(GameObject[] traffic) {
        this.traffic = traffic;
    }

    public Attack[] GetMalTraffic() {
        return malTraffic;
    }

    public void SetMalTraffic(Attack[] malTraffic) {
        this.malTraffic = malTraffic;
    }

    /**
     *  Returns the identification number of the data center
     */
    public int GetId() {
        return id;
    }

    public DataCenterWrapper Wrap() {
        return new DataCenterWrapper(this);
    }
}
