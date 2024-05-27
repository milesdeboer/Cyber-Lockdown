using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int VALUE_SCALE = 100;
    public static int DATA_CENTERS_PER_PLAYER = 3;
    
    [SerializeField]
    private int numPlayers = 2;

    /**
     *  Returns the number of players in the game.
     *  @returns {int} - The number of players in the game.
     */
    public int GetNumPlayers() {
        return numPlayers;
    }
}
