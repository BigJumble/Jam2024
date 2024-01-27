using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public struct Input
    {
        public string uuid;
        public string name;
        public int team;
        public float joystickX;
        public float joystickY;
        public int characterID;
        public int soundEffectID;
    }

    public string uuid;
    public PlayerManager.Team team;
    private PlayerManager pm;

    private Vehicle vehicle;
    private SpeechBubble speech;

    private DateTime lastTouch;

    public void SetPosition(Transform transform)
    {
        this.transform.position = transform.position;
    }

    public void SetTeam(PlayerManager.Team team)
    {
        this.team = team;
    }

    public void Init(PlayerManager pm, string uuid)
    {
        this.uuid = uuid;
        this.pm = pm;
        this.team = PlayerManager.Team.None;
        StartCoroutine(DeathCounter());
    }

    public void Touch(float x, float y)
    {
        lastTouch = DateTime.Now;
        vehicle.SetInput(x, y);
    }

    private IEnumerator DeathCounter()
    {
        while(true)
        {
            yield return new WaitForSeconds(5f);
            if ((DateTime.UtcNow - lastTouch).TotalSeconds > 5) break;
        }

        Debug.Log("[NotifyOfDeath]");

        pm.NotifyOfDeath(this);

        yield return null;
    }

    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();       
        speech = GetComponentInChildren<SpeechBubble>();
    }

    private void Update()
    {
        if(UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            speech.TalkForSeconds(4f);
        }
    }
}
