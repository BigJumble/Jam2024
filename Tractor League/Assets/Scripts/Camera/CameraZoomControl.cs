using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoomControl : MonoBehaviour
{
    public List<Transform> targets;
    private Camera camera;
    public float smoothTime = 0.5f;
    private float targetSize;
    private float velocity;
    [SerializeField]
    private float factor = 2f;
    private Transform cow;

    void Start()
    {
        cow = GameObject.FindGameObjectWithTag("Ball")?.transform;
        camera = GetComponent<Camera>();
        FindObjectOfType<PlayerManager>().OnUpdatePlayers += UpdateTargets;
    }

    public void UpdateTargets(List<Player> players)
    {
        targets = players.ConvertAll((x) => x.transform);
        targets.Add(cow);
    }

    float maxDistance;
    float distance;
    private void Update()
    {
        if (targets.Count == 0) return;

        maxDistance = 0;
        foreach (Transform target in targets)
        {
            distance = target.position.magnitude;
            if (distance > maxDistance)
                maxDistance = distance;
        }

        targetSize = maxDistance * factor;

        camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, Mathf.Clamp(targetSize, 30f, 33f), ref velocity, smoothTime);
    }
}
