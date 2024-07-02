using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager
{
    private static int playerNum;
    private static int turnNum;

    public static int GetPlayerNum() {
        return playerNum;
    }
    public static void SetPlayerNum(int newPlayerNum) {
        playerNum = newPlayerNum;
    }

    public static int GetTurnNum() {
        return turnNum;
    }

    public static void SetTurnNum(int newTurnNum) {
        turnNum = newTurnNum;
    }
}
