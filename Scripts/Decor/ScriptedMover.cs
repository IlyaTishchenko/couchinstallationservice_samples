using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for moveable objects along the certain path
[ExecuteInEditMode]
public class ScriptedMover : MonoBehaviour
{
    [SerializeField]
    public List<Vector3> points = null;

    [Space(20)]
    [SerializeField]
    Transform moverTransform = null;

    [SerializeField]
    float speed = 1.0f;

    [SerializeField]
    float rotationSpeed = 200.0f;

    [SerializeField]
    float leanAngle = 60.0f;

    [Space(20)]
    [SerializeField]
    bool animated = true;

    [SerializeField]
    Animator animator = null;

    [Space(20)]
    [SerializeField]
    bool isResting = true;

    [SerializeField]
    bool manualLiftoff = false;

    [SerializeField]
    GameObject movingGameObject = null;

    [SerializeField]
    GameObject sittingGameObject = null;

    [Space(20)]
    [SerializeField]
    Sofa sofa = null;

    void Start()
    {
        if (!Application.isPlaying)
            return;

        restTime = Random.Range(2.0f, 8.0f);
    }

    public void LiftOff()
    {
        if (movementState == MovementState.Resting)
        restTimer = restTime;
    }

    void CalculatePosition()
    {
        position = Vector3.Lerp(points[currentIndex], points[nextIndex], t);

        if (movementState == MovementState.Resting)
            return;

        t += speed * Time.deltaTime / Vector3.Distance(points[currentIndex], points[nextIndex]);

        if (t > 1.0f)
        {
            t = 0.0f;

            if (currentIndex == points.Count - 1)
                currentIndex = 0;
            else
                currentIndex++;

            if (nextIndex == points.Count - 1)
                nextIndex = 0;
            else
                nextIndex++;
        }
    }

    void SetMovingParams()
    {
        moverTransform.Rotate(0f, 0f, -prevz, Space.Self);
        var towards = points[nextIndex] - points[currentIndex];
        towards.Normalize();
        zturn = Mathf.Clamp(Vector3.SignedAngle(towards, Quaternion.Euler(moverTransform.eulerAngles) * Vector3.forward, Vector3.up), -leanAngle, leanAngle);

        moverTransform.position += (transform.position + position - moverTransform.position) * Time.deltaTime * 5.0f;
        if (transform.position + position - moverTransform.position != Vector3.zero)
        {
            var lookRotation = Quaternion.LookRotation(transform.position + position - moverTransform.position, Vector3.up);
            moverTransform.rotation = Quaternion.RotateTowards(moverTransform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        if (prevz < zturn) prevz += Mathf.Min(rotationSpeed * Time.deltaTime, zturn - prevz);
        else if (prevz >= zturn) prevz -= Mathf.Min(rotationSpeed * Time.deltaTime, prevz - zturn);
        prevz = Mathf.Clamp(prevz, -leanAngle, leanAngle);
        moverTransform.Rotate(0f, 0f, prevz, Space.Self);

        if (Vector3.Dot(towards, Vector3.up) > 0.0f)
        {
            if (animated && animator != null)
                animator.SetFloat("flySpeed", 1.0f);
        }
        else
        {
            if (animated && animator != null)
                animator.SetFloat("flySpeed", 0.0f);
        }

        if (animated && animator != null && animator.GetBool("landing") == true)
            animator.SetBool("landing", false);

        if (movingGameObject != null && !movingGameObject.activeSelf)
            movingGameObject.SetActive(true);
        if (sittingGameObject != null && sittingGameObject.activeSelf)
            sittingGameObject.SetActive(false);
    }

    void SetLandingParams()
    {
        moverTransform.position += (transform.position + position - moverTransform.position) * Time.deltaTime * 5.0f;
        moverTransform.LookAt(transform.position + points[nextIndex]);

        if (animated && animator != null && animator.GetBool("landing") == false)
            animator.SetBool("landing", true);
    }

    void SetRestingParams()
    {
        moverTransform.position += (transform.position + position - moverTransform.position) * Time.deltaTime * 5.0f;
        moverTransform.LookAt(transform.position + points[nextIndex]);

        if (Vector3.Distance(moverTransform.position, transform.position + position) < 0.1f)
        {
            if (movingGameObject != null && movingGameObject.activeSelf)
                movingGameObject.SetActive(false);
            if (sittingGameObject != null && !sittingGameObject.activeSelf)
                sittingGameObject.SetActive(true);

            if (animated && animator != null && animator.GetBool("landing") == true)
                animator.SetBool("landing", false);
        }
    }

    void UpdateMovement()
    {
        CalculatePosition();

        switch (movementState)
        {
            case MovementState.Moving:
            {
                SetMovingParams();
            }
            break;
            case MovementState.Landing:
            {
                SetLandingParams();
            }
            break;
            case MovementState.Resting:
            {
                SetRestingParams();
            }
            break;
        }
    }

    void Update()
    {
        if (!Application.isPlaying && points.Count > 1)
        {
            moverTransform.LookAt(transform.position + points[1]);
            return;
        }

        if (sofa != null && sofa.isMoving)
            LiftOff();

        if (isResting)
        {
            if (currentIndex == points.Count - 1)
            {
                movementState = MovementState.Landing;
            }

            if (currentIndex == 0 && t == 0.0f)
            {
                if (restTimer < restTime)
                {
                    if (!manualLiftoff)
                        restTimer += Time.deltaTime;

                    movementState = MovementState.Resting;
                }
                else
                {
                    restTimer = 0.0f;
                    movementState = MovementState.Moving;
                    restTime = Random.Range(4.0f, 8.0f);
                }
            }
        }
        else
            movementState = MovementState.Moving;

        UpdateMovement();
    }
    Vector3 position = Vector3.zero;
    float t = 0.0f;
    int currentIndex = 0;
    int nextIndex = 1;
    float zturn = 0.0f;
    float prevz = 0.0f;
    float restTime = 0.0f;
    float restTimer = 0.0f;
    MovementState movementState = MovementState.Resting;

    public enum MovementState
    {
        Moving,
        Landing,
        Resting,
    }

    #if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        var oldColor = Gizmos.color;

        Gizmos.color = Color.magenta;

        if (points.Count > 1)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Gizmos.DrawLine(transform.position + points[i], transform.position + points[i + 1]);
            }

            Gizmos.DrawLine(transform.position + points[points.Count - 1], transform.position + points[0]);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + position, 0.1f);

        Gizmos.color = oldColor;
    }

    #endif
}
