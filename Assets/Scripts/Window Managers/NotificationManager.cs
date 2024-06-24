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
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private ConflictManager conflictManager;
    [SerializeField]
    private AttackManager attackManager;
    [SerializeField]
    private DataCenterManager dataCenterManager;

    private List<Notification> notifications;

    [SerializeField]
    private GameObject notificationEntry;
    [SerializeField]
    private GameObject emailEntry;

    private Vector2[] positions = {
        new Vector2(42f, 120f),
        new Vector2(42f, 94f),
        new Vector2(42f, 68f),
        new Vector2(42f, 42f)
    };

    public void OnClick(GameObject self, Notification n, bool accepted) {
        if (accepted) {
            int aid;
            if (Int32.TryParse(self.name, out aid)) {
                DataCenter dc = dataCenterManager.GetDataCenter(((Email) n).GetDataCenter());
                Attack phish = attackManager.GetAttack(aid);
                conflictManager.Infect(phish, dc);
                playerManager.UpdateDisplay();
                dc.GetPhishes().Remove(phish.GetId());
            }
        }
        notifications.Remove(n);
        UpdateDisplay();
    }

    public void ClearClick() {
        notifications
            .Where(n => n.GetOwner() == gameManager.GetTurnPlayer())
            .ToList()
            .ForEach(n => notifications.Remove(n));
    }

    public void CreateEmails() {
        dataCenterManager
            .GetDataCenters()
            .Where(dc => dc.GetOwner() == gameManager.GetTurnPlayer())
            //.Where(dc => dc.GetPhishes().Count > 0)
            .ToList()
            .ForEach(dc => AddNotification(new Email(dc.GetOwner(), dc.GetId(), (dc.GetPhishes().Count > 0) ? dc.GetPhishes().Single() : -1)));
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
                GameObject nObject = Instantiate((n is Email) ? emailEntry : notificationEntry, positions[i], Quaternion.identity);
                nObject.transform.SetParent(this.gameObject.transform, false);
                nObject.GetComponent<RectTransform>().localPosition.Set(positions[i].x, positions[i].y, 0);
                nObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(delegate {
                    OnClick(nObject, n, false);
                });
                nObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().SetText(n.GetTitle());
                nObject.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().SetText(n.GetBody());
                if (n is Email) {
                    nObject.transform.GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(delegate {
                        OnClick(nObject, n, true);
                    });
                    if (((Email) n).GetAttack() != -1) nObject.name = ((Email) n).GetAttack().ToString();
                }
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
        CreateEmails();
        UpdateDisplay();
    }
}