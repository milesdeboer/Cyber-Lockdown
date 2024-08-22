using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWrapper
{
    public int i;
    public int m;
    public int or;// overall resources
    public int ar;// available resources

    public string n;

    public int[] u;
    public int rat;
    public int tar;//work target

    public PlayerWrapper(Player player) {
        i = player.GetId();
        m = player.GetMoney();
        or = player.GetOverallResources();
        ar = player.GetAvailableResources();
        n = player.GetName();
        u = player.GetUnlocks();
        rat = player.GetWorkRate();
        tar = player.GetWorkTarget();
    }

    public Player Unwrap() {
        Player player = new Player(i);
        player.SetMoney(m);
        player.SetOverallResources(or);
        player.SetAvailableResources(ar);
        player.SetName(n);
        player.SetUnlocks(u);
        player.SetWorkRate(rat);
        player.SetWorkTarget(tar);

        return player;
    }
}
