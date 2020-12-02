using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for holding up camera parameters to animate between
public class CameraParams : MonoBehaviour
{
    [SerializeField]
    public float fov = 45;

    [SerializeField]
    public Color backgroundColor = new Color(0.73f, 0.87f, 0.98f);

    [SerializeField]
    public bool isLookingAtCenter = true;

    [SerializeField]
    public Vector3 centerOffset = Vector3.zero;

    [SerializeField]
    public bool autoMove = false;

    [SerializeField]
    public float amplitude = 1.0f;

    [SerializeField]
    public float frequency = 1.0f;

    [SerializeField]
    public GameCamera gameCamera = null;

    [System.NonSerialized]
    public Player player = null;

    void Start()
    {
        if (transform.parent != null)
        {
            player = transform.parent.GetComponentInChildren<Player>();
        }

        startPos = transform.position;
        startRot = transform.rotation;
        startLocalPos = transform.localPosition;
    }
    Vector3 startPos = Vector3.zero;
    Quaternion startRot = Quaternion.identity;
    Vector3 startLocalPos = Vector3.zero;

    void Update()
    {
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        if (transform.parent != null)
        {
            if (player == null)
                return;

            var localPos = player.transform.localPosition;
            var pos = startLocalPos;
            pos.x += localPos.x / 2.0f;
            pos.z += localPos.z / 2.0f;
            transform.localPosition += (pos - transform.localPosition) * Time.deltaTime * 0.2f;

            transform.LookAt(transform.parent.position);
        }
        else
        {
            if (autoMove)
            {
                transform.position = startPos + Vector3.right * Mathf.Sin(t * frequency) * amplitude;

                if (gameCamera != null && !gameCamera.isInTransition && gameCamera.target == this)
                    t += Time.deltaTime;
            }

            if (isLookingAtCenter)
                transform.LookAt(centerOffset);
        }
    }
    float t = 0.0f;
}
