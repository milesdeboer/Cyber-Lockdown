using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int VALUE_SCALE = 100;
    public static int DATA_CENTERS_PER_PLAYER = 4;
    
    [SerializeField]
    private int numPlayers = 2;


    public int GetNumPlayers() {
        return numPlayers;
    }
}
