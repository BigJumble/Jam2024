using UnityEngine;

public class Furnace : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("entered " + collider.name);

        // Only transport the ball / cow
        if (!collider.CompareTag("Ball")) return;


        collider.attachedRigidbody.velocity = Vector2.zero;
        collider.transform.SetPositionAndRotation(
            new Vector3(0, 0, 0), Quaternion.identity
        );
    }
}
