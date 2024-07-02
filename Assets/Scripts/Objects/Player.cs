using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int id = 0;
    private int money = 0;
    private int overallResources;
    private int availableResources;

    private string name;


    public Player(int id) {
        this.id = id;
        overallResources = 100;
        availableResources = 100;
    }

    public int GetId() {
        return id;
    }

    public int GetMoney() {
        return money;
    }

    public void SetMoney(int money) {
        this.money = money;
    }

    public int GetOverallResources() {
        return overallResources;
    }

    public void SetOverallResources(int overallResources) {
        this.overallResources = overallResources;
    }

    public int GetAvailableResources() {
        return availableResources;
    }

    public void SetAvailableResources(int availableResources) {
        this.availableResources = availableResources;
    }

    public string GetName() {
        return name;
    }
    public void SetName(string name) {
        this.name = name;
    }

    public PlayerWrapper Wrap() {
        return new PlayerWrapper(this);
    }
}
