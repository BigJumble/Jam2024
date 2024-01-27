using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class MessageHandler : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        MessageQueue.OnMessage(e);
    }
}

public class MessageQueue
{
    private static MessageQueue _instance;

    private List<MessageReceiver> receivers;

    private MessageQueue()
    {
        receivers = new List<MessageReceiver>();
    }

    public static MessageQueue Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MessageQueue();
            }
            return _instance;
        }
    }

    public static void OnMessage(MessageEventArgs e)
    {
        foreach (var receiver in Instance.receivers)
        {
            receiver.OnMessage(e);
        }
    }

    public static void Subscribe(MessageReceiver messageReceiver)
    {
        Instance.receivers.Add(messageReceiver);
    }
}

public interface MessageReceiver
{
    public void OnMessage(MessageEventArgs e);
}


public class Server : MonoBehaviour
{
    private WebSocketServer wssv;

    private void Start()
    {
        wssv = new WebSocketServer(4649);

        wssv.AddWebSocketService<MessageHandler>("/Echo");
        wssv.Start();
        if (wssv.IsListening)
        {
            Debug.Log("Listening on port " + wssv.Port + ", and providing WebSocket services: ");

            foreach (var path in wssv.WebSocketServices.Paths)
                Debug.Log("- " + path);
        }
    }

    private void OnDestroy()
    {
        if (wssv != null && wssv.IsListening)
        {
            wssv.Stop();
            Debug.Log("WebSocket Server stopped");
        }
    }
}
