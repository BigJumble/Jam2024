using System;
using System.Collections;
using System.Collections.Generic;
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
        public DateTime received;
    }

    public string uuid;
    public PlayerManager.Team team;
    private PlayerManager pm;

    private Vehicle vehicle;
    private SpeechBubble speech;

    [SerializeField]
    private List<AudioClip> soundEffects;

    [SerializeField]
    private AudioSource audioSource;

    private DateTime lastUpdate;

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

    public void Touch(float x, float y, DateTime lastUpdate, int soundEffectId)
    {
        this.lastUpdate = lastUpdate;
        vehicle.SetInput(x, y);
        if (soundEffectId > 0 && soundEffectId <= soundEffects.Count && !audioSource.isPlaying)
        {
            var clip = soundEffects[soundEffectId - 1];
            audioSource.PlayOneShot(clip);
            speech.TalkForSeconds(clip.length);
        }
    }

    private IEnumerator DeathCounter()
    {
        while(true)
        {
            yield return new WaitForSeconds(5f);
            if ((DateTime.UtcNow - lastUpdate).TotalSeconds > 5) break;
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
}
