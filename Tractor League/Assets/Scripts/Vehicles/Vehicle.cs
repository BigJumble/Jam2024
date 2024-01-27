using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Vehicle : Entity
{
    public float maxSteerAngle = 30f;
    public Transform leftWheel, rightWheel;
    public float moveSpeed = 5f;
    public float turnSpeed = 5f;

    private float wheelAngle = 0f;
    private Rigidbody2D rb;

    private float xValue, yValue;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Get input
        xValue = Input.GetAxis("Horizontal");

        // Calculate target wheel angle
        float targetWheelAngle = maxSteerAngle * xValue;

        // Smoothly rotate wheel to target angle
        wheelAngle = Mathf.Lerp(wheelAngle, targetWheelAngle, Time.deltaTime * turnSpeed);

        // Apply rotation to wheel transform
        leftWheel.localEulerAngles = new Vector3(0, 0, wheelAngle);
        rightWheel.localEulerAngles = new Vector3(0, 0, wheelAngle);
    }

    private void FixedUpdate()
    {
        // Get input
        yValue = Input.GetAxis("Vertical");

        rb.angularDrag = Input.GetKey(KeyCode.Space) ? 1.5f : 4f;

        // Calculate forward movement
        float moveAmount = yValue * moveSpeed * Time.fixedDeltaTime;
        rb.AddForce(transform.up * moveAmount);

        // Apply rotation based on wheel angle
        float turnAmount = wheelAngle / maxSteerAngle * moveSpeed * Time.fixedDeltaTime;

        if(Mathf.Abs(yValue) > 0)
            rb.AddTorque(turnAmount * Mathf.Sign(yValue));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Bump only other players/ball and only when hitting from the front
        if (!collision.collider.CompareTag("Front Wheel") || (
            !collision.otherCollider.CompareTag("Player") &&
            !collision.otherCollider.CompareTag("Front Wheel"))
        ) return;

        // Only the faster one bumps
        if (collision.collider.attachedRigidbody.velocity.magnitude <
            collision.otherCollider.attachedRigidbody.velocity.magnitude)
            return;

        // Get the collision direction
        Vector2 collisionDirection = collision.GetContact(0).normal;

        // Set the push force magnitude
        float pushForce = 10.0f; // Adjust this value as needed

        // Apply the push force to the collided object
        collision.rigidbody.AddForce(collisionDirection * pushForce, ForceMode2D.Impulse);
    }

    public void SetInput(float x, float y)
    {
        xValue = x;
        yValue = y;
    }
}
