using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification
{
    private static Dictionary<int, int> counts = new Dictionary<int, int>();

    private int nid;
    private int owner;
    private string title;
    private string body;

    public Notification(string title, string body, int owner) {
        this.title = title;
        this.body = body;
        this.owner = owner;
        counts[owner] = (counts.ContainsKey(owner)) ? counts[owner] + 1 : 1;
        nid = 1000 * (owner+1) + counts[owner];
    }

    public Notification(int owner) {
        this.owner = owner;
        counts[owner] = (counts.ContainsKey(owner)) ? counts[owner] + 1 : 1;
        nid = 1000 * (owner+1) + counts[owner];
    }

    public int GetId() {
        return nid;
    }
    public void SetId(int nid) {
        this.nid = nid;
    }

    public int GetOwner() {
        return owner;
    }
    public void SetOwner(int owner) {
        this.owner = owner;
    }

    public string GetTitle() {
        return title;
    }
    public void SetTitle(string title) {
        this.title = title;
    }

    public string GetBody() {
        return body;
    }
    public void SetBody(string body) {
        this.body = body;
    }  

    public NotificationWrapper Wrap() {
        return new NotificationWrapper(this);
    }
}
