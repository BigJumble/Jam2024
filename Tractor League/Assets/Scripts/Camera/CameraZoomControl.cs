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

    private void Update()
    {
        if (targets.Count == 0) return;

        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        foreach (Transform target in targets)
        {
            bounds.Encapsulate(target.position);
        }

        float maxDistance = Mathf.Max(bounds.size.x, bounds.size.y);
        targetSize = 10 + (maxDistance / 2.0f);

        camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, targetSize, ref velocity, smoothTime);
    }
}
