using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Email : Notification
{
    private int dataCenter;
    private int attack;

    public Email(int owner, int dataCenter, int attack) : base(owner) {
        this.attack = attack;
        this.dataCenter = dataCenter;
    }

    public int GetDataCenter() {
        return dataCenter;
    }
    public void SetDataCenter(int dataCenter) {
        this.dataCenter = dataCenter;
    }

    public int GetAttack() {
        return attack;
    }
    public void SetAttack(int attack) {
        this.attack = attack;
    }

}
