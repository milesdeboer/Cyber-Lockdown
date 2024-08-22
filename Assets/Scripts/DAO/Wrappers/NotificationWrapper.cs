using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NotificationWrapper
{
    public int i;
    public int o;
    public string t;
    public string b;

    public int dc = -1;
    public int a = -1;

    public NotificationWrapper(Notification notification) {
        i = notification.GetId();
        o = notification.GetOwner();
        t = notification.GetTitle();
        b = notification.GetBody();
        if (notification is Email) {
            dc = ((Email) notification).GetDataCenter();
            a = ((Email) notification).GetAttack();
        }
    }

    public Notification Unwrap() {
        Notification notification;
        if (dc > -1) {
            notification = new Email(o, dc, a);
            notification.SetTitle(t);
            notification.SetBody(b);
            ((Email) notification).SetDataCenter(dc);
            ((Email) notification).SetAttack(a);

        } else {
            notification = new Notification(t, b, o);
        }
        notification.SetId(i);
        
        return notification;
    }
}
