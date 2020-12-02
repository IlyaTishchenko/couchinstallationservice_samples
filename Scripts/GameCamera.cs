using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField]
    public CameraParams cameraCityTransform = null;

    [SerializeField]
    public CameraParams cameraCityInbetweenTransform = null;

    [SerializeField]
    public CameraParams cameraMainMenuTransform = null;

    [SerializeField]
    public CameraParams cameraMainMenuInbetweenTransform = null;

    [SerializeField]
    public Camera cameraComponent = null;

    [System.NonSerialized]
    public CameraParams target = null;

    [System.NonSerialized]
    public bool isInTransition = false;

    public void Initialize()
    {
        target = cameraMainMenuTransform;

        cameraComponent.fieldOfView = target.fov;
        cameraComponent.backgroundColor = target.backgroundColor;
        transform.position = target.transform.position;
        transform.rotation = target.transform.rotation;

        startColor = target.backgroundColor;
        startFov = target.fov;
        startPos = target.transform.position;
        startQuat = target.transform.rotation;
    }
    Color startColor = Color.white;
    float startFov = 0;
    Vector3 startPos = Vector3.zero;
    Quaternion startQuat = Quaternion.identity;

    void Update()
    {
        if (!isInTransition)
        {
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;

            if (cameraComponent.fieldOfView != target.fov)
                cameraComponent.fieldOfView = target.fov;

            if (cameraComponent.backgroundColor != target.backgroundColor)
                cameraComponent.backgroundColor = target.backgroundColor;
        }
    }

    public void MoveImmediate(CameraParams to)
    {
        target = to;
        transform.position = to.transform.position;
        transform.rotation = to.transform.rotation;

        if (cameraComponent.fieldOfView != to.fov)
            cameraComponent.fieldOfView = to.fov;

        if (cameraComponent.backgroundColor != to.backgroundColor)
            cameraComponent.backgroundColor = to.backgroundColor;

        if (isInTransition)
            isInTransition = false;
    }

    public void StartMoving(CameraParams to, System.Action _callback, bool towardsLevel = false)
    {
        target = to;
        callback = _callback;

        isInTransition = true;
        startColor = cameraComponent.backgroundColor;
        startFov = cameraComponent.fieldOfView;
        startQuat = transform.rotation;
        startPos = transform.position;

        toLevel = towardsLevel;

        StartCoroutine("MoveCamera");
    }
    System.Action callback = null;
    bool toLevel = false;

    // Camera movement update coroutine
    IEnumerator MoveCamera()
    {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * cameraSpeed)
        {
            var easing = EasingFunction.EaseInOutCubic(0.0f, 1.0f, t);

            if (target == null)
            {
                Debug.LogError("No camera target found!");
            }
            transform.position = Vector3.Lerp(startPos, target.transform.position, easing);
            if (!toLevel)
            {
                transform.rotation = Quaternion.Slerp(startQuat, target.transform.rotation, easing);
                cameraComponent.fieldOfView = Mathf.Lerp(startFov, target.fov, easing);
            }
            cameraComponent.backgroundColor = Color.Lerp(startColor, target.backgroundColor, easing);

            yield return null;
        }

        if (callback != null)
        {
            callback();
            callback = null;
        }

        toLevel = false;
        isInTransition = false;
    }
    float cameraSpeed = 0.5f;
}
