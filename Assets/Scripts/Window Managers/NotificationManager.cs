using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    private List<Notification> notifications;

    [SerializeField]
    private GameObject notificationEntry;
    private Vector2[] positions = {
        new Vector2(42f, 120f),
        new Vector2(42f, 94f),
        new Vector2(42f, 68f),
        new Vector2(42f, 42f)
    };

    public void OnClick(Notification n) {
        notifications.Remove(n);
        UpdateDisplay();
    }

    public void UpdateDisplay() {
        GameObject.FindGameObjectsWithTag("Notification").ToList().ForEach(o => Destroy(o));
        int i = 0;
        int p = gameManager.GetTurnPlayer();
        notifications
            .Where(n => n.GetOwner() == p)
            .Take(4)
            .ToList()
            .ForEach(n => {
                GameObject nObject = Instantiate(notificationEntry, positions[i], Quaternion.identity);
                nObject.transform.SetParent(this.gameObject.transform, false);
                nObject.GetComponent<RectTransform>().localPosition.Set(positions[i].x, positions[i].y, 0);
                nObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(delegate {
                    OnClick(n);
                });
                nObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().SetText(n.GetTitle());
                nObject.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().SetText(n.GetBody());
                i++;
            });
    }

    public List<Notification> GetNotifications() {
        return notifications;
    }
    public void SetNotifications(List<Notification> notifications) {
        this.notifications = notifications;
    }
    public void AddNotification(Notification n) {
        notifications.Add(n);
    }
    public void AddNotification(string title, string body, int owner) {
        notifications.Add(new Notification(title, body, owner));
    }

    public void Save() {
        NotificationDAO dao = new NotificationDAO();
        dao.Save(this);
    }
    public void Load() {
        NotificationDAO dao = new NotificationDAO();
        if (!dao.Load(this)) notifications = new List<Notification>();
        UpdateDisplay();
    }
}