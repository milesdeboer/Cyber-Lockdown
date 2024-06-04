using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int id = 0;
    private int money = 0;
    private int overallResources = 100;
    private int availableResources = 100;


    public Player(int id) {
        this.id = id;
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

    public PlayerWrapper Wrap() {
        return new PlayerWrapper(this);
    }
}
