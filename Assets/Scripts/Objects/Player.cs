using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int id = 0;


    public Player(int id) {
        this.id = id;
    }

    public int GetId() {
        return id;
    }
}
