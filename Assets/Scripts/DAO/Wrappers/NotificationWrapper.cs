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

    public NotificationWrapper(Notification notification) {
        nid = notification.GetId();
        owner = notification.GetOwner();
        title = notification.GetTitle();
        body = notification.GetBody();
    }

    public Notification Unwrap() {
        Notification notification = new Notification(title, body, owner);
        notification.SetId(nid);
        return notification;
    }
}
