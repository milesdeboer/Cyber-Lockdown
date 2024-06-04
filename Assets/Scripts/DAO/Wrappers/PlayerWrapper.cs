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

    public PlayerWrapper(Player player) {
        pid = player.GetId();
        money = player.GetMoney();
        overallResources = player.GetOverallResources();
        availableResources = player.GetAvailableResources();
        Debug.Log("Player: [" + pid + ", " + availableResources);
    }

    public Player Unwrap() {
        Player player = new Player(pid);
        player.SetMoney(money);
        player.SetOverallResources(overallResources);
        player.SetAvailableResources(availableResources);

        Debug.Log("Resources: " + pid + "-" + player.GetAvailableResources() + "/" + player.GetOverallResources());

        return player;
    }
}
