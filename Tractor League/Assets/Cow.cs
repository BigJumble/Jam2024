using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if(Mathf.Abs(transform.position.x) > 60 || Mathf.Abs(transform.position.y) > 60)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            transform.position = Vector3.zero;
        }


        rolling = Mathf.Abs(rb.angularVelocity) > spinAngularVelocityThreshold ||
            rb.velocity.magnitude > spinVelocityMagnitudeThreshold;


        animator.SetBool("Rolling", rolling);

        circleCollider.enabled = rolling;
        polygonCollider.enabled = !rolling;
    }
}
