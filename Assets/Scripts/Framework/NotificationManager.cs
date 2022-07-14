using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager
{
    private static Dictionary<IListener, Notification[]> listeners = new Dictionary<IListener, Notification[]>();

    public static void AddListener(IListener pListener, params Notification[] pNotifications)
    {
        listeners.Add(pListener, pNotifications);
    }

    public static void RemoveListener(IListener pListener)
    {
        if (listeners.ContainsKey(pListener))
            listeners.Remove(pListener);
    }

    public static void SendNotification(Notification pNotification, object pData = null)
    {
        foreach (IListener listener in listeners.Keys)
        {
            Notification[] notifications = listeners[listener];
            for (int i = 0; i < notifications.Length; i++) {
                if (notifications[i] == pNotification) {
                    listener.NotificationReceived(pNotification, pData);
                }
            }
        }
    }
}

public interface IListener
{
    void NotificationReceived(Notification pNotification, object pData);
}
