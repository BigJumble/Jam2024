using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using static UnityEngine.EventSystems.EventTrigger;

public struct PlayerInput
{
    public string PlayerName;
    public string Team;
    public float JoystickX;
    public float JoystickY;
    public int CharacterID;
    public int SoundEffectID;

    // Constructor for convenience
    public PlayerInput(string playerName, string team, float joystickX, float joystickY, int characterID, int soundEffectID)
    {
        PlayerName = playerName;
        Team = team;
        JoystickX = joystickX;
        JoystickY = joystickY;
        CharacterID = characterID;
        SoundEffectID = soundEffectID;
    }
}

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

    private static List<Server> servers;

    private MessageQueue() { }

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
        foreach (var server in servers)
        {
            server.OnMessage(e);
        }
    }

    public static void Subscribe(Server server)
    {
        servers.Add(server);
    }
}


public class Server : MonoBehaviour
{
    public Dictionary<string, PlayerInput> PlayersInput;

    private WebSocketServer wssv;

    private void Awake()
    {
        PlayersInput = new Dictionary<string, PlayerInput>();
        MessageQueue.Subscribe(this);
    }

    private void Start()
    {
        wssv = new WebSocketServer(4649);

        wssv.AddWebSocketService<MessageHandler>("/HandleInput");
        wssv.Start();
        if (wssv.IsListening)
        {
            Debug.Log("Listening on port " + wssv.Port + ", and providing WebSocket services: ");

            foreach (var path in wssv.WebSocketServices.Paths)
                Debug.Log("- " + path);
        }
        InvokeRepeating("logA", 0, 1);
    }

    private void logA()
    {
        Debug.Log(".");
        foreach (var entry in PlayersInput)
        {
            Debug.Log(entry.Key + ": " + entry.Value);
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

    public void OnMessage(MessageEventArgs e)
    {
        Debug.Log("Received: " + e.Data);

        Debug.Log(e.Data); // gets data
                           // data is
                           // "PlayerName|Team|joystick X|joystick Y|CharacterID|SoundEffectID"

        string[] dataParts = e.Data.Split('|');

        if (dataParts.Length == 6)
        {
            string playerName = dataParts[0];

            PlayerInput playerInput = new PlayerInput(
                playerName,
                dataParts[1], // Team
                float.Parse(dataParts[2]), // Joystick X
                float.Parse(dataParts[3]), // Joystick Y
                int.Parse(dataParts[4]), // CharacterID
                int.Parse(dataParts[5]) // SoundEffectID
            );

            PlayersInput[playerName] = playerInput;

            Debug.Log("Stored input for player " + playerName + ": " + playerInput);
        }
        else
        {
            Debug.LogWarning("Invalid data format: " + e.Data);
        }
    }

}

