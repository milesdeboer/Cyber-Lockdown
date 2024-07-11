using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWrapper
{
    public int pid;
    public int money;
    public int overallResources;
    public int availableResources;

    public string name;

    public int[] unlocks;
    public int workRate;
    public int workTarget;

    public PlayerWrapper(Player player) {
        pid = player.GetId();
        money = player.GetMoney();
        overallResources = player.GetOverallResources();
        availableResources = player.GetAvailableResources();
        name = player.GetName();
        unlocks = player.GetUnlocks();
        workRate = player.GetWorkRate();
        workTarget = player.GetWorkTarget();
    }

    public Player Unwrap() {
        Player player = new Player(pid);
        player.SetMoney(money);
        player.SetOverallResources(overallResources);
        player.SetAvailableResources(availableResources);
        player.SetName(name);
        player.SetUnlocks(unlocks);
        player.SetWorkRate(workRate);
        player.SetWorkTarget(workTarget);

        return player;
    }
}
