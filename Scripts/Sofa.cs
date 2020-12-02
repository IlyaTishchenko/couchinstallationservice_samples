using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Sofa : Movable
{
    [SerializeField]
    public Room room = null;

    [SerializeField]
    public int leftBlockIndex = -1;

    [SerializeField]
    public int rightBlockIndex = -1;

    [SerializeField]
    public ParticleSystem shineParticles = null;

    [SerializeField]
    public bool isShining = true;

    [SerializeField]
    public AudioSource scratchSound = null;

    [SerializeField]
    public List<TrailRenderer> trailRenderers = null;

    [System.NonSerialized]
    public bool isMoving = false;

    void Start()
    {
        if (leftBlockIndex == -1 || rightBlockIndex == -1)
            return;

        leftTarget = room.newblocks[leftBlockIndex].position;
        rightTarget = room.newblocks[rightBlockIndex].position;
    }

    void Update()
    {
        if (leftBlockIndex == -1 || rightBlockIndex == -1)
            return;

        Move();

        if (!room.game.isSFXOn)
            return;

        if (isMoving)
        {
            if (!scratchSound.isPlaying)
                scratchSound.Play();
        }
    }
    Vector3 leftTarget;
    Vector3 rightTarget;

    void Move()
    {
        if (leftTarget != room.newblocks[leftBlockIndex].position || rightTarget != room.newblocks[rightBlockIndex].position)
        {
            leftTarget = Vector3.MoveTowards(leftTarget, room.newblocks[leftBlockIndex].position, moveSpeed * Time.deltaTime);
            rightTarget = Vector3.MoveTowards(rightTarget, room.newblocks[rightBlockIndex].position, moveSpeed * Time.deltaTime);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        var target = (leftTarget + rightTarget) / 2;
        transform.localPosition = target;

        transform.LookAt(transform.parent.TransformPoint(leftTarget));
    }

    public override void MoveImmediate()
    {
        if (leftBlockIndex == -1 || rightBlockIndex == -1)
            return;

        foreach (var tr in trailRenderers)
            tr.enabled = false;

        leftTarget = room.newblocks[leftBlockIndex].position;
        rightTarget = room.newblocks[rightBlockIndex].position;
        var target = (leftTarget + rightTarget) / 2;
        transform.localPosition = target;
        transform.LookAt(transform.parent.TransformPoint(leftTarget));

        foreach (var tr in trailRenderers)
            tr.enabled = true;
    }
}
