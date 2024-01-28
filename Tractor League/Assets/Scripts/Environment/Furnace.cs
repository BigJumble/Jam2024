using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class Furnace : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField]
    private PlayerManager.Team team;

    public AudioClip explodeCow;

    private void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("entered " + collider.name);

        // Only transport the ball / cow
        if (!collider.CompareTag("Ball")) return;


        collider.attachedRigidbody.velocity = Vector2.zero;
        collider.transform.SetPositionAndRotation(
            new Vector3(0, 0, 0), Quaternion.identity
        );

        if(!audioSource.isPlaying)
            audioSource.PlayOneShot(explodeCow);

        OnGoal?.Invoke(team);
    }

    public event Action<PlayerManager.Team> OnGoal;
}
