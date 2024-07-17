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

    private int[] unlocks;
    private int workTarget;
    private int workRequired;
    private int workRate;


    public Player(int id) {
        this.id = id;
        overallResources = 100;
        availableResources = 100;
        unlocks = new int[20];
    }

    public int GetId() {
        return id;
    }

    public int GetMoney() {
        return money;
    }

    public void AddMoney(int money) {
        this.money += money;
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

    public int[] GetUnlocks() {
        return unlocks;
    }

    public void SetUnlocks(int[] unlocks) {
        this.unlocks = unlocks;
    }

    public int GetUnlock(int i) {
        return unlocks[i];
    }

    public void SetUnlock(int i, int unlock) {
        unlocks[i] = unlock;
    }

    public int GetWorkTarget() {
        return workTarget;
    }
    public void SetWorkTarget(int workTarget) {
        this.workTarget = workTarget;
        GoalManager.SetWorkTarget(workTarget);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The work required to complete the current goal</returns>
    public int GetWorkRequired() {
        return workRequired;
    }

    /// <summary>
    /// Sets the work required to complete the current goal.
    /// </summary>
    /// <param name="workRequired">The work required to complete the current goal</param>
    public void SetWorkRequired(int workRequired) {
        this.workRequired = workRequired;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The rate at which work is being done on the current goal</returns>
    public int GetWorkRate() {
        return workRate;
    }
    /// <summary>
    /// Sets the rate at which work is beign done on the current goal
    /// </summary>
    /// <param name="workRate">The rate at which work is being done on the current goal</param>
    public void SetWorkRate(int workRate) {
        this.workRate = workRate;
    }

    /// <summary>
    /// Applies the work rate to the work done to the goal
    /// </summary>
    public void Work() {
        if (GoalManager.GetWorkTarget() >= 0)
            unlocks[GoalManager.GetWorkTarget()] += workRate; 
    }


    /// <summary>
    /// Converts Player to PlayerWrapper
    /// </summary>
    /// <returns>PlayerWrapper holding the data about this player</returns>
    public PlayerWrapper Wrap() {
        return new PlayerWrapper(this);
    }
}
