using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Cow : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    private bool rolling;

    public float spinVelocityMagnitudeThreshold = 3f;
    public float spinAngularVelocityThreshold = 1f;

    [SerializeField]
    private CircleCollider2D circleCollider;
    [SerializeField]
    private PolygonCollider2D polygonCollider;

    private AudioSource audioSource;

    [SerializeField]
    private List<AudioClip> mooSounds;

    private void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
        this.rb = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
    }

    private bool wasRolling;
    private void FixedUpdate()
    {
        if(Mathf.Abs(transform.position.x) > 60 || Mathf.Abs(transform.position.y) > 60)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            transform.position = Vector3.zero;
        }

        wasRolling = rolling;

        rolling = Mathf.Abs(rb.angularVelocity) > spinAngularVelocityThreshold ||
            rb.velocity.magnitude > spinVelocityMagnitudeThreshold;

        if(!wasRolling && rolling && !audioSource.isPlaying)
            audioSource.PlayOneShot(mooSounds[Random.Range(0, mooSounds.Count)]);

        animator.SetBool("Rolling", rolling);

        circleCollider.enabled = rolling;
        polygonCollider.enabled = !rolling;
    }
}
