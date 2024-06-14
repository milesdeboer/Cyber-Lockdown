using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCenter
{
    public static int BASE_DATA_CENTER_MONEY = 100;
    public static int BASE_DATA_CENTER_RESOURCES = 50;

    private int id = -1;

    private int owner = -1;// change to id

    private int active = 0;

    private int emailFilter = 0;
    private int dlp = 0;
    private int hiddenStructure = 0;
    private int encryption = 0;

    private int ids = 0;
    private int ips = 0;

    private double firewall = 0;    

    private HashSet<int> attacks;
    private HashSet<int> phishes;

    private int money = BASE_DATA_CENTER_MONEY;
    private int resources = BASE_DATA_CENTER_RESOURCES;

    private Dictionary<int, int> exploits;

    private HashSet<DateTime> record;

    private GameObject[] emails;
    private int[] malMail;//!!! change to int[]

    private GameObject[] traffic;
    private int[] malTraffic;//!!! change to int[] - See Notes @ 2024-05-31

    public DataCenter(int id) {
        this.id = id;
        attacks = new HashSet<int>();
        phishes = new HashSet<int>();
        exploits = new Dictionary<int, int>();
        record = new HashSet<DateTime>();
    }

    public int GetOwner() {
        return owner;
    }

    public void SetOwner(int owner) {
        this.owner = owner;
    }

    public int GetMoney() {
        return money;
    }

    public void SetMoney(int money) {
        this.money = money;
    }

    public int GetResources() {
        return resources;
    }
    public void SetResources(int resources) {
        this.resources = resources;
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

    public HashSet<int> GetAttacks() {
        return attacks;
    }

    public void AddAttack(int attack) {
        attacks.Add(attack);
    }

    public void SetAttacks(HashSet<int> attacks) {
        this.attacks = attacks;
    }

    public HashSet<int> GetPhishes() {
        return phishes;
    }

    public void AddPhish(int phish) {
        phishes.Add(phish);
    }
    public void SetPhishes(HashSet<int> phishes) {
        this.phishes = phishes;
    }

    public GameObject[] GetEmails() {
        return emails;
    }
    public void SetEmails(GameObject[] emails) {
        this.emails = emails;
    }

    public int[] GetMalMail() {
        return malMail;
    }

    public Dictionary<int, int> GetExploits() {
        return exploits;
    }
    public void SetExploits(Dictionary<int, int> exploits) {
        this.exploits = exploits;
    }

    public void AddExploit(int pid, int amount) {
        if (exploits.ContainsKey(pid)) exploits[pid] = Math.Max(exploits[pid], amount);
        else exploits.Add(pid, amount);
    }

    public int GetExploit(int pid) {

        return exploits.ContainsKey(pid) ? exploits[pid] : 0;
    }
    public void AddRecord(DateTime dt) {
        record.Add(dt);
    }

    public HashSet<DateTime> GetRecord() {
        return record;
    }
    public void SetRecord(HashSet<DateTime> record) {
        this.record = record;
    }

    public void SetMalMail(int[] malMail) {
        this.malMail = malMail;
    }

    public GameObject[] GetTraffic() {
        return traffic;
    }
    public void SetTraffic(GameObject[] traffic) {
        this.traffic = traffic;
    }

    public int[] GetMalTraffic() {
        return malTraffic;
    }

    public void SetMalTraffic(int[] malTraffic) {
        this.malTraffic = malTraffic;
    }

    public bool IsActive() {
        return active == 0;
    }

    public int GetActive() {
        return active;
    }

    public void SetActive(int active) {
        this.active = active;
    }

    public int GetMoneyProduction() {
        return IsActive() ? money : 0;
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
