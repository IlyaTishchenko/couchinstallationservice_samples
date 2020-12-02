using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Movable
{
    [SerializeField]
    public Room room = null;

    [SerializeField]
    public int blockIndex = -1;

    [SerializeField]
    public Animator playerAnimator = null;

    [SerializeField]
    public AnimationCurve idleMoveCurve = null;

    [SerializeField]
    AudioSource footstepsSound = null;

    [SerializeField]
    public Transform SpeechCloudTransform = null;

    [SerializeField]
    public float idleMoveSpeed = 1.0f;

    [SerializeField]
    public ParticleSystem dustParticles = null;

    [SerializeField]
    public Direction startDir = Direction.right;

    [SerializeField]
    public ParticleSystem questionSignParticles = null;

    [System.NonSerialized]
    public Direction currentDir = Direction.right;

    [System.NonSerialized]
    public float idleMove = 0.0f;

    [System.NonSerialized]
    public Direction targetDir = Direction.right;

    [System.NonSerialized]
    public bool ShouldThink = true;

    [System.NonSerialized]
    public bool isFalseMovingOrPushing = false;

    [System.NonSerialized]
    public Game game = null;

    void Start()
    {
        targetQuat = transform.rotation;
        currentDir = startDir;
        targetDir = startDir;
    }

    void Update()
    {
        if (blockIndex == -1)
            return;

        if (currentDir != targetDir)
        {
            targetQuat = Quaternion.LookRotation(GetFacing(targetDir) - transform.position);
            currentDir = targetDir;
        }

        Move();

        if (!ShouldThink)
        {
            standStillTime = 0.0f;
            thinkingTime = 0.0f;
            thinking = false;
            if (thinkingBlending != 0.0f)
            {
                thinkingBlending = 0.0f;
                playerAnimator.SetFloat("Thinking", thinkingBlending);
                if (questionSignParticles.isPlaying)
                    questionSignParticles.Stop();
            }
            return;
        }

        var thinkingBlendingSpeed = 5.0f;
        if (!thinking)
        {
            if (transform.localPosition != standStillPosition)
            {
                standStillPosition = transform.localPosition;
                standStillTime = 0.0f;

                if (thinkingBlending != 0.0f)
                {
                    thinkingBlending = 0.0f;
                    playerAnimator.SetFloat("Thinking", thinkingBlending);
                }
            }
            else
            {
                if (standStillTime < standStillTimeDuration)
                {
                    standStillTime += Time.deltaTime;
                }
                else
                {
                    if (thinkingBlending < 1.0f)
                    {
                        thinkingBlending += Time.deltaTime * thinkingBlendingSpeed;
                        playerAnimator.SetFloat("Thinking", thinkingBlending);
                    }
                    else
                    {
                        standStillTime = 0.0f;
                        thinkingTime = 0.0f;
                        thinking = true;
                        thinkingBlending = 1.0f;
                        playerAnimator.SetFloat("Thinking", thinkingBlending);
                    }
                }
            }

            if (questionSignParticles.isPlaying)
                questionSignParticles.Stop();
        }
        else
        {
            if (thinkingTime < thinkingTimeDuration && transform.localPosition == standStillPosition)
            {
                thinkingTime += Time.deltaTime;
            }
            else
            {
                if (thinkingBlending > 0.0f)
                {
                    thinkingBlending -= Time.deltaTime * thinkingBlendingSpeed;
                    playerAnimator.SetFloat("Thinking", thinkingBlending);
                }
                else
                {
                    standStillTime = 0.0f;
                    thinkingTime = 0.0f;
                    thinking = false;
                    thinkingBlending = 0.0f;
                    playerAnimator.SetFloat("Thinking", thinkingBlending);
                }
            }

            if (!questionSignParticles.isPlaying)
                questionSignParticles.Play();
        }
    }
    Vector3 standStillPosition = Vector3.zero;
    float standStillTime = 0.0f;
    const float standStillTimeDuration = 10.0f;
    bool thinking = false;
    float thinkingTime = 0.0f;
    const float thinkingTimeDuration = 3.0f;
    float thinkingBlending = 0.0f;

    void Move()
    {
        var target = room.newblocks[blockIndex].position;

        if (isFalseMovingOrPushing)
            return;

        if (!isStanding())
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, moveSpeed * Time.deltaTime);
            if (idleMove < 1.0f)
                idleMove += Time.deltaTime * idleMoveSpeed;
        }
        else
        {
            if (idleMove > 0.0f)
                idleMove -= Time.deltaTime * idleMoveSpeed;
        }

        playerAnimator.SetFloat("IdleMove", idleMoveCurve.Evaluate(idleMove));

        if (transform.rotation != targetQuat)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetQuat, rotationSpeed * Time.deltaTime);
        }
    }

    public bool isStanding()
    {
        var target = room.newblocks[blockIndex].position;
        if (transform.localPosition == target)
            return true;

        return false;
    }

    public void FalseMoveOrPush(Direction dir, bool isPushing)
    {
        if (!isFalseMovingOrPushing)
        {
            isFalseMovingOrPushing = true;
            targetDir = dir;
            StartCoroutine(FalseMoveOrPushCoroutine(dir, isPushing));
        }
    }

    // Fake movement animation coroutine
    IEnumerator FalseMoveOrPushCoroutine(Direction dir, bool isPushing)
    {
        if (isPushing)
            playerAnimator.SetFloat("MovePush", 1.0f);
        else
            playerAnimator.SetFloat("MovePush", 0.0f);

        var startPos = transform.position;
        var target = GetFacing(dir) - startPos;
        target.Normalize();
        target *= 0.25f;

        var falseMoveSpeed = isPushing ? 4.0f : 5.0f;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * falseMoveSpeed)
        {
            var easing = EasingFunction.EaseInOutCubic(0.0f, 1.0f, Mathf.Clamp(t, 0.0f, 1.0f));
            transform.position = Vector3.Lerp(startPos, target + startPos, easing);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target), rotationSpeed * Time.deltaTime);
            if (idleMove < 1.0f)
                idleMove += Time.deltaTime * idleMoveSpeed;

            playerAnimator.SetFloat("IdleMove", idleMoveCurve.Evaluate(idleMove));

            yield return null;
        }

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * falseMoveSpeed)
        {
            var easing = EasingFunction.EaseInOutCubic(0.0f, 1.0f, Mathf.Clamp(t, 0.0f, 1.0f));
            transform.position = Vector3.Lerp(target + startPos, startPos, easing);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target), rotationSpeed * Time.deltaTime);
            if (idleMove > 0.0f)
                idleMove -= Time.deltaTime * idleMoveSpeed;

            playerAnimator.SetFloat("IdleMove", idleMoveCurve.Evaluate(idleMove));

            yield return null;
        }

        isFalseMovingOrPushing = false;
    }

    public void PlayFootstepsSound()
    {
        if (!game.isSFXOn)
            return;

        footstepsSound.Play();
    }

    public void Animate(bool isPushing)
    {
        var target = room.newblocks[blockIndex].position;

        if (isPushing)
            playerAnimator.SetFloat("MovePush", 1.0f);
        else
            playerAnimator.SetFloat("MovePush", 0.0f);
    }

    public override void MoveImmediate()
    {
        transform.localPosition = room.newblocks[blockIndex].position;
        transform.rotation = Quaternion.LookRotation(GetFacing(targetDir) - transform.position);
    }

    public Vector3 GetFacing(Direction dir)
    {
        var target = Vector3.zero;
        switch(dir)
        {
            case Direction.left:
            {
                target = transform.parent.TransformPoint(transform.localPosition + Vector3.left);
            } break;
            case Direction.right:
            {
                target = transform.parent.TransformPoint(transform.localPosition + Vector3.right);
            } break;
            case Direction.top:
            {
                target = transform.parent.TransformPoint(transform.localPosition + Vector3.forward);
            } break;
            case Direction.bottom:
            {
                target = transform.parent.TransformPoint(transform.localPosition + Vector3.back);
            } break;
        }
        return target;
    }
    Quaternion targetQuat;
}
