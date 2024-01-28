using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WebSocketSharp;

public class PlayerManager : MonoBehaviour, MessageReceiver
{
    public enum Team
    {
        A,
        B,
        None,
    }

    public enum Character
    {
        Savickas,
        Bielka,
        Nauseda,
        Audrele
    }

    [SerializeField]
    private List<GameObject> playerPrefabs;

    [SerializeField]
    private Dictionary<string, Player> players;

    private List<Transform> teamASpawnLocations;
    private List<Transform> teamBSpawnLocations;

    private List<Player> teamAPlayers;
    private List<Player> teamBPlayers;

    ConcurrentDictionary<string, Player.Input> state = new ConcurrentDictionary<string, Player.Input>();

    private void Start()
    {
        teamASpawnLocations = GameObject.Find("Team A Spawn Points")
            .transform.GetComponentsInChildren<Transform>().ToList();

        teamBSpawnLocations = GameObject.Find("Team B Spawn Points")
            .transform.GetComponentsInChildren<Transform>().ToList();

        teamAPlayers = new List<Player>();
        teamBPlayers = new List<Player>();

        players = new Dictionary<string, Player>();

        MessageQueue.Subscribe(this);

        InvokeRepeating("UpdateState", 0, 0.05f);
    }

    public void HandleMessage(Player.Input input)
    {
        state.AddOrUpdate(input.uuid, input, (oldKey, oldValue) => input);
    }

    private void UpdateState()
    {
        foreach(var (uuid, input) in state)
        {
            Player p = ResolvePlayer(uuid, (Character)input.characterID);
            SetTeam(p, (Team)input.team);
            p.Touch(input.joystickX, input.joystickY, input.received, input.soundEffectID);
        }
    }

    public void NotifyOfDeath(Player player)
    {
        Debug.Log("[NotifyOfDeath]");

        // TODO: notify globally of removal

        switch (player.team)
        {
            case Team.A:
                teamAPlayers.Remove(player);
                break;
            case Team.B:
                teamBPlayers.Remove(player);
                break;
        }

        players.Remove(player.uuid);
        state.TryRemove(player.uuid, out Player.Input _);
        Destroy(player.gameObject);
    }

    private Player ResolvePlayer(string uuid, Character character)
    {
        if (!players.TryGetValue(uuid, out Player player))
        {
            player = CreatePlayer(uuid, character);
            players[uuid] = player;
        }
        return player;
    }

    private Player CreatePlayer(string uuid, Character character)
    {
        Debug.Log("[Create Player]");
        var player = Instantiate(playerPrefabs[(int)character]).GetComponent<Player>();
        RespawnPlayer(player);
        player.Init(this, uuid);

        return player;
    }

    private void SetTeam(Player player, Team team)
    {
        if (player.team == team) return;

        switch(team)
        {
            case Team.A:
                if (teamBPlayers.Contains(player))
                    teamBPlayers.Remove(player);
                if (!teamAPlayers.Contains(player))
                    teamAPlayers.Add(player);
                    break;
            case Team.B:
                if (teamAPlayers.Contains(player))
                    teamAPlayers.Remove(player);
                if (!teamBPlayers.Contains(player))
                    teamBPlayers.Add(player);
                break;
        }

        player.SetTeam(team);
    }

    private void RespawnPlayer(Player player)
    {
        Debug.Log("[RespawnPlayer]");
        List<Transform> locations;
        switch (player.team)
        {
            case Team.A:
                locations = teamASpawnLocations;
                break;
            case Team.B:
                locations = teamBSpawnLocations;
                break;
            default:
                Debug.LogError("Unhandled team " + player.team);
                return;
        }

        player.SetPosition(SelectEmptySpawnLocation(locations));
        Debug.Log("[Set Position]");
    }

    private Transform SelectEmptySpawnLocation(List<Transform> locations)
    {
        foreach(var location in locations)
        {
            RaycastHit2D hit = Physics2D.CircleCast(location.position, 2f, Vector2.right);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                float distance = Vector2.Distance(transform.position, hit.point);
                Debug.Log("Player hit at distance: " + distance + " skipping spawn");
                continue;
            }

            return location;
        }

        return locations[^1];
    }

    string[] dataParts;
    public void OnMessage(MessageEventArgs e)
    {
        dataParts = e.Data.Split('|');

        if (dataParts.Length == 7)
        {
            Player.Input playerInput = new();

            playerInput.uuid = dataParts[0];
            playerInput.name = dataParts[1];
            playerInput.team = int.Parse(dataParts[2]) - 1;
            playerInput.joystickX = float.Parse(dataParts[3]);
            playerInput.joystickY = float.Parse(dataParts[4]);
            playerInput.characterID = int.Parse(dataParts[5]) - 1;
            playerInput.soundEffectID = int.Parse(dataParts[6]);
            playerInput.received = DateTime.UtcNow;


            HandleMessage(playerInput);
        }
        else
        {
            Debug.LogError("Invalid data format: " + e.Data);
        }
    }
}
