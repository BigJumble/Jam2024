using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class GoalManager : MonoBehaviour
{
    private Dictionary<PlayerManager.Team, int> teamScore;

    private List<Furnace> furnaces;

    [SerializeField]
    private TextMeshProUGUI score;

    [SerializeField]
    private TextMeshProUGUI clock;

    private void Start()
    {
        teamScore = new Dictionary<PlayerManager.Team, int>()
        {
            {  PlayerManager.Team.A, 0 },
            {  PlayerManager.Team.B, 0 },
        };

        furnaces = FindObjectsOfType<Furnace>().ToList();
        foreach(var furnace in furnaces)
        {
            furnace.OnGoal += OnGoalHandler;
        }
    }

    private void OnDestroy()
    {
        foreach (var furnace in furnaces)
        {
            furnace.OnGoal -= OnGoalHandler;
        }
    }

    private void FixedUpdate()
    {
        SetUIScore();
        SetClock();
    }

    private void SetUIScore()
    {
        score.text = $"{teamScore[PlayerManager.Team.A]} - {teamScore[PlayerManager.Team.B]}";
    }

    private void SetClock()
    {
        float timeSinceStart = Time.time;
        int minutes = Mathf.FloorToInt(timeSinceStart / 60);
        int seconds = Mathf.FloorToInt(timeSinceStart % 60);

        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        clock.text = formattedTime;
    }

    private void OnGoalHandler(PlayerManager.Team team)
    {
        teamScore[team] += 1;
    }
}
