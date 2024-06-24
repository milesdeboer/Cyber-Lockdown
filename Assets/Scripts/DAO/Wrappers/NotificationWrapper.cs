using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NotificationWrapper
{
    public int nid;
    public int owner;
    public string title;
    public string body;

    public int dataCenter = -1;
    public int attack = -1;

    public NotificationWrapper(Notification notification) {
        nid = notification.GetId();
        owner = notification.GetOwner();
        title = notification.GetTitle();
        body = notification.GetBody();
        if (notification is Email) {
            dataCenter = ((Email) notification).GetDataCenter();
            attack = ((Email) notification).GetAttack();
        }
    }

    public Notification Unwrap() {
        Notification notification = new Notification(title, body, owner);
        notification.SetId(nid);
        if (notification is Email) {
            ((Email) notification).SetDataCenter(dataCenter);
            ((Email) notification).SetAttack(attack);
        }
        return notification;
    }
}
