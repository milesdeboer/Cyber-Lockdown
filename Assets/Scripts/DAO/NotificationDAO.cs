using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class NotificationDAO
{
    public NotificationWrapper[] notifications;

    public void Save(NotificationManager manager) {
        notifications = manager.GetNotifications().Select(n => n.Wrap()).ToArray();

        string json = JsonUtility.ToJson(this, GameManager.READABLE_SAVE);
        File.WriteAllText(Application.persistentDataPath + "/notificationsave.json", json);
    }

    public bool Load(NotificationManager manager) {
        if(File.Exists(Application.persistentDataPath + "/notificationsave.json")) {
            string json = File.ReadAllText(Application.persistentDataPath + "/notificationsave.json");
            NotificationDAO temp = JsonUtility.FromJson<NotificationDAO>(json);

            manager.SetNotifications(temp.notifications.ToList().Select(n => n.Unwrap()).ToList());
            Debug.Log(manager.GetNotifications().Count);
            return true;
        } else return false;
    }
}
